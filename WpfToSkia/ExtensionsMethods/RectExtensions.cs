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
    }
}
