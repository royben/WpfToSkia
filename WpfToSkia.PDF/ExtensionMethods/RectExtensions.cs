using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfToSkia.PDF.ExtensionMethods
{
    public static class RectExtensions
    {
        public static XRect ToXRect(this Rect rect)
        {
            return new XRect(rect.Left, rect.Top, rect.Width, rect.Height);
        }
    }
}
