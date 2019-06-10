using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfToSkia
{
    public class SkiaFrameworkElement
    {
        public FrameworkElement WpfElement { get; set; }
        public List<SkiaFrameworkElement> Children { get; set; }

        public SkiaFrameworkElement()
        {
            Children = new List<SkiaFrameworkElement>();
        }

        public virtual void Render(SKCanvas canvas, Rect bounds)
        {
            
        }

        public virtual Size Measure(Size availableSize)
        {
            return availableSize;
        }
    }
}
