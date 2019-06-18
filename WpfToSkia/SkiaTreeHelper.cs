using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfToSkia
{
    public static class SkiaTreeHelper
    {
        public static SkiaTree LoadTree(FrameworkElement root)
        {
            var item = LoadTreeInternal(root);
            return new SkiaTree(item);
        }

        private static SkiaFrameworkElement LoadTreeInternal(FrameworkElement root)
        {
            SkiaFrameworkElement item = SkiaElementResolver.Default.CreateElementFor(root);

            if (item != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
                {
                    var element = VisualTreeHelper.GetChild(root, i) as FrameworkElement;

                    if (element != null)
                    {
                        var treeItem = LoadTreeInternal(element);

                        if (treeItem != null)
                        {
                            treeItem.Parent = item;
                            item.Children.Add(treeItem);
                        }
                    }
                }

                return item;
            }

            return null;
        }
    }
}
