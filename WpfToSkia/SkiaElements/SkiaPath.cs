using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace WpfToSkia.SkiaElements
{
    public class SkiaPath : SkiaFrameworkElement
    {
        protected override void OnRender(IDrawingContext context, Rect bounds, double opacity)
        {
            base.OnRender(context, bounds, opacity);

            Path path = WpfElement as Path;

            DrawingStyle style = DrawingStyle.FromElement(this);
            style.Fill = path.Fill;
            style.Stroke = path.Stroke;
            style.StrokeThickness = new Thickness(path.StrokeThickness);
            style.Opacity = opacity;

            context.DrawGeometry(bounds, path.Data, style);
        }
    }
}
