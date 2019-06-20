using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfToSkia.SkiaElements;
using WpfToSkia.ExtensionsMethods;
using System.Windows.Input;
using System.Diagnostics;
using SkiaSharp.Tests;
using WpfToSkia.DrawingContexts;
using WpfToSkia.Renderers;
using System.Windows.Shapes;

namespace WpfToSkia
{
    [ContentProperty(nameof(Child))]
    public class SkiaHost : FrameworkElement
    {
        private WriteableBitmap _bitmap;
        private Image _image;
        private bool _host_loaded;
        private ScrollViewer _scrollViewer;

        public FrameworkElement Child
        {
            get { return (FrameworkElement)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register("Child", typeof(FrameworkElement), typeof(SkiaHost), new PropertyMetadata(null));

        public IRenderer Renderer
        {
            get { return (IRenderer)GetValue(RendererProperty); }
            set { SetValue(RendererProperty, value); }
        }
        public static readonly DependencyProperty RendererProperty =
            DependencyProperty.Register("Renderer", typeof(IRenderer), typeof(SkiaHost), new PropertyMetadata(null));

        public SkiaHost()
        {
            Focusable = false;
            FocusVisualStyle = null;

            Renderer = new SkiaRenderer();

            _image = new Image() { Stretch = Stretch.None, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };

            if (!this.DesignMode())
            {
                AddVisualChild(_image);
                AddLogicalChild(_image);

                SkiaElementResolver.Default.Clear();
                SkiaElementResolver.Default.RegisterBinder<Border, SkiaBorder>();
                SkiaElementResolver.Default.RegisterBinder<StackPanel, SkiaStackPanel>();
                SkiaElementResolver.Default.RegisterBinder<TextBlock, SkiaTextBlock>();
                SkiaElementResolver.Default.RegisterBinder<ItemsControl, SkiaItemsControl>();
                SkiaElementResolver.Default.RegisterBinder<Rectangle, SkiaRectangle>();
                SkiaElementResolver.Default.RegisterBinder<ScrollViewer, SkiaScrollViewer>();
                SkiaElementResolver.Default.RegisterBinder<Ellipse, SkiaEllipse>();
            }

            Loaded += SkiaHost_Loaded;
        }

        private void SkiaHost_Loaded(object sender, RoutedEventArgs e)
        {
            if (_host_loaded || this.ActualWidth == 0 || this.ActualHeight == 0) return;

            if (this.DesignMode())
            {
                AddVisualChild(Child);
                AddLogicalChild(Child);
            }
            else
            {
                _scrollViewer = this.FindAncestor<ScrollViewer>();
                Child.Opacity = 0;
                AddVisualChild(Child);
                AddLogicalChild(Child);

                Child.Loaded += (_, __) =>
                {
                    Renderer.Init(this);
                    Renderer.SourceChanged += (___, ____) => 
                    {
                        _image.Source = Renderer.Source;
                    };
                    _image.Source = Renderer.Source;
                    _host_loaded = true;

                    if (_scrollViewer != null)
                    {
                        _scrollViewer.ScrollChanged += (x, xx) =>
                        {
                            if (Renderer.IsVirtualizing)
                            {
                                _image.Margin = new Thickness(_scrollViewer.HorizontalOffset, _scrollViewer.VerticalOffset, 0, 0);
                            }
                        };
                    }
                };

                _host_loaded = true;
            }
        }

        private void InvalidatePartial(Rect bounds)
        {
            //int width = (int)_bitmap.Width;
            //int height = (int)_bitmap.Height;

            //_bitmap.Lock();

            //using (var surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul), _bitmap.BackBuffer, width * 4))
            //{
            //    SKCanvas canvas = surface.Canvas;
            //    canvas.Clear(new SKColor(0, 0, 0, 0));
            //    _tree.Root.Render(new RenderPackage()
            //    {
            //        Canvas = canvas,
            //    });
            //}

            //_bitmap.AddDirtyRect(new Int32Rect((int)bounds.Left, (int)bounds.Top, (int)bounds.Width + 1, (int)bounds.Height + 1));
            //_bitmap.Unlock();
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return this.DesignMode() ? 1 : 2;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (this.DesignMode())
            {
                return Child;
            }
            else
            {
                if (index == 0)
                {
                    return _image;
                }
                else
                {
                    return Child;
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.DesignMode())
            {
                return Child.DesiredSize;
            }
            else
            {
                Child.Measure(availableSize);
                _image.Measure(availableSize);
            }
            return Child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.DesignMode())
            {
                Child.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
            }
            else
            {
                Child.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
                _image.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
            }

            return finalSize;
        }
    }
}
