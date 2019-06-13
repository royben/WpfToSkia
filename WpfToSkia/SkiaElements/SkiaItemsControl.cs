using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfToSkia.SkiaElements
{
    public class SkiaItemsControl : SkiaFrameworkElement
    {
        public override List<BindingProperty> GetBindingProperties()
        {
            var list = base.GetBindingProperties();

            list.Add(new BindingProperty(ItemsControl.ItemsSourceProperty, BindingPropertyMode.AffectsLayout));

            return list;
        }
    }
}
