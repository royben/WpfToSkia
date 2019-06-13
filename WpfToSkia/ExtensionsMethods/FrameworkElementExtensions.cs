using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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

        public static bool DesignMode(this FrameworkElement element)
        {
            return (DesignerProperties.GetIsInDesignMode(element));
        }

        public static Rect GetBounds(this FrameworkElement element)
        {
            var parentOffset = VisualTreeHelper.GetOffset((VisualTreeHelper.GetParent(element) as FrameworkElement));
            var offset = VisualTreeHelper.GetOffset(element);
            return new Rect(parentOffset.X + offset.X, parentOffset.Y + offset.Y, element.ActualWidth, element.ActualHeight);
        }

        public static T FindAncestor<T>(this DependencyObject dependencyObject) where T : class
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);

            if (parent == null) return null;

            var parentT = parent as T;
            return parentT ?? FindAncestor<T>(parent);
        }
    }
}
