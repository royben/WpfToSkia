using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfToSkia.PDF.ExtensionMethods
{
    public static class PointExtensions
    {
        public static XPoint ToXPoint(this Point point)
        {
            return new XPoint(point.X, point.Y);
        }
    }
}
