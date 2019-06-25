using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfToSkia.ExtensionMethods
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

        public static System.Drawing.Brush ToGdiBrush(this Brush brush, double width, double height)
        {
            if (brush == null)
            {
                return null;
            }
            else if (brush is SolidColorBrush)
            {
                return new System.Drawing.SolidBrush((brush as SolidColorBrush).Color.ToGdiColor());
            }
            else if (brush is LinearGradientBrush)
            {
                var b = brush as LinearGradientBrush;

                double angle = Math.Atan2(b.EndPoint.Y - b.StartPoint.Y, b.EndPoint.X - b.StartPoint.X) * 180 / Math.PI;

                System.Drawing.Drawing2D.LinearGradientBrush gradient = new System.Drawing.Drawing2D.LinearGradientBrush(new System.Drawing.Rectangle(0, 0, width.ToInt32(), height.ToInt32()), System.Drawing.Color.Black, System.Drawing.Color.Black, (float)angle);

                System.Drawing.Drawing2D.ColorBlend blend = new System.Drawing.Drawing2D.ColorBlend();

                List<System.Drawing.Color> colors = new List<System.Drawing.Color>();
                List<float> offsets = new List<float>();

                foreach (var stop in b.GradientStops)
                {
                    colors.Add(stop.Color.ToGdiColor());
                    offsets.Add((float)stop.Offset);
                }

                blend.Colors = colors.ToArray();
                blend.Positions = offsets.ToArray();

                gradient.InterpolationColors = blend;

                return gradient;
            }
            else
            {
                throw new NotSupportedException($"Unsupported brush type {brush.GetType().Name}.");
            }
        }
    }
}
