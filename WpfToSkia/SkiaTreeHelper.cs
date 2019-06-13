using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfToSkia
{
    public static class WpfTreeHelper
    {
        public static SkiaTree LoadTree(FrameworkElement root)
        {
            var item = LoadTreeInternal(root, new Vector(0, 0));
            item.Bounds = new Rect(0, 0, root.ActualWidth, root.ActualHeight);
            return new SkiaTree(item);
        }

        private static SkiaFrameworkElement LoadTreeInternal(FrameworkElement root, Vector offset)
        {
            SkiaFrameworkElement item = SkiaElementResolver.Default.CreateElementFor(root);

            if (item != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
                {
                    var element = VisualTreeHelper.GetChild(root, i) as FrameworkElement;

                    if (element != null)
                    {
                        var item_offset = VisualTreeHelper.GetOffset(element);
                        var new_offset = new Vector(offset.X + item_offset.X, offset.Y + item_offset.Y);

                        var treeItem = LoadTreeInternal(element, new_offset);

                        if (treeItem != null)
                        {
                            treeItem.Bounds = new Rect(new_offset.X, new_offset.Y, element.ActualWidth, element.ActualHeight);
                            item.Children.Add(treeItem);
                        }
                    }
                }

                return item;
            }

            return null;
        }

        private static void SetBounds(FrameworkElement element, FrameworkElement parent, SkiaFrameworkElement skia)
        {
            var parentOffset = VisualTreeHelper.GetOffset(parent);
            var offset = VisualTreeHelper.GetOffset(element);
            skia.Bounds = new Rect(parentOffset.X + offset.X, parentOffset.Y + offset.Y, element.ActualWidth, element.ActualHeight);
        }
    }
}
