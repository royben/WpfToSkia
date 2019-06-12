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

        public static SKRect ToSKRectStroke(this Rect rect, Thickness thickness)
        {
            return new SKRect((rect.Left + thickness.Left / 2).ToFloat(), (rect.Top + thickness.Top / 2).ToFloat(), (rect.Right - thickness.Right / 2).ToFloat(), (rect.Bottom - thickness.Bottom / 2).ToFloat());
        }

        public static SKRect ToSKRectClip(this Rect rect, Thickness thickness)
        {
            return new SKRect((rect.Left + thickness.Left).ToFloat(), (rect.Top + thickness.Top).ToFloat(), (rect.Right - thickness.Right).ToFloat(), (rect.Bottom - thickness.Bottom).ToFloat());
        }
    }
}
