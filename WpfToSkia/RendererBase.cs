using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfToSkia.ExtensionsMethods;

namespace WpfToSkia
{
    public abstract class RendererBase<T> : IRenderer where T : IDrawingContext
    {
        private class RenderQueueItem
        {
            public Action Action { get; set; }
            public TaskCompletionSource<Object> CompletionSource { get; set; }
        }

        private WriteableBitmap _bitmap;
        private Thread _renderThread;
        private SkiaHost _host;
        private ScrollViewer _scrollViewer;
        private SkiaTree _tree;
        private List<BindingEventContainer> _containers;
        private ProducerConsumerQueue<RenderQueueItem> _renderThreadQueue;
        private ActionThrottle _scrolling_throttle;
        private ActionThrottle _sizing_throttle;
        private ActionThrottle _tree_change_throttle;
        private Dictionary<BindingProperty, ActionThrottle> _bindingThrottlers;

        public event EventHandler<WriteableBitmap> SourceChanged;

        public WriteableBitmap Source
        {
            get { return _bitmap; }
        }

        public bool IsVirtualizing
        {
            get { return _host.ActualWidth * _host.ActualHeight > MaximumBitmapSize && _scrollViewer != null; }
        }

        public int MaximumBitmapSize { get; set; }

        public double BindingFPS { get; set; }

        public double ScrollingFPS { get; set; }

        public double SizingFPS { get; set; }

        public bool EnableBinding { get; set; }

        /// <summary>
        /// Gets or sets the framework element data item.
        /// </summary>
        public static readonly DependencyProperty IsInitializedProperty = DependencyProperty.RegisterAttached("IsInitialized", typeof(bool), typeof(RendererBase<T>), new FrameworkPropertyMetadata(false));
        /// <summary>
        /// Sets the IsInitialized attached property.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsInitialized(SkiaFrameworkElement element, bool value)
        {
            element.SetValue(IsInitializedProperty, value);
        }
        /// <summary>
        /// Gets the IsInitialized attached property.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static bool GetIsInitialized(SkiaFrameworkElement element)
        {
            return (bool)element.GetValue(IsInitializedProperty);
        }

        public RendererBase()
        {
            _renderThreadQueue = new ProducerConsumerQueue<RenderQueueItem>();
            _bindingThrottlers = new Dictionary<BindingProperty, ActionThrottle>();
            MaximumBitmapSize = 4000 * 4000;
            BindingFPS = 60;
            ScrollingFPS = 30;
            SizingFPS = 30;
            EnableBinding = true;
        }

        public void Init(SkiaHost host)
        {
            _host = host;

            _scrolling_throttle = new ActionThrottle(TimeSpan.FromMilliseconds(1000d / ScrollingFPS), _host.Dispatcher);
            _sizing_throttle = new ActionThrottle(TimeSpan.FromMilliseconds(1000d / SizingFPS), _host.Dispatcher);
            _tree_change_throttle = new ActionThrottle(TimeSpan.FromMilliseconds(1000d / 30), _host.Dispatcher);

            _host.SizeChanged += _host_SizeChanged;
            _scrollViewer = _host.FindAncestor<ScrollViewer>();

            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged += _scrollViewer_ScrollChanged;
                _scrollViewer.SizeChanged += _scrollViewer_SizeChanged;
            }

            InitSurface();
            ReloadVisualTree();

            if (_renderThread == null)
            {
                _renderThread = new Thread(RenderThreadMethod);
                _renderThread.IsBackground = true;
                _renderThread.Start();
            }

            Render();
        }

