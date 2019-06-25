using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfToSkia.ExtensionMethods
{
    public static class ColorExtensions
    {
        public static SKColor ToSKColor(this Color color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }

        public static System.Drawing.Color ToGdiColor(this Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
