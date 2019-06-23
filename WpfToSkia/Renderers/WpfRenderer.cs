using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfToSkia.DrawingContexts;

namespace WpfToSkia.Renderers
{
    public class WpfRenderer : RendererBase<WpfDrawingContext>
    {
        private RenderTargetBitmap _bitmap;
        private DrawingVisual _visual;
        private DrawingContext _context;
        private int _stride;

        protected override void OnSurfaceCreated(IntPtr backBuffer, int width, int height, int stride)
        {
            _stride = stride;
            _bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        }

        protected override WpfDrawingContext CreateDrawingContext()
        {
            _visual = new DrawingVisual();
            _context = _visual.RenderOpen();
            return new WpfDrawingContext(_context, (int)_bitmap.Width, (int)_bitmap.Height);
        }

        protected override void OnRenderCompleted(IntPtr backBuffer, int width, int height, int stride)
        {
            _context.Close();
            _bitmap.Render(_visual);
            _bitmap.CopyPixels(new Int32Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight), backBuffer, _stride * _bitmap.PixelHeight, _stride);
        }
    }
}
