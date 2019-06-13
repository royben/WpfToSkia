using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfToSkia.ExtensionsMethods
{
    public static class RectExtensions
    {
        public static SKRect ToSKRect(this Rect rect)
        {
            return new SKRect(rect.Left.ToFloat(), rect.Top.ToFloat(), rect.Right.ToFloat(), rect.Bottom.ToFloat());
        }

        public static SKRect ToSKRectStroke(this Rect rect, Thickness thickness, double offsetX = 0, double offsetY = 0)
        {
            var r =  new SKRect((rect.Left + thickness.Left / 2 + offsetX).ToFloat(), (rect.Top + thickness.Top / 2 + offsetY).ToFloat(), (rect.Right - thickness.Right / 2 - offsetX).ToFloat(), (rect.Bottom - thickness.Bottom / 2 + offsetY).ToFloat());
            return r;
        }

        public static SKRect ToSKRectClip(this Rect rect, Thickness thickness, double offsetX = 0, double offsetY = 0)
        {
            return new SKRect((rect.Left + thickness.Left + offsetX).ToFloat(), (rect.Top + thickness.Top + offsetY).ToFloat(), (rect.Right - thickness.Right + offsetX).ToFloat(), (rect.Bottom - thickness.Bottom + offsetY).ToFloat());
        }
    }
}
