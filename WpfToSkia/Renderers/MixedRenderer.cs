using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfToSkia.DrawingContexts;

namespace WpfToSkia.Renderers
{
    public class MixedRenderer : RendererBase<MixedDrawingContext>
    {
        private Bitmap _gdi_bitmap;
        private Graphics _g;
        private SKSurface _surface;

        protected override void OnSurfaceCreated(IntPtr backBuffer, int width, int height, int stride)
        {
            if (_gdi_bitmap != null)
            {
                _gdi_bitmap.Dispose();
                _g.Dispose();
            }

            _gdi_bitmap = new Bitmap(width, height,
                                        stride,
                                        System.Drawing.Imaging.PixelFormat.Format32bppPArgb,
                                        backBuffer);

            _g = Graphics.FromImage(_gdi_bitmap);

            if (_surface != null)
            {
                _surface.Dispose();
            }

            _surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul), backBuffer, stride);
        }

        protected override MixedDrawingContext CreateDrawingContext()
        {
            return new MixedDrawingContext(_g, _surface.Canvas);
        }
    }
}
