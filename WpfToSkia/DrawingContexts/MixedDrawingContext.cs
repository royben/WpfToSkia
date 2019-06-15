using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfToSkia.ExtensionsMethods;

namespace WpfToSkia.DrawingContexts
{
    public class MixedDrawingContext : IDrawingContext
    {
        private Graphics _g;
        private SKCanvas _canvas;

        public MixedDrawingContext(Graphics g, SKCanvas canvas)
        {
            _g = g;
            _canvas = canvas;
        }

        public void BeginDrawing()
        {
            _g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            _g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            _g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
        }

        public void Clear(System.Windows.Media.Color color)
        {
            _g.Clear(color.ToGdiColor());
        }

        public void DrawRect(Rect bounds, DrawingStyle style)
        {
            if (style.Fill != null)
            {
                var fill = style.Fill.ToGdiBrush(bounds.Width, bounds.Height);
                _g.FillRectangle(fill, bounds.Left.ToFloat(), bounds.Top.ToFloat(), bounds.Width.ToFloat(), bounds.Height.ToFloat());
                fill.Dispose();
            }

            if (style.Stroke != null)
            {
                Pen pen = new Pen(style.Stroke.ToGdiBrush(bounds.Width, bounds.Height), style.StrokeThickness.Left.ToFloat());
                _g.DrawRectangle(pen, bounds.Left.ToFloat(), bounds.Top.ToFloat(), bounds.Width.ToFloat(), bounds.Height.ToFloat());
                pen.Dispose();
            }
        }

        public void ClipRect(Rect bounds, CornerRadius cornerRadius)
        {
            _g.SetClip(bounds.ToGdiRectF());
        }

        public void DrawText(string text, Rect bounds, DrawingStyle style)
        {
            SKFontStyleSlant fontStyle = SKFontStyleSlant.Upright;

            if (style.FontStyle == FontStyles.Italic)
            {
                fontStyle = SKFontStyleSlant.Italic;
            }
            else if (style.FontStyle == FontStyles.Oblique)
            {
                fontStyle = SKFontStyleSlant.Oblique;
            }

            var typeFace = SKTypeface.FromFamilyName(style.FontFamily.ToString(), new SKFontStyle(style.FontWeight.ToOpenTypeWeight(), 1, fontStyle));

            SKPaint paint = new SKPaint();
            paint.Typeface = typeFace;
            paint.TextSize = style.FontSize.ToFloat();
            paint.IsAntialias = style.EdgeMode == System.Windows.Media.EdgeMode.Unspecified;

            if (style.HasOpacity)
            {
                paint.ColorFilter = SKColorFilter.CreateBlendMode(SKColors.White.WithAlpha((byte)(style.Opacity * 255d)), SKBlendMode.DstIn);
            }

            if (style.Fill != null)
            {
                if (style.Fill is System.Windows.Media.SolidColorBrush)
                {
                    paint.Color = style.Fill.ToSKColor();
                }
                else
                {
                    paint.Shader = style.Fill.ToSkiaShader(bounds.Width, bounds.Height);
                }
            }

            _canvas.DrawText(text, bounds.Left.ToFloat(), bounds.Bottom.ToFloat(), paint);

        }

        public void EndDrawing()
        {

        }
    }
}
