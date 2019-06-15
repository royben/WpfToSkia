using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfToSkia.DrawingContexts;

namespace WpfToSkia.Renderers
{
    public class SkiaRenderer : RendererBase<SkiaDrawingContext>
    {
        private SKSurface _surface;

        protected override void OnSurfaceCreated(IntPtr backBuffer, int width, int height, int stride)
        {
            if (_surface != null)
            {
                _surface.Dispose();
            }

            _surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul), backBuffer, stride);
        }

        protected override SkiaDrawingContext CreateDrawingContext()
        {
            return new SkiaDrawingContext(_surface.Canvas);
        }
    }
}
