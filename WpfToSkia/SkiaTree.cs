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
    }
}
