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

        //public void InvalidateBounds()
        //{
        //    InvalidateBoundsInternal(Root, new Vector(0, 0));
        //}

        //private void InvalidateBoundsInternal(SkiaFrameworkElement element, Vector offset)
        //{
        //    var o = VisualTreeHelper.GetOffset(element.WpfElement);
        //    var newoffset = new Vector(offset.X + o.X, offset.Y + o.Y);
        //    element.Bounds = new Rect(newoffset.X, newoffset.Y, element.WpfElement.ActualWidth, element.WpfElement.ActualHeight);

        //    foreach (var item in element.Children)
        //    {
        //        InvalidateBoundsInternal(item, newoffset);
        //    }
        //}
    }
}
