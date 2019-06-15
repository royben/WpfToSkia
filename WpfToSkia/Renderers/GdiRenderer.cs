using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfToSkia.DrawingContexts;

namespace WpfToSkia.Renderers
{
    public class GdiRenderer : RendererBase<GdiDrawingContext>
    {
        private Bitmap _gdi_bitmap;
        private Graphics _g;

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
        }

        protected override GdiDrawingContext CreateDrawingContext()
        {
            return new GdiDrawingContext(_g);
        }
    }
}
