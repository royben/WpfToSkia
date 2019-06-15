using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SkiaSharp;
using WpfToSkia.ExtensionsMethods;

namespace WpfToSkia.SkiaElements
{
    public class SkiaBorder : SkiaFrameworkElement
    {
        protected override void OnRender(IDrawingContext context, Rect bounds, double opacity)
        {
            base.OnRender(context, bounds, opacity);

            Border border = WpfElement as Border;

            DrawingStyle style = new DrawingStyle();
            style.CornerRadius = border.CornerRadius;
            style.Fill = border.Background;
            style.Stroke = border.BorderBrush;
            style.StrokeThickness = border.BorderThickness;
            style.Opacity = opacity;
            style.EdgeMode = RenderOptions.GetEdgeMode(border);
            style.Effect = border.Effect;

            context.DrawRect(bounds, style);

            if (border.ClipToBounds)
            {
                context.ClipRect(bounds, border.CornerRadius);
            }
        }
    }
}
