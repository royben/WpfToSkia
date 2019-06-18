using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfToSkia
{
    public interface IRenderer
    {
        event EventHandler<WriteableBitmap> SourceChanged;
        WriteableBitmap Source { get; }
        bool IsVirtualizing { get; }
        int MaximumBitmapSize { get; set; }
        double BindingFPS { get; set; }
        double ScrollingFPS { get; set; }
        double SizingFPS { get; set; }
        bool EnableBinding { get; set; }
        void Init(SkiaHost host);
    }
}
