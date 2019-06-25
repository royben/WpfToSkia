using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SkiaSharp;
using WpfToSkia.ExtensionMethods;

namespace WpfToSkia.SkiaElements
{
    public class SkiaTextBlock : SkiaFrameworkElement
    {
        protected override void OnRender(IDrawingContext context, Rect bounds, double opacity)
        {
            base.OnRender(context, bounds, opacity);

            TextBlock textBlock = WpfElement as TextBlock;

            DrawingStyle style = DrawingStyle.FromElement(this);
            style.Fill = textBlock.Foreground;
            style.FontFamily = textBlock.FontFamily;
            style.FontSize = textBlock.FontSize;
            style.FontStyle = textBlock.FontStyle;
            style.FontWeight = textBlock.FontWeight;
            style.Opacity = opacity;

            context.DrawText(bounds, textBlock.Text, style);
        }

        public override List<BindingProperty> GetBindingProperties()
        {
            var props = base.GetBindingProperties();
            props.Add(new BindingProperty(TextBlock.TextProperty, BindingPropertyMode.AffectsLayout));
            props.Add(new BindingProperty(TextBlock.ForegroundProperty, BindingPropertyMode.AffectsRender));
            props.Add(new BindingProperty(TextBlock.FontFamilyProperty, BindingPropertyMode.AffectsRender));
            props.Add(new BindingProperty(TextBlock.FontWeightProperty, BindingPropertyMode.AffectsRender));
            props.Add(new BindingProperty(TextBlock.FontSizeProperty, BindingPropertyMode.AffectsRender));
            props.Add(new BindingProperty(TextBlock.FontStyleProperty, BindingPropertyMode.AffectsRender));
            return props;
        }
    }
}
