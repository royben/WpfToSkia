using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfToSkia
{
    public interface IDrawingContext
    {
        void BeginDrawing();

        void Clear(Color color);

        void DrawRect(Rect bounds, DrawingStyle style);

        void ClipRect(Rect bounds, CornerRadius cornerRadius);

        void DrawEllipse(Rect bounds, DrawingStyle style);

        void DrawText(Rect bounds, String text, DrawingStyle style);

        void DrawImage(Rect bounds, BitmapSource image, DrawingStyle style);

        void DrawGeometry(Rect bounds, Geometry geometry, DrawingStyle style);

        void DrawLine(Rect bounds, Point p1, Point p2, DrawingStyle style);

        void DrawPolygon(Rect bounds, Point[] points, DrawingStyle style);

        void EndDrawing();
    }
}
