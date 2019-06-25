using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfToSkia.PDF.ExtensionMethods
{
    public static class ColorExtensions
    {
        public static XColor ToXColor(this Color color)
        {
            return XColor.FromArgb(color);
        }
    }
}
