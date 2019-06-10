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

namespace WpfToSkia
{
    [ContentProperty(nameof(Child))]
    public class SkiaHost : FrameworkElement
    {
        private WriteableBitmap _bitmap;
        private Image _image;
        private SkiaTree _tree;

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
            AddVisualChild(_image);
            AddLogicalChild(_image);

            SkiaElementResolver.Default.RegisterBinder<Border, SkiaBorder>();
            SkiaElementResolver.Default.RegisterBinder<StackPanel, SkiaStackPanel>();
            SkiaElementResolver.Default.RegisterBinder<TextBlock, SkiaTextBlock>();

            Loaded += SkiaHost_Loaded;
        }

        private void SkiaHost_Loaded(object sender, RoutedEventArgs e)
        {
            InitCanvas();
            _tree = WpfTreeHelper.LoadTree(Child);
            Invalidate();
        }

        private void InitCanvas()
        {
            _bitmap = new WriteableBitmap((int)ActualWidth, (int)ActualHeight, 96, 96, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);
        }

        public void Invalidate()
        {
            int width = (int)_bitmap.Width;
            int height = (int)_bitmap.Height;

            _bitmap.Lock();

            using (var surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul), _bitmap.BackBuffer, width * 4))
            {
                SKCanvas canvas = surface.Canvas;
                canvas.Clear(new SKColor(0, 0, 0, 0));
                //_tree.Root.Measure(new Size(ActualWidth, ActualHeight));
                _tree.Root.Render(canvas, new Rect(0, 0, ActualWidth, ActualHeight));
            }

            _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            _bitmap.Unlock();

            _image.Source = _bitmap;
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
            return _image;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _image.Measure(availableSize);
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _image.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
            return finalSize;
        }
    }
}
