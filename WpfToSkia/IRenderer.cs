using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfToSkia
{
    /// <summary>
    /// Represents a renderer.
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// Occurs when the source bitmap has changed. Usually called when the drawing surface size has changed.
        /// </summary>
        event EventHandler<WriteableBitmap> SourceChanged;

        SkiaTree SkiaTree { get; }

        /// <summary>
        /// Gets the current source bitmap.
        /// </summary>
        WriteableBitmap Source { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="SkiaHost"/> is inside a ScrollViewer and the its size exceeded the <see cref="MaximumBitmapSize"/>.
        /// </summary>
        bool IsVirtualizing { get; }

        /// <summary>
        /// Gets or sets the maximum size of the bitmap.
        /// When the actual size exceeds this value, the renderer will start to virtualize the rendering.
        /// </summary>
        int MaximumBitmapSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum response rate for properties changes.
        /// </summary>
        double BindingFPS { get; set; }

        /// <summary>
        /// Gets or sets the maximum response rate for parent ScrollViewer changes.
        /// </summary>
        double ScrollingFPS { get; set; }

        /// <summary>
        /// Gets or sets the maximum response rate for size changes.
        /// </summary>
        double SizingFPS { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable invalidation of elements when their properties changes.
        /// </summary>
        bool EnableBinding { get; set; }

        /// <summary>
        /// Should be called when the <see cref="SkiaHost.Child"/> is completely loaded.
        /// </summary>
        /// <param name="host">The Skia host.</param>
        void Init(SkiaHost host);
    }
}
