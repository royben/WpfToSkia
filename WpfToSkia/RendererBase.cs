using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public event EventHandler<WriteableBitmap> SourceChanged;

        public WriteableBitmap Source
        {
            get { return _bitmap; }
        }

        public bool IsVirtualizing
        {
            get { return _host.ActualWidth * _host.ActualHeight > MaximumBitmapSize; }
        }

        public int MaximumBitmapSize { get; set; }

        public RendererBase()
        {
            _renderThreadQueue = new ProducerConsumerQueue<RenderQueueItem>();
            MaximumBitmapSize = 4000 * 4000;
        }

        public void Init(SkiaHost host)
        {
            _host = host;
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
                InitSurface();
            }
        }

        private void _host_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InitSurface();
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
            if (IsVirtualizing)
            {
                Render();
            }
        }

        protected void ReloadVisualTree()
        {
            _tree = WpfTreeHelper.LoadTree(_host.Child);
            _containers = new List<BindingEventContainer>();

            foreach (var element in _tree.Flatten())
            {
                foreach (var bindingProperty in element.GetBindingProperties())
                {
                    var container = BindingEventContainer.Generate(element, bindingProperty);
                    container.ValueChanged += (x, ee) =>
                    {
                        if (ee.BindingProperty.Mode == BindingPropertyMode.AffectsRender)
                        {
                            //InvalidatePartial(element.Bounds);
                        }
                        else
                        {
                            //_tree.InvalidateBounds();
                            //Invalidate();
                        }
                    };

                    _containers.Add(container);
                }
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

            _bitmap = new WriteableBitmap((int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);

            int width = (int)_bitmap.Width;
            int height = (int)_bitmap.Height;

            _bitmap.Lock();

            OnSurfaceCreated(_bitmap.BackBuffer, width, height, width * 4);

            _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            _bitmap.Unlock();

            OnSourceChanged();
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
            return IsVirtualizing ? new Rect(_scrollViewer.HorizontalOffset, _scrollViewer.VerticalOffset, _scrollViewer.ViewportWidth, _scrollViewer.ViewportHeight) : new Rect(0, 0, _host.ActualWidth, _host.ActualHeight);
        }

        protected virtual void OnSourceChanged()
        {
            SourceChanged?.Invoke(this, _bitmap);
        }
    }
}
