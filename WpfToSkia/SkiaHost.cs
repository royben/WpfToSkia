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

namespace WpfToSkia
{
    [ContentProperty(nameof(Child))]
    public class SkiaHost : FrameworkElement
    {
        private WriteableBitmap _bitmap;
        private Image _image;
        private SkiaTree _tree;
        private List<BindingEventContainer> _containers;
        private SKSurface _surface;
        private FrameworkElement _mouseEnterElement;
        private bool _loaded;

        public FrameworkElement Child
        {
            get { return (FrameworkElement)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register("Child", typeof(FrameworkElement), typeof(SkiaHost), new PropertyMetadata(null));

        public SkiaHost()
        {
            _image = new Image() { Stretch = Stretch.Fill };

            if (!this.DesignMode())
            {
                _containers = new List<BindingEventContainer>();

                AddVisualChild(_image);
                AddLogicalChild(_image);

                SkiaElementResolver.Default.Clear();
                SkiaElementResolver.Default.RegisterBinder<Border, SkiaBorder>();
                SkiaElementResolver.Default.RegisterBinder<StackPanel, SkiaStackPanel>();
                SkiaElementResolver.Default.RegisterBinder<TextBlock, SkiaTextBlock>();

                this.SizeChanged += SkiaHost_SizeChanged;
            }

            Loaded += SkiaHost_Loaded;
        }

        private void SkiaHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!this.DesignMode() && _loaded)
            {
                InitCanvas();
                Invalidate();
            }
        }

        private void SkiaHost_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DesignMode())
            {
                AddVisualChild(Child);
                AddLogicalChild(Child);
            }
            else
            {
                InitCanvas();
                _tree = WpfTreeHelper.LoadTree(Child);

                foreach (var element in _tree.Flatten())
                {
                    foreach (var dp in element.GetBindingProperties())
                    {
                        var container = BindingEventContainer.Generate(element, element.WpfElement, dp.DependencyProperty);
                        container.ValueChanged += (x, ee) =>
                        {
                            InvalidatePartial(element.Bounds);
                        };

                        _containers.Add(container);
                    }
                }

                Invalidate();

                _loaded = true;
            }
        }

        private void InitCanvas()
        {
            _bitmap = new WriteableBitmap((int)ActualWidth, (int)ActualHeight, 96, 96, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);

            int width = (int)_bitmap.Width;
            int height = (int)_bitmap.Height;

            _bitmap.Lock();

            _surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul), _bitmap.BackBuffer, width * 4);

            _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            _bitmap.Unlock();

            _image.Source = _bitmap;
        }

        public void Invalidate()
        {
            _bitmap.Lock();

            SKCanvas canvas = _surface.Canvas;
            canvas.Clear(new SKColor(0, 0, 0, 0));
            _tree.Root.Render(canvas, new Rect(0, 0, ActualWidth, ActualHeight));

            _bitmap.AddDirtyRect(new Int32Rect(0, 0, (int)_bitmap.Width, (int)_bitmap.Height));
            _bitmap.Unlock();
        }

        private void InvalidatePartial(Rect bounds)
        {
            int width = (int)_bitmap.Width;
            int height = (int)_bitmap.Height;

            _bitmap.Lock();

            using (var surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul), _bitmap.BackBuffer, width * 4))
            {
                SKCanvas canvas = surface.Canvas;
                canvas.Clear(new SKColor(0, 0, 0, 0));
                _tree.Root.Render(canvas, new Rect(0, 0, ActualWidth, ActualHeight));
            }

            _bitmap.AddDirtyRect(new Int32Rect((int)bounds.Left, (int)bounds.Top, (int)bounds.Width + 1, (int)bounds.Height + 1));
            _bitmap.Unlock();
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.DesignMode() ? Child : _image;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.DesignMode())
            {
                Child.Measure(availableSize);
            }
            else
            {
                _image.Measure(availableSize);
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.DesignMode())
            {
                Child.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
            }
            else
            {
                _image.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
            }

            return finalSize;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Rect mouseRect = new Rect(e.GetPosition(this), new Size(1, 1));

            var item = _tree.Flatten().LastOrDefault(x => x.Bounds.IntersectsWith(mouseRect));

            if (item != null)
            {
                if (item.Bounds.IntersectsWith(mouseRect))
                {
                    if (_mouseEnterElement != item.WpfElement)
                    {
                        if (_mouseEnterElement != null)
                        {
                            _mouseEnterElement.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice, 0)
                            {
                                RoutedEvent = Mouse.MouseLeaveEvent,
                                Source = _mouseEnterElement,
                            });
                        }

                        _mouseEnterElement = item.WpfElement;

                        item.WpfElement.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice, 0)
                        {
                            RoutedEvent = Mouse.MouseEnterEvent,
                            Source = item.WpfElement,
                        });
                    }
                }
            }
            else
            {
                if (_mouseEnterElement != null)
                {
                    _mouseEnterElement.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice, 0)
                    {
                        RoutedEvent = Mouse.MouseLeaveEvent,
                        Source = _mouseEnterElement,
                    });
                }
            }
        }
    }
}
