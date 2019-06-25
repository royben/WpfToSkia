using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfToSkia.PDF.ExtensionMethods
{
    public static class BrushExtensions
    {
        public static XBrush ToXBrush(this Brush brush, double width, double height)
        {
            if (brush == null)
            {
                return null;
            }
            else if (brush is SolidColorBrush)
            {
                return new XSolidBrush((brush as SolidColorBrush).Color.ToXColor());
            }
            else if (brush is LinearGradientBrush)
            {
                var b = brush as LinearGradientBrush;
                return new XLinearGradientBrush(new Rect(0, 0, width, height), b.GradientStops.FirstOrDefault().Color.ToXColor(), b.GradientStops.LastOrDefault().Color.ToXColor(), XLinearGradientMode.Horizontal);
            }
            else
            {
                throw new NotSupportedException($"Unsupported brush type {brush.GetType().Name}.");
            }
        }

        public static XColor ToXColor(this Brush brush)
        {
            if (brush == null)
            {
                return XColors.Transparent;
            }
            else if (brush is SolidColorBrush)
            {
                return (brush as SolidColorBrush).Color.ToXColor();
            }
            else
            {
                throw new NotSupportedException($"Unsupported brush type {brush.GetType().Name}.");
            }
        }
    }
}
