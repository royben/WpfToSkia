using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfToSkia.SkiaElements
{
    public class SkiaImage : SkiaFrameworkElement
    {
        protected override void OnRender(IDrawingContext context, Rect bounds, double opacity)
        {
            base.OnRender(context, bounds, opacity);

            Image image = WpfElement as Image;

            DrawingStyle style = DrawingStyle.FromElement(this);
            style.Opacity = opacity;

            context.DrawImage(bounds, image.Source as BitmapSource, style);
        }
    }
}
