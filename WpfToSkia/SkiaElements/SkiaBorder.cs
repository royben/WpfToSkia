using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SkiaSharp;
using WpfToSkia.ExtensionsMethods;

namespace WpfToSkia.SkiaElements
{
    public class SkiaBorder : SkiaFrameworkElement
    {
        public override void Render(RenderPackage package)
        {
            base.Render(package);

            var canvas = package.Canvas;
            var bounds = Bounds;
            var opacity = package.Opacity;

            Border border = WpfElement as Border;

            if (border.Background != null)
            {
                canvas.DrawRoundRect(
                    new SKRoundRect(bounds.ToSKRectStroke(border.BorderThickness), border.CornerRadius.TopLeft.ToFloat(), border.CornerRadius.TopRight.ToFloat()),
                    new SKPaintBuilder(border, package)
                    .SetFill(border.Background)
                    .Build());
            }

            if (border.BorderBrush != null && (border.BorderThickness.Top > 0 || border.BorderThickness.Left > 0 || border.BorderThickness.Right > 0 || border.BorderThickness.Bottom > 0))
            {
                canvas.DrawRoundRect(
                    new SKRoundRect(bounds.ToSKRectStroke(border.BorderThickness), border.CornerRadius.TopLeft.ToFloat(), border.CornerRadius.TopRight.ToFloat()),
                    new SKPaintBuilder(border, package)
                    .SetStroke(border.BorderBrush, border.BorderThickness)
                    .Build());
            }

            if (border.ClipToBounds)
            {
                canvas.ClipRoundRect(new SKRoundRect(bounds.ToSKRectClip(border.BorderThickness), border.CornerRadius.TopLeft.ToFloat(), border.CornerRadius.TopRight.ToFloat()), SKClipOperation.Intersect, false);
            }

            foreach (var child in Children)
            {
                var measureSize = child.Measure(bounds.Size);
                var element = child.WpfElement;

                double left = bounds.Left + element.Margin.Left;
                double top = bounds.Top + element.Margin.Top;
                double width = measureSize.Width;
                double height = measureSize.Height;

                if (element.HorizontalAlignment == HorizontalAlignment.Left)
                {
                    left = bounds.Left + element.Margin.Left;
                }
                else if (element.HorizontalAlignment == HorizontalAlignment.Right)
                {
                    left = bounds.Left + bounds.Width - width - element.Margin.Right;
                }
                else if (element.HorizontalAlignment == HorizontalAlignment.Center)
                {
                    left = bounds.Left + (bounds.Width / 2) - (width / 2) + element.Margin.Left - element.Margin.Right;
                }

                if (element.VerticalAlignment == VerticalAlignment.Top)
                {
                    top = bounds.Top + element.Margin.Top;
                }
                else if (element.VerticalAlignment == VerticalAlignment.Bottom)
                {
                    top = bounds.Top + bounds.Height - height - element.Margin.Bottom;
                }
                else if (element.VerticalAlignment == VerticalAlignment.Center)
                {
                    top = bounds.Top + (bounds.Height / 2) - (height / 2) + element.Margin.Top - element.Margin.Bottom;
                }

                Rect childBounds = new Rect(left, top, width, height);

                child.Render(new RenderPackage()
                {
                    Canvas = canvas,
                    Opacity = border.Opacity,
                });
            }
        }

        public override Size Measure(Size availableSize)
        {
            double width = WpfElement.Width;
            double height = WpfElement.Height;

            if (width.IsNaN() && WpfElement.HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                width = availableSize.Width - WpfElement.Margin.Left - WpfElement.Margin.Right;
            }

            if (height.IsNaN() && WpfElement.VerticalAlignment == VerticalAlignment.Stretch)
            {
                height = availableSize.Height - WpfElement.Margin.Top - WpfElement.Margin.Bottom;
            }

            if (Children.Count > 0)
            {
                var sizes = Children.Select(x => x.Measure(new Size(width.PositiveLimit(), height.PositiveLimit()))).ToList();
                var max_width = sizes.Max(x => x.Width) + Children.Max(x => x.WpfElement.Margin.Left) + Children.Max(x => x.WpfElement.Margin.Right);
                var max_height = sizes.Max(x => x.Height) + Children.Max(x => x.WpfElement.Margin.Top) + Children.Max(x => x.WpfElement.Margin.Bottom);

                if (width.IsNaN())
                {
                    width = max_width;
                }

                if (height.IsNaN())
                {
                    height = max_height;
                }
            }

            return new Size(width.PositiveLimit(), height.PositiveLimit());
        }
    }
}
