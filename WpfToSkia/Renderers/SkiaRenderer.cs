using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfToSkia.DrawingContexts;

namespace WpfToSkia.Renderers
{
    /// <summary>
    /// Represents a Skia <see cref="IRenderer"/>.
    /// </summary>
    /// <seealso cref="WpfToSkia.RendererBase{WpfToSkia.DrawingContexts.SkiaDrawingContext}" />
    public class SkiaRenderer : RendererBase<SkiaDrawingContext>
    {
        private SKSurface _surface;

        /// <summary>
        /// Called when the surface has been created
        /// </summary>
        /// <param name="backBuffer">The back buffer.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stride">The stride.</param>
        protected override void OnSurfaceCreated(IntPtr backBuffer, int width, int height, int stride)
        {
            if (_surface != null)
            {
                _surface.Dispose();
            }

            _surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul), backBuffer, stride);
        }

        /// <summary>
        /// Creates the drawing context.
        /// </summary>
        /// <returns></returns>
        protected override SkiaDrawingContext CreateDrawingContext()
        {
            return new SkiaDrawingContext(_surface.Canvas);
        }
    }
}
