using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfToSkia.ExtensionMethods
{
    public static class SkiaCanvasExtensions
    {
        public static void ApplyTransform(this SKCanvas canvas, Transform transform, Point transformOrigin, Rect bounds)
        {
            if (transform != null)
            {
                float origin_x = (bounds.Left + (transformOrigin.X * bounds.Width)).ToFloat();
                float origin_y = (bounds.Top + (transformOrigin.Y * bounds.Height)).ToFloat();

                if (transform is TransformGroup)
                {
                    foreach (var t in (transform as TransformGroup).Children)
                    {
                        ApplyTransform(canvas, t, transformOrigin, bounds);
                    }
                }
                else if (transform is ScaleTransform)
                {
                    ScaleTransform scale = transform as ScaleTransform;
                    canvas.Scale(scale.ScaleX.ToFloat(), scale.ScaleY.ToFloat(), scale.CenterX.ToFloat() + origin_x, scale.CenterY.ToFloat() + origin_y);
                }
                else if (transform is RotateTransform)
                {
                    RotateTransform rotate = transform as RotateTransform;
                    canvas.RotateDegrees(rotate.Angle.ToFloat(), rotate.CenterX.ToFloat() + origin_x, rotate.CenterY.ToFloat() + origin_y);
                }
                else if (transform is TranslateTransform)
                {
                    TranslateTransform translate = transform as TranslateTransform;
                    canvas.Translate(translate.X.ToFloat(), translate.Y.ToFloat());
                }
            }
        }
    }
}
