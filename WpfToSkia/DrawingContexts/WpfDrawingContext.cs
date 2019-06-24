using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfToSkia.DrawingContexts
{
    public class WpfDrawingContext : IDrawingContext
    {
        private DrawingContext _context;
        private int width;
        private int height;
        private bool _hasClip;
        private Rect _clipBounds;

        public WpfDrawingContext(DrawingContext context, int width, int height)
        {
            _context = context;
            this.width = width;
            this.height = height;
        }

        public void BeginDrawing()
        {

        }

        public void Clear(Color color)
        {
            _context.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));
        }

        public void DrawRect(Rect bounds, DrawingStyle style)
        {
            _context.PushOpacity(style.Opacity);
            _context.DrawRectangle(style.Fill, new Pen(style.Stroke, style.StrokeThickness.Left), bounds);
            _context.Pop();

            if (_hasClip)
            {
                ClipRect(_clipBounds, new CornerRadius());
            }
        }

        public void ClipRect(Rect bounds, CornerRadius cornerRadius)
        {
            _hasClip = true;
            _clipBounds = bounds;
            _context.PushClip(new RectangleGeometry(bounds));
        }

        public void DrawEllipse(Rect bounds, DrawingStyle style)
        {
            throw new NotImplementedException();
        }

        public void DrawText(Rect bounds, string text, DrawingStyle style)
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

        public void DrawLine(Rect bounds, Point p1, Point p2, DrawingStyle style)
        {
            throw new NotImplementedException();
        }

        public void DrawPolygon(Rect bounds, Point[] points, DrawingStyle style)
        {
            throw new NotImplementedException();
        }

        public void EndDrawing()
        {

        }
    }
}
