using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfToSkia.ExtensionsMethods
{
    public static class BrushExtensions
    {
        public static SKShader ToSkiaShader(this Brush brush, double width, double height)
        {
            if (brush == null)
            {
                return null;
            }
            else if (brush is SolidColorBrush)
            {
                return SKShader.CreateColor((brush as SolidColorBrush).Color.ToSKColor());
            }
            else if (brush is LinearGradientBrush)
            {
                var b = brush as LinearGradientBrush;

                return SKShader.CreateLinearGradient(
                    new SKPoint((float)(b.StartPoint.X * width), (float)(b.StartPoint.Y * height)),
                    new SKPoint((float)(b.EndPoint.X * width), (float)(b.EndPoint.Y * height)),
                    b.GradientStops.Select(x => x.Color.ToSKColor()).ToArray(),
                    b.GradientStops.Select(x => (float)x.Offset).ToArray(),
                    SKShaderTileMode.Clamp);
            }
            else if (brush is RadialGradientBrush)
            {
                var b = brush as RadialGradientBrush;

                return SKShader.CreateRadialGradient(
                    new SKPoint((float)(b.Center.X * width), (float)(b.Center.Y * height)),
                    (float)b.RadiusX,
                    b.GradientStops.Select(x => x.Color.ToSKColor()).ToArray(),
                    b.GradientStops.Select(x => (float)x.Offset).ToArray(),
                    SKShaderTileMode.Clamp);
            }
            else
            {
                throw new NotSupportedException($"Unsupported brush type {brush.GetType().Name}.");
            }
        }

        public static SKColor ToSKColor(this Brush brush)
        {
            if (brush == null)
            {
                return SKColors.Transparent;
            }
            else if (brush is SolidColorBrush)
            {
                return (brush as SolidColorBrush).Color.ToSKColor();
            }
            else
            {
                throw new NotSupportedException($"Unsupported brush type {brush.GetType().Name}.");
            }
        }
    }
}
