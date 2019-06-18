using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using WpfToSkia.ExtensionsMethods;

namespace WpfToSkia.DrawingContexts
{
    public class SkiaDrawingContext : IDrawingContext
    {
        private SKCanvas _canvas;
        private List<Action> _drawings;

        public SkiaDrawingContext(SKCanvas canvas)
        {
            _canvas = canvas;
        }

        public void BeginDrawing()
        {
            _drawings = new List<Action>();
            _canvas.Save();
        }

        public void Clear(Color color)
        {
            AddDrawingAction(() =>
            {
                _canvas.Clear(color.ToSKColor());
            });
        }

        public void DrawRect(Rect bounds, DrawingStyle style)
        {
            bool isRounded = style.CornerRadius.TopLeft > 0;

            if (style.Fill != null)
            {
                SKPaint paintFill = new SKPaint();
                paintFill.Style = SKPaintStyle.Fill;
                paintFill.IsAntialias = style.EdgeMode == EdgeMode.Unspecified;

                if (style.Fill is SolidColorBrush)
                {
                    paintFill.Color = style.Fill.ToSKColor();
                }
                else
                {
                    paintFill.Shader = style.Fill.ToSkiaShader(bounds.Width, bounds.Height);
                }

                if (style.HasOpacity)
                {
                    paintFill.ColorFilter = SKColorFilter.CreateBlendMode(SKColors.White.WithAlpha((byte)(style.Opacity * 255d)), SKBlendMode.DstIn);
                }

                if (style.Effect != null)
                {
                    if (style.Effect is DropShadowEffect)
                    {
                        var fx = style.Effect as DropShadowEffect;
                        paintFill.ImageFilter = SKImageFilter.CreateDropShadow(fx.ShadowDepth.ToFloat(), fx.ShadowDepth.ToFloat(), fx.BlurRadius.ToFloat(), fx.BlurRadius.ToFloat(), fx.Color.ToSKColor(), SKDropShadowImageFilterShadowMode.DrawShadowAndForeground);
                    }
                }

                AddDrawingAction(() =>
                {
                    if (isRounded)
                    {
                        _canvas.DrawRoundRect(new SKRoundRect(bounds.ToSKRectStroke(style.StrokeThickness, 0, 0), style.CornerRadius.TopLeft.ToFloat(), style.CornerRadius.TopRight.ToFloat()), paintFill);
                    }
                    else
                    {
                        _canvas.DrawRect(bounds.ToSKRectStroke(style.StrokeThickness, 0, 0), paintFill);
                    }
                });
            }

            if (style.Stroke != null && style.StrokeThickness.Left > 0)
            {
                SKPaint paintStroke = new SKPaint();
                paintStroke.IsAntialias = style.EdgeMode == EdgeMode.Unspecified;
                paintStroke.Style = SKPaintStyle.Stroke;
                paintStroke.IsStroke = true;
                paintStroke.StrokeWidth = style.StrokeThickness.Left.ToFloat();

                if (style.Stroke is SolidColorBrush)
                {
                    paintStroke.Color = style.Stroke.ToSKColor();
                }
                else
                {
                    paintStroke.Shader = style.Stroke.ToSkiaShader(bounds.Width, bounds.Height);
                }

                if (style.HasOpacity)
                {
                    paintStroke.ColorFilter = SKColorFilter.CreateBlendMode(SKColors.White.WithAlpha((byte)(style.Opacity * 255d)), SKBlendMode.DstIn);
                }

                AddDrawingAction(() =>
                {
                    if (isRounded)
                    {
                        _canvas.DrawRoundRect(new SKRoundRect(bounds.ToSKRectStroke(style.StrokeThickness, 0, 0), style.CornerRadius.TopLeft.ToFloat(), style.CornerRadius.TopRight.ToFloat()), paintStroke);
                    }
                    else
                    {
                        _canvas.DrawRect(bounds.ToSKRectStroke(style.StrokeThickness, 0, 0), paintStroke);
                    }
                });
            }
        }

        public void ClipRect(Rect bounds, CornerRadius cornerRadius)
        {
            AddDrawingAction(() =>
            {
                if (cornerRadius.TopLeft > 0)
                {
                    _canvas.ClipRoundRect(new SKRoundRect(bounds.ToSKRect(), cornerRadius.TopLeft.ToFloat(), cornerRadius.TopLeft.ToFloat()), SKClipOperation.Intersect, false);
                }
                else
                {
                    _canvas.ClipRect(bounds.ToSKRect(), SKClipOperation.Intersect, false);
                }
            });
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
            paint.IsAntialias = style.EdgeMode == EdgeMode.Unspecified;

            if (style.HasOpacity)
            {
                paint.ColorFilter = SKColorFilter.CreateBlendMode(SKColors.White.WithAlpha((byte)(style.Opacity * 255d)), SKBlendMode.DstIn);
            }

            if (style.Fill != null)
            {
                if (style.Fill is SolidColorBrush)
                {
                    paint.Color = style.Fill.ToSKColor();
                }
                else
                {
                    paint.Shader = style.Fill.ToSkiaShader(bounds.Width, bounds.Height);
                }
            }

            AddDrawingAction(() =>
            {
                _canvas.DrawText(text, bounds.Left.ToFloat(), bounds.Bottom.ToFloat(), paint);
            });
        }

        public void EndDrawing()
        {
            foreach (var drawing in _drawings)
            {
                drawing();
            }

            _drawings.Clear();

            _canvas.Restore();
        }

        private void AddDrawingAction(Action drawingAction)
        {
            _drawings.Add(drawingAction);
        }
    }
}
