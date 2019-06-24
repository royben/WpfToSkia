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
    /// <summary>
    /// Represents a basic Skia element with no visual representation.
    /// </summary>
    /// <seealso cref="System.Windows.DependencyObject" />
    public class SkiaFrameworkElement : DependencyObject
    {
        /// <summary>
        /// Occurs when a child was added to the <see cref="Children"/> collection.
        /// </summary>
        public event EventHandler<FrameworkElement> ChildAdded;

        /// <summary>
        /// Occurs when a child was removed from the <see cref="Children"/> collection.
        /// </summary>
        public event EventHandler<FrameworkElement> ChildRemoved;

        private FrameworkElement _wpfElement;
        /// <summary>
        /// Gets or sets the encapsulated WPF <see cref="FrameworkElement"/>.
        /// </summary>
        public FrameworkElement WpfElement
        {
            get { return _wpfElement; }
            set { _wpfElement = value; OnWpfFrameworkElementChanged(value); }
        }

        /// <summary>
        /// Gets or sets the collection of child <see cref="SkiaFrameworkElement"/>.
        /// </summary>
        public List<SkiaFrameworkElement> Children { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        public SkiaFrameworkElement Parent { get; set; }

        /// <summary>
        /// Gets the bounds.
        /// </summary>
        public Rect Bounds { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkiaFrameworkElement"/> class.
        /// </summary>
        public SkiaFrameworkElement()
        {
            Children = new List<SkiaFrameworkElement>();
        }

        /// <summary>
        /// Renders the visual of this element.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="viewport">The viewport.</param>
        /// <param name="opacity">The opacity.</param>
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

        /// <summary>
        /// Should be override in order to draw a visual representation of this element.
        /// </summary>
        protected virtual void OnRender(IDrawingContext context, Rect bounds, double opacity)
        {

        }

        /// <summary>
        /// Invalidates the visual of this element.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="viewport">The viewport.</param>
        /// <param name="opacity">The opacity.</param>
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

        /// <summary>
        /// Invalidates this element <see cref="Bounds"/>.
        /// </summary>
        public void InvalidateBounds()
        {
            InvalidateBounds(Parent);
        }

        /// <summary>
        /// Invalidates the specified element <see cref="Bounds"/>.
        /// </summary>
        /// <param name="parent">The parent.</param>
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

        /// <summary>
        /// Gets this element bounds base on its <see cref="WpfElement"/> position and size in the visual tree.
        /// </summary>
        /// <returns></returns>
        protected virtual Rect GetBounds()
        {
            var offset = VisualTreeHelper.GetOffset(WpfElement);
            return new Rect(offset.X, offset.Y, WpfElement.Width.NaNDefault(WpfElement.ActualWidth), WpfElement.Height.NaNDefault(WpfElement.ActualHeight));
        }

        /// <summary>
        /// Returns all the <see cref="BindingProperty"/> definitions for this element.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Called when the <see cref="WpfElement"/> property has been changed.
        /// </summary>
        /// <param name="element">The element.</param>
        protected virtual void OnWpfFrameworkElementChanged(FrameworkElement element)
        {

        }

        /// <summary>
        /// Notifies the <see cref="IRenderer"/> about a child added to this element.
        /// </summary>
        /// <param name="element">The element.</param>
        protected void NotifyChildAdded(FrameworkElement element)
        {
            ChildAdded?.Invoke(this, element);
        }

        /// <summary>
        /// Notifies the <see cref="IRenderer"/> about a child removed from this element.
        /// </summary>
        /// <param name="element">The element.</param>
        protected void NotifyChildRemoved(FrameworkElement element)
        {
            ChildRemoved?.Invoke(this, element);
        }
    }
}
