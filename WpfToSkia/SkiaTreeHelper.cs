using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfToSkia
{
    public static class WpfTreeHelper
    {
        public static SkiaTree LoadTree(FrameworkElement root)
        {
            return new SkiaTree(LoadTreeInternal(root));
        }

        private static SkiaFrameworkElement LoadTreeInternal(FrameworkElement root)
        {
            SkiaFrameworkElement item = SkiaElementResolver.Default.CreateElementFor(root);

            foreach (var child in LogicalTreeHelper.GetChildren(root).OfType<FrameworkElement>())
            {
                item.Children.Add(LoadTreeInternal(child));
            }

            return item;
        }
    }
}
