using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfToSkia
{
    /// <summary>
    /// Represents a Skia rendering engine visual tree.
    /// </summary>
    public class SkiaTree
    {
        /// <summary>
        /// Gets the root element.
        /// </summary>
        public SkiaFrameworkElement Root { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkiaTree"/> class.
        /// </summary>
        /// <param name="root">The root.</param>
        public SkiaTree(SkiaFrameworkElement root)
        {
            Root = root;
        }

        /// <summary>
        /// returns a flat list of all elements in the tree.
        /// </summary>
        /// <returns></returns>
        public List<SkiaFrameworkElement> Flatten()
        {
            return Flatten(Root, new List<SkiaFrameworkElement>());
        }
        private List<SkiaFrameworkElement> Flatten(SkiaFrameworkElement element, List<SkiaFrameworkElement> list)
        {
            list.Add(element);

            foreach (var item in element.Children)
            {
                Flatten(item, list);
            }

            return list;
        }

        /// <summary>
        /// Finds the a <see cref="SkiaFrameworkElement"/> by the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Injects the specified element to the tree.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public SkiaFrameworkElement Inject(FrameworkElement element)
        {
            var parent = VisualTreeHelper.GetParent(element);
            var treeParent = Find(x => x.WpfElement == parent);
            var existing = treeParent.Children.FirstOrDefault(x => x.WpfElement == element);

            if (existing == null)
            {
                var elementTree = LoadTree(element);
                elementTree.Root.Parent = treeParent;
                treeParent.Children.Add(elementTree.Root);

                return elementTree.Root;
            }
            else
            {
                return existing;
            }
        }

        /// <summary>
        /// Ejects the specified element from the tree.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a new <see cref="SkiaTree"/> based on the specified <see cref="FrameworkElement"/> visual tree.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <returns></returns>
        public static SkiaTree LoadTree(FrameworkElement root)
        {
            return new SkiaTree(LoadTreeInternal(root));
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
