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
    public class DrawingStyle
    {
        public Brush Fill { get; set; }
        public Brush Stroke { get; set; }
        public Thickness StrokeThickness { get; set; }
        public double Opacity { get; set; }
        public Effect Effect { get; set; }
        public CornerRadius CornerRadius { get; set; }
        public EdgeMode EdgeMode { get; set; }
        public FontFamily FontFamily { get; set; }
        public double FontSize { get; set; }
        public FontWeight FontWeight { get; set; }
        public FontStyle FontStyle { get; set; }

        public bool HasOpacity
        {
            get { return Opacity > 0 && Opacity < 1; }
        }
    }
}
