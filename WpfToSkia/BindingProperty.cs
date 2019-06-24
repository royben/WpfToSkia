using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfToSkia
{
    /// <summary>
    /// Represents a binding property.
    /// </summary>
    public class BindingProperty
    {
        /// <summary>
        /// Gets or sets the dependency property.
        /// </summary>
        public DependencyProperty DependencyProperty { get; set; }

        /// <summary>
        /// Gets or sets the binding mode.
        /// </summary>
        public BindingPropertyMode Mode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingProperty"/> class.
        /// </summary>
        public BindingProperty()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingProperty"/> class.
        /// </summary>
        /// <param name="dp">The dependency property.</param>
        /// <param name="mode">The mode.</param>
        public BindingProperty(DependencyProperty dp, BindingPropertyMode mode) : this()
        {
            DependencyProperty = dp;
            Mode = mode;
        }
    }
}
