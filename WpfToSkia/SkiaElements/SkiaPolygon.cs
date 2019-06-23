using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace WpfToSkia.SkiaElements
{
    public class SkiaPolygon : SkiaFrameworkElement
    {
        protected override void OnRender(IDrawingContext context, Rect bounds, double opacity)
        {
            base.OnRender(context, bounds, opacity);

            Polygon polygon = WpfElement as Polygon;

            DrawingStyle style = DrawingStyle.FromElement(this);
            style.Fill = polygon.Fill;
            style.Stroke = polygon.Stroke;
            style.StrokeThickness = new Thickness(polygon.StrokeThickness);
            style.Opacity = opacity;

            context.DrawPolygon(bounds, polygon.Points.ToArray(), style);
        }
    }
}
