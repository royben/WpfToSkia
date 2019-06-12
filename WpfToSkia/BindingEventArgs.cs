using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfToSkia
{
    public class BindingEventArgs : EventArgs
    {
        public SkiaFrameworkElement SkiaElement { get; set; }
        public BindingProperty BindingProperty { get; set; }
        public Object Value { get; set; }
    }
}
