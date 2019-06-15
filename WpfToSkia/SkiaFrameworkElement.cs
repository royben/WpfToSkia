using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using WpfToSkia.ExtensionsMethods;

namespace WpfToSkia
{
    public class SkiaFrameworkElement
    {
        public FrameworkElement WpfElement { get; set; }
        public List<SkiaFrameworkElement> Children { get; set; }
        public SkiaFrameworkElement Parent { get; set; }
        public Rect Bounds { get; private set; }

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

        protected virtual void OnRender(IDrawingContext context, Rect bounds, double opacity)
        {

        }

        protected virtual Rect GetBounds()
        {
            var offset = VisualTreeHelper.GetOffset(WpfElement);
            return new Rect(offset.X, offset.Y, WpfElement.ActualWidth, WpfElement.ActualHeight);
        }

        public virtual List<BindingProperty> GetBindingProperties()
        {
            return new List<BindingProperty>()
            {
                 new BindingProperty(FrameworkElement.OpacityProperty,BindingPropertyMode.AffectsRender),
            };
        }
    }
}
