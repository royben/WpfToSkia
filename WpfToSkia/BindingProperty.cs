using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfToSkia
{
    public class BindingProperty
    {
        public DependencyProperty DependencyProperty { get; set; }

        public BindingPropertyMode Mode { get; set; }

        public BindingProperty()
        {

        }

        public BindingProperty(DependencyProperty dp, BindingPropertyMode mode) : this()
        {
            DependencyProperty = dp;
            Mode = mode;
        }
    }
}
