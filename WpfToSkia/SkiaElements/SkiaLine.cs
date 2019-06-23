using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace WpfToSkia.SkiaElements
{
    public class SkiaLine : SkiaFrameworkElement
    {
        protected override void OnRender(IDrawingContext context, Rect bounds, double opacity)
        {
            base.OnRender(context, bounds, opacity);

            Line line = WpfElement as Line;

            DrawingStyle style = DrawingStyle.FromElement(this);
            style.Stroke = line.Stroke;
            style.StrokeThickness = new Thickness(line.StrokeThickness);
            style.Opacity = opacity;

            context.DrawLine(bounds, new Point(line.X1, line.Y1), new Point(line.X2, line.Y2), style);
        }
    }
}
