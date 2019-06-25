using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfToSkia.PDF.ExtensionMethods;

namespace WpfToSkia.PDF
{
    public class PdfDrawingContext : IDrawingContext
    {
        private XGraphics _g;

        public event EventHandler<Rect> Rendering;

        public PdfDrawingContext(XGraphics g)
        {
            _g = g;
        }

        public void Reset(XGraphics g)
        {
            _g = g;
        }

        public void BeginDrawing()
        {

        }

        public void Clear(Color color)
        {

        }

        public void DrawRect(Rect bounds, DrawingStyle style)
        {
            OnRendering(bounds);

            if (style.HasFill)
            {
                if (style.CornerRadius.TopLeft > 0)
                {
                    _g.DrawRoundedRectangle(style.Fill.ToXBrush(bounds.Width, bounds.Height), bounds.ToXRect(), new XSize(style.CornerRadius.TopLeft, style.CornerRadius.TopLeft));
                }
                else
                {
                    _g.DrawRectangle(style.Fill.ToXBrush(bounds.Width, bounds.Height), bounds.ToXRect());
                }
            }

            if (style.HasStroke)
            {
                if (style.CornerRadius.TopLeft > 0)
                {
                    _g.DrawRoundedRectangle(new XPen(style.Stroke.ToXColor(), style.StrokeThickness.Left), bounds.ToXRect(), new XSize(style.CornerRadius.TopLeft, style.CornerRadius.TopLeft));
                }
                else
                {
                    _g.DrawRectangle(new XPen(style.Stroke.ToXColor(), style.StrokeThickness.Left), bounds.ToXRect());
                }
            }
        }

        public void ClipRect(Rect bounds, CornerRadius cornerRadius)
        {

        }

        public void DrawEllipse(Rect bounds, DrawingStyle style)
        {
            OnRendering(bounds);

            if (style.HasFill)
            {
                _g.DrawEllipse(style.Fill.ToXBrush(bounds.Width, bounds.Height), bounds.ToXRect());
            }

            if (style.HasStroke)
            {
                _g.DrawEllipse(new XPen(style.Stroke.ToXColor(), style.StrokeThickness.Left), bounds.ToXRect());
            }
        }

        public void DrawText(Rect bounds, string text, DrawingStyle style)
        {
            OnRendering(bounds);

            XFontStyle fontStyle = XFontStyle.Regular;

            if (style.FontStyle == FontStyles.Italic)
            {
                fontStyle = XFontStyle.Italic;

                if (style.FontWeight == FontWeights.Bold)
                {
                    fontStyle = XFontStyle.BoldItalic;
                }
            }
            else if (style.FontWeight == FontWeights.Bold)
            {
                fontStyle = XFontStyle.BoldItalic;
            }

            XFont font = new XFont(style.FontFamily.ToString(), style.FontSize, fontStyle);

            _g.DrawString(text, font, style.Fill.ToXBrush(bounds.Width, bounds.Height), bounds.ToXRect(), new XStringFormat());
        }

        public void DrawImage(Rect bounds, BitmapSource image, DrawingStyle style)
        {
            OnRendering(bounds);

            _g.DrawImage(XImage.FromBitmapSource(image), bounds.ToXRect());
        }

        public void DrawGeometry(Rect bounds, Geometry geometry, DrawingStyle style)
        {

        }

        public void DrawLine(Rect bounds, Point p1, Point p2, DrawingStyle style)
        {
            OnRendering(bounds);

            _g.DrawLine(new XPen(style.Stroke.ToXColor()), p1.ToXPoint(), p2.ToXPoint());
        }

        public void DrawPolygon(Rect bounds, Point[] points, DrawingStyle style)
        {
            OnRendering(bounds);

            _g.TranslateTransform(bounds.Left, bounds.Top);

            if (style.HasFill)
            {
                _g.DrawPolygon(style.Fill.ToXBrush(bounds.Width, bounds.Height), points, XFillMode.Alternate);
            }

            if (style.HasStroke)
            {
                _g.DrawPolygon(new XPen(style.Stroke.ToXColor()), points);
            }

            _g.Restore();
        }

        public void EndDrawing()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnRendering(Rect bounds)
        {
            Rendering?.Invoke(this, bounds);
        }
    }
}
