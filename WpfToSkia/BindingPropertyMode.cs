using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfToSkia
{
    /// <summary>
    /// Represents a binding property mode.
    /// </summary>
    public enum BindingPropertyMode
    {
        /// <summary>
        /// Determines that the property should affect the visual presentation of the element.
        /// </summary>
        AffectsRender,

        /// <summary>
        /// Determines that the property should affect the size/position of the element.
        /// </summary>
        AffectsLayout,
    }
}
