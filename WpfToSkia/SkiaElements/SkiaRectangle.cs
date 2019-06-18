using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfToSkia.SkiaElements
{
    public class SkiaRectangle : SkiaFrameworkElement
    {
        protected override void OnRender(IDrawingContext context, Rect bounds, double opacity)
        {
            base.OnRender(context, bounds, opacity);

            Rectangle rectangle = WpfElement as Rectangle;

            DrawingStyle style = new DrawingStyle();
            style.Fill = rectangle.Fill;
            style.Stroke = rectangle.Stroke;
            style.StrokeThickness = new Thickness(rectangle.StrokeThickness);
            style.Opacity = opacity;
            style.EdgeMode = RenderOptions.GetEdgeMode(rectangle);
            style.Effect = rectangle.Effect;

            context.DrawRect(bounds, style);
        }

        public override List<BindingProperty> GetBindingProperties()
        {
            var props = base.GetBindingProperties();
            props.Add(new BindingProperty(Rectangle.FillProperty, BindingPropertyMode.AffectsRender));
            return props;
        }
    }
}
