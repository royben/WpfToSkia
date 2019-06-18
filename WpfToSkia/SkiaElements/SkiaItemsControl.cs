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
        private HashSet<Object> _elements;

        public SkiaItemsControl() : base()
        {
            _elements = new HashSet<object>();
        }

        public override List<BindingProperty> GetBindingProperties()
        {
            var list = base.GetBindingProperties();

            list.Add(new BindingProperty(ItemsControl.ItemsSourceProperty, BindingPropertyMode.AffectsLayout));

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
                    if (!_elements.Contains(control.Items[i]))
                    {
                        FrameworkElement element = control.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;

                        if (element != null)
                        {
                            element.Loaded -= Element_Loaded;
                            element.Loaded += Element_Loaded;
                            element.Unloaded -= Element_Unloaded;
                            element.Unloaded += Element_Unloaded;
                            _elements.Add(control.Items[i]);

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
            _elements.Remove(element);
            _elements.Remove(element.DataContext);
        }

        private void Element_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            NotifyChildAdded(element);
        }
    }
}
