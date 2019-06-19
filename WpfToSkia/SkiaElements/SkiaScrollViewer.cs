using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfToSkia.SkiaElements
{
    public class SkiaScrollViewer : SkiaFrameworkElement
    {
        public override List<BindingProperty> GetBindingProperties()
        {
            var list = base.GetBindingProperties();
            list.Add(new BindingProperty(ScrollViewer.VerticalOffsetProperty, BindingPropertyMode.AffectsLayout));
            list.Add(new BindingProperty(ScrollViewer.HorizontalOffsetProperty, BindingPropertyMode.AffectsLayout));
            return list;
        }
    }
}