        private void _scrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsVirtualizing)
            {
                _sizing_throttle.ResetReplace(() =>
                {
                    InitSurface();
                    Render();
                });
            }
        }

        private void _host_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _sizing_throttle.ResetReplace(() =>
            {
                InitSurface();
                Render();
            });
        }

        private void RenderThreadMethod()
        {
            while (true)
            {
                RenderQueueItem item = _renderThreadQueue.BlockDequeue();

                item.Action();
                item.CompletionSource.SetResult(true);
            }
        }

        private void _scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            _scrolling_throttle.ResetReplace(() =>
            {
                if (IsVirtualizing)
                {
                    Render();
                }
            });
        }

        protected void ReloadVisualTree()
        {
            _tree = SkiaTreeHelper.LoadTree(_host.Child);
            _containers = new List<BindingEventContainer>();

            _bindingThrottlers.ToList().ForEach(x => x.Value.Dispose());
            _bindingThrottlers.Clear();

            InitElement(_tree.Root);
        }

        private void Element_ChildAdded(object sender, FrameworkElement element)
        {
            var skiaElement = _tree.Inject(element);
            InitElement(skiaElement);

            _tree_change_throttle.ResetReplace(() =>
            {
                Render();
            });
        }

        private void Element_ChildRemoved(object sender, FrameworkElement element)
        {
            var skiaElement = _tree.Eject(element);

            if (skiaElement != null)
            {
                DisposeElement(skiaElement);
            }

            _tree_change_throttle.ResetReplace(() =>
            {
                Render();
            });
        }

        private void InitElement(SkiaFrameworkElement element)
        {
            if (!GetIsInitialized(element))
            {
                if (EnableBinding)
                {
                    foreach (var bindingProperty in element.GetBindingProperties())
                    {
                        var container = BindingEventContainer.Generate(element, bindingProperty);
                        container.ValueChanged += OnBindingContainerValueChanged;
                        _containers.Add(container);

                        _bindingThrottlers.Add(container.BindingProperty, new ActionThrottle(TimeSpan.FromMilliseconds(1000d / BindingFPS), _host.Dispatcher));
                    }
                }

                element.ChildAdded += Element_ChildAdded;
                element.ChildRemoved += Element_ChildRemoved;

                SetIsInitialized(element, true);
            }

            foreach (var child in element.Children)
            {
                InitElement(child);
            }
        }

        private void DisposeElement(SkiaFrameworkElement element)
        {
            element.ChildAdded -= Element_ChildAdded;
            element.ChildRemoved -= Element_ChildRemoved;
        }

        private void OnBindingContainerValueChanged(object sender, BindingEventArgs e)
        {
            ActionThrottle throttle = null;

            if (_bindingThrottlers.TryGetValue(e.BindingProperty, out throttle))
            {

                throttle.ResetReplace(() =>
                {
                    RenderSingle(e.SkiaElement, e.BindingProperty.Mode);
                });
            }
        }

        private void InitSurface()
        {
            double renderWidth = _host.ActualWidth;
            double renderHeight = _host.ActualHeight;

            if (_scrollViewer != null && IsVirtualizing)
            {
                renderWidth = _scrollViewer.ViewportWidth;
                renderHeight = _scrollViewer.ViewportHeight;
            }

            _bitmap = new WriteableBitmap((int)Math.Max(renderWidth, 1), (int)Math.Max(renderHeight, 1), 96, 96, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);

            int width = (int)_bitmap.Width;
            int height = (int)_bitmap.Height;

            _bitmap.Lock();

            OnSurfaceCreated(_bitmap.BackBuffer, width, height, width * 4);

            _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            _bitmap.Unlock();
        }

        protected abstract void OnSurfaceCreated(IntPtr backBuffer, int width, int height, int stride);

        protected abstract T CreateDrawingContext();

        protected void Render()
        {
            _bitmap.Lock();

            T context = CreateDrawingContext();

            context.BeginDrawing();

            context.Clear(Colors.Transparent);
            _tree.Root.Render(context, new Rect(0, 0, _host.ActualWidth, _host.ActualHeight), GetVirtualizedBounds(), 1);

            context.EndDrawing();

            _bitmap.AddDirtyRect(new Int32Rect(0, 0, (int)_bitmap.Width, (int)_bitmap.Height));
            _bitmap.Unlock();

            OnSourceChanged();
        }

        private void RenderSingle(SkiaFrameworkElement element, BindingPropertyMode mode)
        {
            SkiaFrameworkElement toRender = element.Parent != null ? element.Parent : element;

            var bounds = toRender.Bounds;

            if (mode == BindingPropertyMode.AffectsLayout)
            {
                toRender.InvalidateBounds();

                if (toRender.Bounds.Contains(bounds))
                {
                    bounds = toRender.Bounds;
                }
                else
                {
                    bounds.Width += 1;
                    bounds.Height += 1;
                }
            }

            if (bounds.Left < _bitmap.Width && bounds.Top < _bitmap.Height)
            {
                _bitmap.Lock();

                T context = CreateDrawingContext();

                context.BeginDrawing();

                context.ClipRect(bounds, new CornerRadius());
                context.Clear(Colors.Transparent);

                _tree.Root.Invalidate(context, bounds, 1);

                context.EndDrawing();

                if (bounds.Right > _bitmap.Width)
                {
                    bounds.Width = _bitmap.Width - bounds.Left;
                }

                if (bounds.Bottom > _bitmap.Height)
                {
                    bounds.Height = _bitmap.Height - bounds.Top;
                }

                _bitmap.AddDirtyRect(new Int32Rect((int)Math.Max(bounds.Left, 0), (int)Math.Max(bounds.Top, 0), (int)bounds.Width, (int)bounds.Height));
                _bitmap.Unlock();
            }
        }

        protected Task InvokeRenderThread(Action action)
        {
            TaskCompletionSource<object> source = new TaskCompletionSource<object>();

            _renderThreadQueue.BlockEnqueue(new RenderQueueItem()
            {
                Action = action,
                CompletionSource = source,
            });

            return source.Task;
        }

        private Rect GetVirtualizedBounds()
        {
            return IsVirtualizing ? new Rect((int)_scrollViewer.HorizontalOffset, (int)_scrollViewer.VerticalOffset, (int)_scrollViewer.ViewportWidth, (int)_scrollViewer.ViewportHeight) : new Rect(0, 0, _host.ActualWidth, _host.ActualHeight);
        }

        protected virtual void OnSourceChanged()
        {
            SourceChanged?.Invoke(this, _bitmap);
        }
    }
}
