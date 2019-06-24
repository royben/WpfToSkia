using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfToSkia
{
    /// <summary>
    /// Represents the <see cref="BindingEventContainer.ValueChanged"/> event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class BindingEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the Skia element.
        /// </summary>
        public SkiaFrameworkElement SkiaElement { get; set; }

        /// <summary>
        /// Gets or sets the binding property.
        /// </summary>
        public BindingProperty BindingProperty { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public Object Value { get; set; }
    }
}
