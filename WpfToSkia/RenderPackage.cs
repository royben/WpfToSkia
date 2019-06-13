using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfToSkia
{
    public class RenderPackage
    {
        public SKCanvas Canvas { get; set; }
        public double Opacity { get; set; }

        public RenderPackage()
        {
            Opacity = 1;
        }
    }
}
