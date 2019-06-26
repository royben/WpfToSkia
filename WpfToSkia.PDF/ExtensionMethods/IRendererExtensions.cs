using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfToSkia;
using WpfToSkia.PDF;

public static class IRendererExtensions
{
    public static void ExportToPDF(this IRenderer renderer, String title, Stream stream)
    {
        var root = renderer.SkiaTree.Root;

        PdfDocument document = new PdfDocument();
        document.Info.Title = title;

        PdfPage page = document.AddPage();
        page.Size = PdfSharp.PageSize.A4;

        XGraphics gfx = XGraphics.FromPdfPage(page);

        PdfDrawingContext context = new PdfDrawingContext(gfx);
        
        context.Rendering += (sender, bounds) =>
        {
            if (bounds.Bottom > page.Height.Value * (double)document.PageCount && bounds.Height < page.Height)
            {
                page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                gfx.Dispose();
                gfx = XGraphics.FromPdfPage(page);
                context.Reset(gfx, (page.Height.Value * (document.PageCount - 1)));
            }
        };

        root.Render(context, root.Bounds, root.Bounds, 1);

        document.Save(stream, false);

        gfx.Dispose();
    }
}
