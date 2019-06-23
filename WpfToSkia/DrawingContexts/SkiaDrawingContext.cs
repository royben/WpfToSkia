﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfToSkia.ExtensionsMethods;

namespace WpfToSkia.DrawingContexts
{
    public class SkiaDrawingContext : IDrawingContext
    {
        private SKCanvas _canvas;
        public SkiaDrawingContext(SKCanvas canvas)
        {
            _canvas = canvas;
        }

        public void BeginDrawing()
        {
            _canvas.Save();
        }

        public void Clear(Color color)
        {
            _canvas.Clear(color.ToSKColor());
        }

        public void DrawLine(Rect bounds, Point p1, Point p2, DrawingStyle style)
        {
            _canvas.ApplyTransform(style.Transform, style.TransformOrigin, bounds);

            SKPaint paint = new SKPaint();
            paint.ApplyStroke(bounds, style);

            _canvas.DrawLine(p1.ToSKPoint(), p2.ToSKPoint(), paint);

            _canvas.ResetMatrix();
        }

        public void DrawPolygon(Rect bounds, Point[] points, DrawingStyle style)
        {
            _canvas.ApplyTransform(style.Transform, style.TransformOrigin, bounds);

            var pp = points.Select(x => x.ToSKPoint()).ToArray();
            SKPath path = new SKPath();
            path.AddPoly(pp, true);
            path.Offset(bounds.Left.ToFloat(), bounds.Top.ToFloat());

            if (style.HasFill)
            {
                SKPaint paintFill = new SKPaint();
                paintFill.ApplyFill(bounds, style);
                _canvas.DrawPath(path, paintFill);
            }

            if (style.HasStroke)
            {
                SKPaint paintStroke = new SKPaint();
                paintStroke.ApplyStroke(bounds, style);
                _canvas.DrawPath(path, paintStroke);
            }

            _canvas.ResetMatrix();
        }

        public void DrawRect(Rect bounds, DrawingStyle style)
        {
            _canvas.ApplyTransform(style.Transform, style.TransformOrigin, bounds);

            bool isRounded = style.CornerRadius.TopLeft > 0;

            if (style.HasFill)
            {
                SKPaint paintFill = new SKPaint();
                paintFill.ApplyFill(bounds, style);

                if (isRounded)
                {
                    _canvas.DrawRoundRect(new SKRoundRect(bounds.ToSKRectStroke(style.StrokeThickness), style.CornerRadius.TopLeft.ToFloat(), style.CornerRadius.TopRight.ToFloat()), paintFill);
                }
                else
                {
                    _canvas.DrawRect(bounds.ToSKRectStroke(style.StrokeThickness), paintFill);
                }
            }

            if (style.HasStroke)
            {
                SKPaint paintStroke = new SKPaint();
                paintStroke.ApplyStroke(bounds, style);

                if (isRounded)
                {
                    _canvas.DrawRoundRect(new SKRoundRect(bounds.ToSKRectStroke(style.StrokeThickness), style.CornerRadius.TopLeft.ToFloat(), style.CornerRadius.TopRight.ToFloat()), paintStroke);
                }
                else
                {
                    _canvas.DrawRect(bounds.ToSKRectStroke(style.StrokeThickness), paintStroke);
                }
            }

            _canvas.ResetMatrix();
        }

        public void ClipRect(Rect bounds, CornerRadius cornerRadius)
        {
            if (cornerRadius.TopLeft > 0)
            {
                _canvas.ClipRoundRect(new SKRoundRect(bounds.ToSKRect(), cornerRadius.TopLeft.ToFloat(), cornerRadius.TopLeft.ToFloat()), SKClipOperation.Intersect, false);
            }
            else
            {
                _canvas.ClipRect(bounds.ToSKRect(), SKClipOperation.Intersect, false);
            }
        }

        public void DrawEllipse(Rect bounds, DrawingStyle style)
        {
            _canvas.ApplyTransform(style.Transform, style.TransformOrigin, bounds);

            if (style.HasFill)
            {
                SKPaint paintFill = new SKPaint();
                paintFill.ApplyFill(bounds, style);

                _canvas.DrawOval(bounds.ToSKRectStroke(style.StrokeThickness), paintFill);
            }

            if (style.HasStroke)
            {
                SKPaint paintStroke = new SKPaint();
                paintStroke.ApplyStroke(bounds, style);

                _canvas.DrawOval(bounds.ToSKRectStroke(style.StrokeThickness), paintStroke);
            }

            _canvas.ResetMatrix();
        }

        public void DrawText(Rect bounds, string text, DrawingStyle style)
        {
            _canvas.ApplyTransform(style.Transform, style.TransformOrigin, bounds);

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

            _canvas.DrawText(text, bounds.Left.ToFloat(), bounds.Bottom.ToFloat(), paint);

            _canvas.ResetMatrix();
        }

        public void DrawImage(Rect bounds, BitmapSource image, DrawingStyle style)
        {
            _canvas.ApplyTransform(style.Transform, style.TransformOrigin, bounds);

            SKPaint paint = new SKPaint();

            if (style.HasOpacity)
            {
                paint.ColorFilter = SKColorFilter.CreateBlendMode(SKColors.White.WithAlpha((byte)(style.Opacity * 255d)), SKBlendMode.DstIn);
            }

            _canvas.DrawBitmap(image.ToSKBitmap(), bounds.ToSKRect(), paint);

            _canvas.ResetMatrix();
        }

        public void DrawGeometry(Rect bounds, Geometry geometry, DrawingStyle style)
        {
            _canvas.ApplyTransform(style.Transform, style.TransformOrigin, bounds);

            PathGeometry g = geometry as PathGeometry;

            if (g == null)
            {
                g = (geometry as StreamGeometry).GetWidenedPathGeometry(new Pen(Brushes.Chartreuse, 2d));
            }

            SKPath skPath = g.ToSKPath();
            skPath.Offset(bounds.TopLeft.ToSKPoint());

            if (style.HasFill)
            {
                SKPaint paintFill = new SKPaint();
                paintFill.ApplyFill(bounds, style);

                _canvas.DrawPath(skPath, paintFill);
            }

            if (style.HasStroke)
            {
                SKPaint paintStroke = new SKPaint();
                paintStroke.ApplyStroke(bounds, style);

                _canvas.DrawPath(skPath, paintStroke);
            }

            _canvas.ResetMatrix();
        }

        public void EndDrawing()
        {
            _canvas.Restore();
        }
    }
}
