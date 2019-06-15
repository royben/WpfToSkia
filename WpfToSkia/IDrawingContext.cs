using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfToSkia
{
    public interface IDrawingContext
    {
        void BeginDrawing();

        void Clear(Color color);

        void DrawRect(Rect bounds, DrawingStyle style);

        void ClipRect(Rect bounds, CornerRadius cornerRadius);

        void DrawText(String text, Rect bounds, DrawingStyle style);

        void EndDrawing();
    }
}
