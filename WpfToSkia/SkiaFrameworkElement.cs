using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfToSkia
{
    public class SkiaFrameworkElement
    {
        public Rect Bounds { get; private set; }
        public FrameworkElement WpfElement { get; set; }
        public List<SkiaFrameworkElement> Children { get; set; }

        public SkiaFrameworkElement()
        {
            Children = new List<SkiaFrameworkElement>();
        }

        public virtual void Render(SKCanvas canvas, Rect bounds, double opacity = 1)
        {
            Bounds = bounds;
        }

        public virtual void Invalidate(WriteableBitmap bitmap)
        {
            int width = (int)bitmap.Width;
            int height = (int)bitmap.Height;

            bitmap.Lock();

            using (var surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul), bitmap.BackBuffer, width * 4))
            {
                SKCanvas canvas = surface.Canvas;
                canvas.Clear(new SKColor(0, 0, 0, 0));
                Render(canvas, Bounds);
            }

            bitmap.AddDirtyRect(new Int32Rect((int)Bounds.Left, (int)Bounds.Top, (int)Bounds.Width, (int)Bounds.Height));
            bitmap.Unlock();
        }

        public virtual Size Measure(Size availableSize)
        {
            return availableSize;
        }

        public virtual List<BindingProperty> GetBindingProperties()
        {
            return new List<BindingProperty>()
            {
                 new BindingProperty(FrameworkElement.OpacityProperty,BindingPropertyMode.AffectsRender),
            };
        }
    }
}
