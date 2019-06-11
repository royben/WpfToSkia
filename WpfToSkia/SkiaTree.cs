using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
