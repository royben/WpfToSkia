using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfToSkia.ExtensionsMethods;

namespace WpfToSkia.DrawingContexts
{
    public class GdiDrawingContext : IDrawingContext
    {
        private Graphics _g;

        public GdiDrawingContext(Graphics g)
        {
            _g = g;
        }

        public void BeginDrawing()
        {
            _g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            _g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            _g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            //_g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            //_g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
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
                System.Drawing.Pen pen = new System.Drawing.Pen(style.Stroke.ToGdiBrush(bounds.Width, bounds.Height), style.StrokeThickness.Left.ToFloat());
                _g.DrawRectangle(pen, bounds.Left.ToFloat(), bounds.Top.ToFloat(), bounds.Width.ToFloat(), bounds.Height.ToFloat());
                pen.Dispose();
            }
        }

        public void ClipRect(Rect bounds, CornerRadius cornerRadius)
        {
            _g.SetClip(bounds.ToGdiRectF());
        }

        public void DrawText(Rect bounds, string text, DrawingStyle style)
        {
            if (style.Fill != null)
            {
                var fill = style.Fill.ToGdiBrush(bounds.Width, bounds.Height);
                _g.DrawString(text, System.Drawing.SystemFonts.DefaultFont, fill, bounds.ToGdiRectF());
                fill.Dispose();
            }
        }

        public void DrawEllipse(Rect bounds, DrawingStyle style)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Rect bounds, BitmapSource image, DrawingStyle style)
        {
            throw new NotImplementedException();
        }

        public void DrawGeometry(Rect bounds, Geometry geometry, DrawingStyle style)
        {
            throw new NotImplementedException();
        }

        public void DrawLine(Rect bounds, System.Windows.Point p1, System.Windows.Point p2, DrawingStyle style)
        {
            throw new NotImplementedException();
        }

        public void DrawPolygon(Rect bounds, System.Windows.Point[] points, DrawingStyle style)
        {
            throw new NotImplementedException();
        }

        public void EndDrawing()
        {

        }
    }
}
