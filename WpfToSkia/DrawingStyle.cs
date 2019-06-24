using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace WpfToSkia
{
    /// <summary>
    /// Represents a drawing style.
    /// </summary>
    public class DrawingStyle
    {
        /// <summary>
        /// Gets or sets the fill.
        /// </summary>
        public Brush Fill { get; set; }

        /// <summary>
        /// Gets or sets the stroke.
        /// </summary>
        public Brush Stroke { get; set; }

        /// <summary>
        /// Gets or sets the stroke thickness.
        /// </summary>
        public Thickness StrokeThickness { get; set; }

        /// <summary>
        /// Gets or sets the opacity.
        /// </summary>
        public double Opacity { get; set; }

        /// <summary>
        /// Gets or sets the effect.
        /// </summary>
        public Effect Effect { get; set; }

        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        public CornerRadius CornerRadius { get; set; }

        /// <summary>
        /// Gets or sets the edge mode.
        /// </summary>
        public EdgeMode EdgeMode { get; set; }

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        public FontFamily FontFamily { get; set; }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public double FontSize { get; set; }

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        public FontWeight FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        public FontStyle FontStyle { get; set; }

        /// <summary>
        /// Gets or sets the transform.
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>
        /// Gets or sets the transform origin.
        /// </summary>
        public Point TransformOrigin { get; set; }

        /// <summary>
        /// Gets a value indicating whether the opacity is smaller than 1 and opacity filter should be applied.
        /// </summary>
        public bool HasOpacity
        {
            get { return Opacity < 1; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Fill"/> property is not null.
        /// </summary>
        public bool HasFill
        {
            get { return Fill != null; }
        }


        /// <summary>
        /// Gets a value indicating whether the <see cref="Stroke"/> property is not null.
        /// </summary>
        public bool HasStroke
        {
            get { return Stroke != null && StrokeThickness.Left > 0; }
        }

        /// <summary>
        /// Creates a new <see cref="DrawingStyle"/> based on the specified <see cref="SkiaFrameworkElement"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static DrawingStyle FromElement(SkiaFrameworkElement element)
        {
            DrawingStyle style = new DrawingStyle();
            style.EdgeMode = RenderOptions.GetEdgeMode(element);
            style.Effect = element.WpfElement.Effect;

            if (element.Parent != null && element.Parent.WpfElement.RenderTransform != Transform.Identity)
            {
                style.Transform = element.Parent.WpfElement.RenderTransform;
                style.TransformOrigin = element.Parent.WpfElement.RenderTransformOrigin;
            }
            else
            {
                style.Transform = element.WpfElement.RenderTransform;
                style.TransformOrigin = element.WpfElement.RenderTransformOrigin;
            }
            return style;
        }
    }
}
