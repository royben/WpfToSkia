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
    /// <summary>
    /// Represents a drawing context.
    /// </summary>
    public interface IDrawingContext
    {
        /// <summary>
        /// Called before drawing session starts.
        /// </summary>
        void BeginDrawing();

        /// <summary>
        /// Clears the current surface.
        /// </summary>
        /// <param name="color">The color.</param>
        void Clear(Color color);

        /// <summary>
        /// Draws a rectangle. Rounded rectangle can be specified using the <see cref="DrawingStyle.CornerRadius"/> property.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="style">The style.</param>
        void DrawRect(Rect bounds, DrawingStyle style);

        /// <summary>
        /// Clips the current drawing session.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="cornerRadius">The corner radius.</param>
        void ClipRect(Rect bounds, CornerRadius cornerRadius);

        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="style">The style.</param>
        void DrawEllipse(Rect bounds, DrawingStyle style);

        /// <summary>
        /// Draws the specified text.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="text">The text.</param>
        /// <param name="style">The style.</param>
        void DrawText(Rect bounds, String text, DrawingStyle style);

        /// <summary>
        /// Draws the specified image.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="image">The image.</param>
        /// <param name="style">The style.</param>
        void DrawImage(Rect bounds, BitmapSource image, DrawingStyle style);

        /// <summary>
        /// Draws the specified geometry.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="geometry">The geometry.</param>
        /// <param name="style">The style.</param>
        void DrawGeometry(Rect bounds, Geometry geometry, DrawingStyle style);

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="style">The style.</param>
        void DrawLine(Rect bounds, Point p1, Point p2, DrawingStyle style);

        /// <summary>
        /// Draws a polygon.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="points">The points.</param>
        /// <param name="style">The style.</param>
        void DrawPolygon(Rect bounds, Point[] points, DrawingStyle style);

        /// <summary>
        /// Called when the drawing sessions ends.
        /// </summary>
        void EndDrawing();
    }
}
