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
    /// <summary>
    /// Represents an <see cref="IRenderer"/> base class.
    /// </summary>
    /// <typeparam name="T">Type of <see cref="IDrawingContext"/></typeparam>
    /// <seealso cref="WpfToSkia.IRenderer" />
    public abstract class RendererBase<T> : DependencyObject, IRenderer where T : IDrawingContext
    {
        private SkiaHost _host;
        private ScrollViewer _scrollViewer;
        private List<BindingEventContainer> _containers;
        private ActionThrottle _scrolling_throttle;
        private ActionThrottle _sizing_throttle;
        private ActionThrottle _tree_change_throttle;
        private Dictionary<BindingProperty, ActionThrottle> _bindingThrottlers;

        #region Events

        /// <summary>
        /// Occurs when the source bitmap has changed. Usually called when the drawing surface size has changed.
        /// </summary>
        public event EventHandler<WriteableBitmap> SourceChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the skia tree.
        /// </summary>
        public SkiaTree SkiaTree { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="SkiaHost" /> is inside a ScrollViewer and the its size exceeded the <see cref="MaximumBitmapSize" />.
        /// </summary>
        public bool IsVirtualizing
        {
            get { return _host.ActualWidth * _host.ActualHeight > MaximumBitmapSize && _scrollViewer != null; }
        }

        /// <summary>
        /// Gets the current source bitmap.
        /// </summary>
        public WriteableBitmap Source
        {
            get { return (WriteableBitmap)GetValue(SourceProperty); }
            private set { SetValue(SourceProperty, value); }
        }
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(WriteableBitmap), typeof(RendererBase<T>), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the maximum size of the bitmap.
        /// When the actual size exceeds this value, the renderer will start to virtualize the rendering.
        /// </summary>
        public int MaximumBitmapSize
        {
            get { return (int)GetValue(MaximumBitmapSizeProperty); }
            set { SetValue(MaximumBitmapSizeProperty, value); }
        }
        public static readonly DependencyProperty MaximumBitmapSizeProperty =
            DependencyProperty.Register("MaximumBitmapSize", typeof(int), typeof(RendererBase<T>), new PropertyMetadata(4000 * 4000));

        /// <summary>
        /// Gets or sets the maximum response rate for properties changes.
        /// </summary>
        public double BindingFPS
        {
            get { return (double)GetValue(BindingFPSProperty); }
            set { SetValue(BindingFPSProperty, value); }
        }
        public static readonly DependencyProperty BindingFPSProperty =
            DependencyProperty.Register("BindingFPS", typeof(double), typeof(RendererBase<T>), new PropertyMetadata(60.0));

        /// <summary>
        /// Gets or sets the maximum response rate for parent ScrollViewer changes.
        /// </summary>
        public double ScrollingFPS
        {
            get { return (double)GetValue(ScrollingFPSProperty); }
            set { SetValue(ScrollingFPSProperty, value); }
        }
        public static readonly DependencyProperty ScrollingFPSProperty =
            DependencyProperty.Register("ScrollingFPS", typeof(double), typeof(RendererBase<T>), new PropertyMetadata(30.0));

        /// <summary>
        /// Gets or sets the maximum response rate for size changes.
        /// </summary>
        public double SizingFPS
        {
            get { return (double)GetValue(SizingFPSProperty); }
            set { SetValue(SizingFPSProperty, value); }
        }
        public static readonly DependencyProperty SizingFPSProperty =
            DependencyProperty.Register("SizingFPS", typeof(double), typeof(RendererBase<T>), new PropertyMetadata(30.0));

        /// <summary>
        /// Gets or sets a value indicating whether to enable invalidation of elements when their properties changes.
        /// </summary>
        public bool EnableBinding
        {
            get { return (bool)GetValue(EnableBindingProperty); }
            set { SetValue(EnableBindingProperty, value); }
        }
        public static readonly DependencyProperty EnableBindingProperty =
            DependencyProperty.Register("EnableBinding", typeof(bool), typeof(RendererBase<T>), new PropertyMetadata(true));

        #endregion

        #region Attached Properties

        public static readonly DependencyProperty IsInitializedProperty = DependencyProperty.RegisterAttached("IsInitialized", typeof(bool), typeof(RendererBase<T>), new FrameworkPropertyMetadata(false));
        public static void SetIsInitialized(SkiaFrameworkElement element, bool value)
        {
            element.SetValue(IsInitializedProperty, value);
        }
        public static bool GetIsInitialized(SkiaFrameworkElement element)
        {
            return (bool)element.GetValue(IsInitializedProperty);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RendererBase{T}"/> class.
        /// </summary>
        public RendererBase()
        {
            _bindingThrottlers = new Dictionary<BindingProperty, ActionThrottle>();
            MaximumBitmapSize = 4000 * 4000;
            BindingFPS = 60;
            ScrollingFPS = 30;
            SizingFPS = 30;
            EnableBinding = true;
        }

        #endregion

        #region Size/Scroll Changes

        /// <summary>
        /// Handles the SizeChanged event of the _scrollViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Handles the SizeChanged event of the _host control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
        private void _host_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _sizing_throttle.ResetReplace(() =>
            {
                InitSurface();
                Render();
            });
        }

        /// <summary>
        /// Handles the ScrollChanged event of the _scrollViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ScrollChangedEventArgs"/> instance containing the event data.</param>
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

        #endregion

        #region Tree Injection

        /// <summary>
        /// Handles the element <see cref="SkiaFrameworkElement.ChildAdded"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="element">The element.</param>
        private void Element_ChildAdded(object sender, FrameworkElement element)
        {
            var skiaElement = SkiaTree.Inject(element);
            InitElement(skiaElement);

            _tree_change_throttle.ResetReplace(() =>
            {
                Render();
            });
        }

        /// <summary>
        /// Handles the element <see cref="SkiaFrameworkElement.ChildRemoved"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="element">The element.</param>
        private void Element_ChildRemoved(object sender, FrameworkElement element)
        {
            var skiaElement = SkiaTree.Eject(element);

            if (skiaElement != null)
            {
                DisposeElement(skiaElement);
            }

            _tree_change_throttle.ResetReplace(() =>
            {
                Render();
            });
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Loads/Reloads the WPF visual tree to <see cref="SkiaTree"/>.
        /// </summary>
        protected void ReloadVisualTree()
        {
            SkiaTree = SkiaTree.LoadTree(_host.Child);
            _containers = new List<BindingEventContainer>();

            _bindingThrottlers.ToList().ForEach(x => x.Value.Dispose());
            _bindingThrottlers.Clear();

            InitElement(SkiaTree.Root);
        }

        /// <summary>
        /// Performs a full rendering of the current <see cref="SkiaTree"/>.
        /// </summary>
        protected void Render()
        {
            Source.Lock();

            T context = CreateDrawingContext();

            context.BeginDrawing();

            context.Clear(Colors.Transparent);
            SkiaTree.Root.Render(context, new Rect(0, 0, _host.ActualWidth, _host.ActualHeight), GetVirtualizedBounds(), 1);

            context.EndDrawing();

            OnRenderCompleted(Source.BackBuffer, (int)Source.Width, (int)Source.Height, (int)Source.Width * 4);

            Source.AddDirtyRect(new Int32Rect(0, 0, (int)Source.Width, (int)Source.Height));
            Source.Unlock();

            OnSourceChanged();
        }

        /// <summary>
        /// Performs a rendering pass on the specified <see cref="SkiaFrameworkElement"/> and its descendants.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="mode">The mode.</param>
        protected void RenderSingle(SkiaFrameworkElement element, BindingPropertyMode mode)
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

            if (bounds.Left < Source.Width && bounds.Top < Source.Height)
            {
                Source.Lock();

                T context = CreateDrawingContext();

                context.BeginDrawing();

                context.ClipRect(bounds, new CornerRadius());
                context.Clear(Colors.Transparent);

                SkiaTree.Root.Invalidate(context, bounds, 1);

                context.EndDrawing();

                if (bounds.Right > Source.Width)
                {
                    bounds.Width = Source.Width - bounds.Left;
                }

                if (bounds.Bottom > Source.Height)
                {
                    bounds.Height = Source.Height - bounds.Top;
                }

                OnRenderCompleted(Source.BackBuffer, (int)Source.Width, (int)Source.Height, (int)Source.Width * 4);

                Source.AddDirtyRect(new Int32Rect((int)Math.Max(bounds.Left, 0), (int)Math.Max(bounds.Top, 0), (int)bounds.Width, (int)bounds.Height));
                Source.Unlock();
            }
        }

        /// <summary>
        /// Gets the current virtualized bounds if in virtualization mode.
        /// </summary>
        /// <returns></returns>
        protected Rect GetVirtualizedBounds()
        {
            return IsVirtualizing ? new Rect((int)_scrollViewer.HorizontalOffset, (int)_scrollViewer.VerticalOffset, (int)_scrollViewer.ViewportWidth, (int)_scrollViewer.ViewportHeight) : new Rect(0, 0, _host.ActualWidth, _host.ActualHeight);
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Called when the surface has been created
        /// </summary>
        /// <param name="backBuffer">The back buffer.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stride">The stride.</param>
        protected abstract void OnSurfaceCreated(IntPtr backBuffer, int width, int height, int stride);

        /// <summary>
        /// Creates the drawing context.
        /// </summary>
        /// <returns></returns>
        protected abstract T CreateDrawingContext();

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the specified element and its descendants by hooking property bindings and events.
        /// </summary>
        /// <param name="element">The element.</param>
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

        /// <summary>
        /// Disposes the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        private void DisposeElement(SkiaFrameworkElement element)
        {
            element.ChildAdded -= Element_ChildAdded;
            element.ChildRemoved -= Element_ChildRemoved;
        }

        /// <summary>
        /// Initializes the surface.
        /// </summary>
        private void InitSurface()
        {
            double renderWidth = _host.ActualWidth;
            double renderHeight = _host.ActualHeight;

            if (_scrollViewer != null && IsVirtualizing)
            {
                renderWidth = _scrollViewer.ViewportWidth;
                renderHeight = _scrollViewer.ViewportHeight;
            }

            Source = new WriteableBitmap((int)Math.Max(renderWidth, 1), (int)Math.Max(renderHeight, 1), 96, 96, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);

            int width = (int)Source.Width;
            int height = (int)Source.Height;

            Source.Lock();

            OnSurfaceCreated(Source.BackBuffer, width, height, width * 4);

            Source.AddDirtyRect(new Int32Rect(0, 0, width, height));
            Source.Unlock();
        }

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Called when a render pass has been completed.
        /// </summary>
        /// <param name="backBuffer">The back buffer.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stride">The stride.</param>
        protected virtual void OnRenderCompleted(IntPtr backBuffer, int width, int height, int stride)
        {

        }

        /// <summary>
        /// Called when a binding container value has been changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="BindingEventArgs"/> instance containing the event data.</param>
        protected virtual void OnBindingContainerValueChanged(object sender, BindingEventArgs e)
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

        /// <summary>
        /// Called when the source bitmap has been changed.
        /// </summary>
        protected virtual void OnSourceChanged()
        {
            SourceChanged?.Invoke(this, Source);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Should be called when the <see cref="SkiaHost.Child" /> is completely loaded.
        /// </summary>
        /// <param name="host">The Skia host.</param>
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

            Render();
        }

        #endregion
    }
}
