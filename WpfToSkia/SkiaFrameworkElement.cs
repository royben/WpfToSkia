using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using WpfToSkia.ExtensionsMethods;

namespace WpfToSkia
{
    public class SkiaFrameworkElement : DependencyObject
    {
        private FrameworkElement _wpfElement;
        public FrameworkElement WpfElement
        {
            get { return _wpfElement; }
            set { _wpfElement = value; OnWpfFrameworkElementChanged(value); }
        }

        public List<SkiaFrameworkElement> Children { get; set; }
        public SkiaFrameworkElement Parent { get; set; }
        public Rect Bounds { get; private set; }
        public event EventHandler<FrameworkElement> ChildAdded;
        public event EventHandler<FrameworkElement> ChildRemoved;

        public SkiaFrameworkElement()
        {
            Children = new List<SkiaFrameworkElement>();
        }

        public void Render(IDrawingContext context, Rect bounds, Rect viewport, double opacity)
        {
            var viewportBounds = bounds;
            viewportBounds.Offset(-viewport.Left, -viewport.Top);
            Bounds = viewportBounds;
            OnRender(context, viewportBounds, opacity);

            foreach (var child in Children)
            {
                var childBounds = child.GetBounds();
                childBounds.Offset(bounds.Left, bounds.Top);

                if (childBounds.IntersectsWith(viewport))
                {
                    child.Render(context, childBounds, viewport, opacity * child.WpfElement.Opacity);
                }
                else
                {
                    childBounds.Offset(-viewport.Left, -viewport.Top);
                    child.Bounds = childBounds;
                }
            }
        }

        public void Invalidate(IDrawingContext context, Rect viewport, double opacity)
        {
            OnRender(context, Bounds, opacity);

            foreach (var child in Children)
            {
                if (child.Bounds.IntersectsWith(viewport))
                {
                    child.Invalidate(context, viewport, opacity * child.WpfElement.Opacity);
                }
            }
        }

        public void InvalidateBounds()
        {
            InvalidateBounds(Parent);
        }

        private void InvalidateBounds(SkiaFrameworkElement parent)
        {
            Bounds = GetBounds();
            var bounds = Bounds;

            if (parent != null)
            {
                bounds.Offset(parent.Bounds.Left, parent.Bounds.Top);
                Bounds = bounds;
            }

            foreach (var child in Children)
            {
                child.InvalidateBounds(this);
            }
        }

        protected virtual void OnRender(IDrawingContext context, Rect bounds, double opacity)
        {

        }

        protected virtual Rect GetBounds()
        {
            var offset = VisualTreeHelper.GetOffset(WpfElement);
            return new Rect(offset.X, offset.Y, WpfElement.Width.NaNDefault(WpfElement.ActualWidth), WpfElement.Height.NaNDefault(WpfElement.ActualHeight));
        }

        public virtual List<BindingProperty> GetBindingProperties()
        {
            return new List<BindingProperty>()
            {
                 new BindingProperty(FrameworkElement.OpacityProperty,BindingPropertyMode.AffectsRender),

                 new BindingProperty(FrameworkElement.MarginProperty,BindingPropertyMode.AffectsLayout),

                 new BindingProperty(FrameworkElement.WidthProperty,BindingPropertyMode.AffectsLayout),
                 new BindingProperty(FrameworkElement.HeightProperty,BindingPropertyMode.AffectsLayout),

                 new BindingProperty(FrameworkElement.RenderTransformProperty,BindingPropertyMode.AffectsRender),
                 new BindingProperty(FrameworkElement.LayoutTransformProperty,BindingPropertyMode.AffectsLayout),

                 new BindingProperty(Canvas.LeftProperty,BindingPropertyMode.AffectsLayout),
                 new BindingProperty(Canvas.TopProperty,BindingPropertyMode.AffectsLayout),
            };
        }

        protected virtual void OnWpfFrameworkElementChanged(FrameworkElement element)
        {

        }

        protected void NotifyChildAdded(FrameworkElement element)
        {
            ChildAdded?.Invoke(this, element);
        }

        protected void NotifyChildRemoved(FrameworkElement element)
        {
            ChildRemoved?.Invoke(this, element);
        }
    }
}
