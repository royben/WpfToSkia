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
        private SkiaFrameworkElement _skiaElement;
        private FrameworkElement _wpfElement;

        public event EventHandler<BindingEventArgs> ValueChanged;

        public Object Value
        {
            get { return (Object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(Object), typeof(BindingEventContainer), new PropertyMetadata(null, (d, e) => (d as BindingEventContainer).OnValueChanged()));

        public BindingEventContainer(SkiaFrameworkElement skiaElement, FrameworkElement wpfElement)
        {
            _skiaElement = skiaElement;
            _wpfElement = wpfElement;
        }

        private void OnValueChanged()
        {
            ValueChanged?.Invoke(this, new BindingEventArgs()
            {
                WpfElement = _wpfElement,
                SkiaElement = _skiaElement,
                Value = Value,
            });
        }

        public static BindingEventContainer Generate(SkiaFrameworkElement skiaElement, FrameworkElement wpfElement, DependencyProperty elementProperty)
        {
            BindingEventContainer container = new BindingEventContainer(skiaElement, wpfElement);

            Binding binding = new Binding();
            binding.Mode = BindingMode.OneWay;
            binding.Source = wpfElement;
            binding.Path = new PropertyPath(elementProperty);
            BindingOperations.SetBinding(container, BindingEventContainer.ValueProperty, binding);

            return container;
        }
    }
}
