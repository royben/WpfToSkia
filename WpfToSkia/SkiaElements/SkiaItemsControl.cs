using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WpfToSkia.SkiaElements
{
    public class SkiaItemsControl : SkiaFrameworkElement
    {
        private HashSet<int> _elements;

        /// <summary>
        /// Gets or sets the framework element data item.
        /// </summary>
        public static readonly DependencyProperty DataItemProperty =
            DependencyProperty.RegisterAttached("DataItem",
            typeof(object), typeof(SkiaItemsControl),
            new FrameworkPropertyMetadata(null));
        /// <summary>
        /// Sets the DataItem attached property.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetDataItem(FrameworkElement element, object value)
        {
            element.SetValue(DataItemProperty, value);
        }

        /// <summary>
        /// Gets the DataItem attached property.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static object GetDataItem(FrameworkElement element)
        {
            return element.GetValue(DataItemProperty);
        }

        public SkiaItemsControl() : base()
        {
            _elements = new HashSet<int>();
        }

        public override List<BindingProperty> GetBindingProperties()
        {
            var list = base.GetBindingProperties();

            list.Add(new BindingProperty(ItemsControl.ItemsSourceProperty, BindingPropertyMode.AffectsLayout));
            list.Add(new BindingProperty(ItemsControl.ItemTemplateProperty, BindingPropertyMode.AffectsLayout));
            list.Add(new BindingProperty(ItemsControl.ItemsPanelProperty, BindingPropertyMode.AffectsLayout));

            return list;
        }

        protected override void OnWpfFrameworkElementChanged(FrameworkElement element)
        {
            base.OnWpfFrameworkElementChanged(element);
            ItemsControl control = element as ItemsControl;
            control.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            ItemsControl control = WpfElement as ItemsControl;
            if (control.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                for (int i = 0; i < control.Items.Count; i++)
                {
                    if (!_elements.Contains(i))
                    {
                        FrameworkElement element = control.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;

                        if (element != null)
                        {
                            element.Loaded -= Element_Loaded;
                            element.Loaded += Element_Loaded;
                            element.Unloaded -= Element_Unloaded;
                            element.Unloaded += Element_Unloaded;
                            SetDataItem(element, i);
                            _elements.Add(i);

                            if (element.IsLoaded)
                            {
                                NotifyChildAdded(element);
                            }
                        }
                    }
                }
            }
        }

        private void Element_Unloaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            element.Loaded -= Element_Loaded;
            element.Unloaded -= Element_Unloaded;
            NotifyChildRemoved(element);
            _elements.Remove((int)GetDataItem(element));
        }

        private void Element_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            NotifyChildAdded(element);
        }
    }
}
