using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfToSkia.ExtensionMethods
{
    public static class RectExtensions
    {
        public static SKRect ToSKRect(this Rect rect)
        {
            return new SKRect(rect.Left.ToFloat(), rect.Top.ToFloat(), rect.Right.ToFloat(), rect.Bottom.ToFloat());
        }

        public static SKRect ToSKRectStroke(this Rect rect, Thickness thickness)
        {
            var r =  new SKRect((rect.Left + thickness.Left / 2).ToFloat(), (rect.Top + thickness.Top / 2).ToFloat(), (rect.Right - thickness.Right / 2).ToFloat(), (rect.Bottom - thickness.Bottom / 2).ToFloat());
            return r;
        }

        public static Rect CollapseStroke(this Rect rect, Thickness thickness)
        {
            return new Rect((rect.Left + thickness.Left / 2).ToFloat(), (rect.Top + thickness.Top / 2).ToFloat(), (rect.Right - thickness.Right / 2).ToFloat(), (rect.Bottom - thickness.Bottom / 2).ToFloat());
        }

        public static SKRect ToSKRectClip(this Rect rect, Thickness thickness)
        {
            return new SKRect((rect.Left + thickness.Left).ToFloat(), (rect.Top + thickness.Top).ToFloat(), (rect.Right - thickness.Right).ToFloat(), (rect.Bottom - thickness.Bottom).ToFloat());
        }

        public static RectangleF ToGdiRectF(this Rect rect)
        {
            return new RectangleF(rect.Left.ToFloat(), rect.Top.ToFloat(), rect.Width.ToFloat(), rect.Height.ToFloat());
        }

        public static Rectangle ToGdiRect(this Rect rect)
        {
            return new Rectangle(rect.Left.ToInt32(), rect.Top.ToInt32(), rect.Width.ToInt32(), rect.Height.ToInt32());
        }
    }
}
