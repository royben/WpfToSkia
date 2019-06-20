using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace WpfToSkia.ExtensionsMethods
{
    public static class SKPaintExtensions
    {
        public static void ApplyFill(this SKPaint paint, Rect bounds, DrawingStyle style)
        {
            paint.Style = SKPaintStyle.Fill;
            paint.IsAntialias = style.EdgeMode == EdgeMode.Unspecified;

            if (style.Fill is SolidColorBrush)
            {
                paint.Color = style.Fill.ToSKColor();
            }
            else
            {
                paint.Shader = style.Fill.ToSkiaShader(bounds.Width, bounds.Height);
            }

            if (style.HasOpacity)
            {
                paint.ColorFilter = SKColorFilter.CreateBlendMode(SKColors.White.WithAlpha((byte)(style.Opacity * 255d)), SKBlendMode.DstIn);
            }

            if (style.Effect != null)
            {
                if (style.Effect is DropShadowEffect)
                {
                    var fx = style.Effect as DropShadowEffect;
                    paint.ImageFilter = SKImageFilter.CreateDropShadow(fx.ShadowDepth.ToFloat(), fx.ShadowDepth.ToFloat(), fx.BlurRadius.ToFloat(), fx.BlurRadius.ToFloat(), fx.Color.ToSKColor(), SKDropShadowImageFilterShadowMode.DrawShadowAndForeground);
                }
            }
        }

        public static void ApplyStroke(this SKPaint paint, Rect bounds, DrawingStyle style)
        {
            paint.IsAntialias = style.EdgeMode == EdgeMode.Unspecified;
            paint.Style = SKPaintStyle.Stroke;
            paint.IsStroke = true;
            paint.StrokeWidth = style.StrokeThickness.Left.ToFloat();

            if (style.Stroke is SolidColorBrush)
            {
                paint.Color = style.Stroke.ToSKColor();
            }
            else
            {
                paint.Shader = style.Stroke.ToSkiaShader(bounds.Width, bounds.Height);
            }

            if (style.HasOpacity)
            {
                paint.ColorFilter = SKColorFilter.CreateBlendMode(SKColors.White.WithAlpha((byte)(style.Opacity * 255d)), SKBlendMode.DstIn);
            }
        }
    }
}
