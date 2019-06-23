using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfToSkia.ExtensionsMethods
{
    public static class PointExtensions
    {
        public static SKPoint ToSKPoint(this Point point)
        {
            return new SKPoint(point.X.ToFloat(), point.Y.ToFloat());
        }
    }
}
