using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfToSkia
{
    public class SkiaTree
    {
        public SkiaFrameworkElement Root { get; private set; }

        public SkiaTree(SkiaFrameworkElement root)
        {
            Root = root;
        }

        public List<SkiaFrameworkElement> Flatten()
        {
            return IterateInternal(Root, new List<SkiaFrameworkElement>());
        }

        private List<SkiaFrameworkElement> IterateInternal(SkiaFrameworkElement element, List<SkiaFrameworkElement> list)
        {
            list.Add(element);

            foreach (var item in element.Children)
            {
                IterateInternal(item, list);
            }

            return list;
        }

        private SkiaFrameworkElement Find(Func<SkiaFrameworkElement, bool> predicate)
        {
            return Find(Root, predicate);
        }

        private SkiaFrameworkElement Find(SkiaFrameworkElement element, Func<SkiaFrameworkElement, bool> predicate)
        {
            if (predicate(element))
            {
                return element;
            }

            foreach (var item in element.Children)
            {
                var result = Find(item, predicate);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public SkiaFrameworkElement Inject(FrameworkElement element)
        {
            var parent = VisualTreeHelper.GetParent(element);
            var treeParent = Find(x => x.WpfElement == parent);
            var existing = treeParent.Children.FirstOrDefault(x => x.WpfElement == element);

            if (existing == null)
            {
                var elementTree = SkiaTreeHelper.LoadTree(element);
                elementTree.Root.Parent = treeParent;
                treeParent.Children.Add(elementTree.Root);

                return elementTree.Root;
            }
            else
            {
                return existing;
            }
        }

        public SkiaFrameworkElement Eject(FrameworkElement element)
        {
            var skiaElement = Find(x => x.WpfElement == element);

            if (skiaElement != null)
            {
                skiaElement.Parent.Children.Remove(skiaElement);
                return skiaElement;
            }

            return null;
        }
    }
}
