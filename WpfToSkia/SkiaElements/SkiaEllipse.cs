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
    public class SkiaEllipse : SkiaFrameworkElement
    {
        protected override void OnRender(IDrawingContext context, Rect bounds, double opacity)
        {
            base.OnRender(context, bounds, opacity);

            Ellipse ellipse = WpfElement as Ellipse;

            DrawingStyle style = DrawingStyle.FromElement(this);
            style.Fill = ellipse.Fill;
            style.Stroke = ellipse.Stroke;
            style.StrokeThickness = new Thickness(ellipse.StrokeThickness);
            style.Opacity = opacity;

            context.DrawEllipse(bounds, style);
        }

        public override List<BindingProperty> GetBindingProperties()
        {
            var props = base.GetBindingProperties();
            props.Add(new BindingProperty(Ellipse.FillProperty, BindingPropertyMode.AffectsRender));
            props.Add(new BindingProperty(Ellipse.StrokeProperty, BindingPropertyMode.AffectsRender));
            props.Add(new BindingProperty(Ellipse.StrokeThicknessProperty, BindingPropertyMode.AffectsRender));
            return props;
        }
    }
}
