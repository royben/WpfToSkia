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
    /// <summary>
    /// Represents a binding event container.
    /// </summary>
    /// <seealso cref="System.Windows.DependencyObject" />
    public class BindingEventContainer : DependencyObject
    {
        /// <summary>
        /// Occurs when the dependency property value has changed.
        /// </summary>
        public event EventHandler<BindingEventArgs> ValueChanged;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public Object Value
        {
            get { return (Object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(Object), typeof(BindingEventContainer), new PropertyMetadata(null, (d, e) => (d as BindingEventContainer).OnValueChanged()));

        /// <summary>
        /// Gets or sets the binding property.
        /// </summary>
        public BindingProperty BindingProperty { get; set; }

        /// <summary>
        /// Gets or sets the skia element.
        /// </summary>
        public SkiaFrameworkElement SkiaElement { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingEventContainer"/> class.
        /// </summary>
        /// <param name="skiaElement">The skia element.</param>
        /// <param name="bindingProperty">The binding property.</param>
        public BindingEventContainer(SkiaFrameworkElement skiaElement, BindingProperty bindingProperty)
        {
            SkiaElement = skiaElement;
            BindingProperty = bindingProperty;
        }

        /// <summary>
        /// Called when the value has been changed
        /// </summary>
        protected virtual void OnValueChanged()
        {
            ValueChanged?.Invoke(this, new BindingEventArgs()
            {
                BindingProperty = BindingProperty,
                SkiaElement = SkiaElement,
                Value = Value,
            });
        }

        /// <summary>
        /// Generates a new <see cref="BindingEventContainer"/> for the specified Skia element and binding property.
        /// </summary>
        /// <param name="skiaElement">The skia element.</param>
        /// <param name="bindingProperty">The binding property.</param>
        /// <returns></returns>
        public static BindingEventContainer Generate(SkiaFrameworkElement skiaElement, BindingProperty bindingProperty)
        {
            BindingEventContainer container = new BindingEventContainer(skiaElement, bindingProperty);

            Binding binding = new Binding();
            binding.Mode = BindingMode.OneWay;
            binding.Source = skiaElement.WpfElement;
            binding.Path = new PropertyPath(bindingProperty.DependencyProperty);
            binding.IsAsync = true;
            BindingOperations.SetBinding(container, BindingEventContainer.ValueProperty, binding);

            return container;
        }
    }
}
