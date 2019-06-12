using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfToSkia
{
    public class BindingEventContainer : DependencyObject
    {
        public event EventHandler<BindingEventArgs> ValueChanged;

        public Object Value
        {
            get { return (Object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(Object), typeof(BindingEventContainer), new PropertyMetadata(null, (d, e) => (d as BindingEventContainer).OnValueChanged()));

        public BindingProperty BindingProperty { get; set; }

        public SkiaFrameworkElement SkiaElement { get; set; }

        public BindingEventContainer(SkiaFrameworkElement skiaElement, BindingProperty bindingProperty)
        {
            SkiaElement = skiaElement;
            BindingProperty = bindingProperty;
        }

        private void OnValueChanged()
        {
            ValueChanged?.Invoke(this, new BindingEventArgs()
            {
                BindingProperty = BindingProperty,
                SkiaElement = SkiaElement,
                Value = Value,
            });
        }

        public static BindingEventContainer Generate(SkiaFrameworkElement skiaElement, BindingProperty bindingProperty)
        {
            BindingEventContainer container = new BindingEventContainer(skiaElement, bindingProperty);

            Binding binding = new Binding();
            binding.Mode = BindingMode.OneWay;
            binding.Source = skiaElement.WpfElement;
            binding.Path = new PropertyPath(bindingProperty.DependencyProperty);
            BindingOperations.SetBinding(container, BindingEventContainer.ValueProperty, binding);

            return container;
        }
    }
}
