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
        public Rect Bounds { get; set; }
        public FrameworkElement WpfElement { get; set; }
        public List<SkiaFrameworkElement> Children { get; set; }
        public SkiaFrameworkElement Parent { get; set; }

        public SkiaFrameworkElement()
        {
            Children = new List<SkiaFrameworkElement>();
        }

        public virtual void Render(RenderPackage package)
        {
            foreach (var child in Children)
            {
                if (child.Bounds.IntersectsWith(package.Viewport))
                {
                    child.Render(new RenderPackage()
                    {
                        Canvas = package.Canvas,
                        Opacity = Parent == null ? 1 : WpfElement.Opacity,
                        Offset = package.Offset,
                        Viewport = package.Viewport,
                    });
                }
            }
        }

        public virtual Size Measure(Size availableSize)
        {
            return availableSize;
        }

        public virtual List<BindingProperty> GetBindingProperties()
        {
            return new List<BindingProperty>()
            {
                 new BindingProperty(FrameworkElement.OpacityProperty,BindingPropertyMode.AffectsRender),
            };
        }

        protected class SKPaintBuilder
        {
            private SKPaint _paint;
            private FrameworkElement _element;
            private SkiaFrameworkElement _skia;
            private RenderPackage _package;

            public SKPaintBuilder(SkiaFrameworkElement skiaElement, RenderPackage package)
            {
                _paint = new SKPaint();
                _skia = skiaElement;
                _element = _skia.WpfElement;
                _skia = skiaElement;
                _package = package;

                _paint.IsAntialias = RenderOptions.GetEdgeMode(_element) == EdgeMode.Aliased ? false : true;

                if (_skia.Parent != null)
                {
                    //_paint.ColorFilter = SKColorFilter.CreateBlendMode(SKColors.White.WithAlpha((byte)((package.Opacity * _element.Opacity) * 255d)), SKBlendMode.DstIn);
                }

                if (_element.Effect is DropShadowEffect)
                {
                    var fx = _element.Effect as DropShadowEffect;
                    _paint.ImageFilter = SKImageFilter.CreateDropShadow(fx.ShadowDepth.ToFloat(), fx.ShadowDepth.ToFloat(), fx.BlurRadius.ToFloat(), fx.BlurRadius.ToFloat(), fx.Color.ToSKColor(), SKDropShadowImageFilterShadowMode.DrawShadowAndForeground);
                }
            }

            public SKPaintBuilder SetFill(Brush fill)
            {
                _paint.Style = SKPaintStyle.Fill;
                _paint.Color = fill.ToSKColor();
                return this;
            }

            public SKPaintBuilder SetStroke(Brush stroke, Thickness thickness)
            {
                _paint.Style = SKPaintStyle.Stroke;
                _paint.Color = stroke.ToSKColor();
                _paint.IsStroke = true;
                _paint.StrokeWidth = thickness.Left.ToFloat();
                return this;
            }

            public SKPaint Build()
            {
                return _paint;
            }
        }
    }
}
