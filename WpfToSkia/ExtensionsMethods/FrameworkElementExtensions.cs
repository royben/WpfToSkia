using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfToSkia.ExtensionsMethods
{
    public static class FrameworkElementExtensions
    {
        public static Rect GetMarginBounds(this FrameworkElement element, Rect bounds)
        {
            return new Rect(
                            bounds.Left + element.Margin.Left,
                            bounds.Top + element.Margin.Top,
                            bounds.Width - element.Margin.Left - element.Margin.Right,
                            bounds.Height - element.Margin.Top - element.Margin.Bottom);
        }
    }
}
