using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfToSkia
{
    /// <summary>
    /// Represents a service which registers and holds the mapping between WPF elements and Skia elements.
    /// </summary>
    public class SkiaElementResolver
    {
        private Dictionary<Type, Type> _binders; //holds the element registrations.

        private static SkiaElementResolver _instance;
        /// <summary>
        /// Gets the default instance.
        /// </summary>
        public static SkiaElementResolver Default
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SkiaElementResolver();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="SkiaElementResolver"/> class from being created.
        /// </summary>
        private SkiaElementResolver()
        {
            _binders = new Dictionary<Type, Type>();
        }

        /// <summary>
        /// Registers a binding between the specified WPF type to the specified Skia type.
        /// </summary>
        /// <typeparam name="TWpf">The type of the WPF.</typeparam>
        /// <typeparam name="TSkia">The type of the skia.</typeparam>
        public void RegisterBinder<TWpf, TSkia>() where TWpf : FrameworkElement where TSkia : SkiaFrameworkElement
        {
            _binders.Add(typeof(TWpf), typeof(TSkia));
        }

        /// <summary>
        /// Clears all registrations.
        /// </summary>
        public void Clear()
        {
            _binders.Clear();
        }

        /// <summary>
        /// Creates a new instance of a <see cref="SkiaFrameworkElement"/> by looking for a proper registration of the specified <see cref="FrameworkElement"/> type.
        /// When no registration was found, will return a default instance of <see cref="SkiaFrameworkElement"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public SkiaFrameworkElement CreateElementFor(FrameworkElement element)
        {
            Type skiaType = null;
            if (_binders.TryGetValue(element.GetType(), out skiaType))
            {
                SkiaFrameworkElement skiaElement = Activator.CreateInstance(skiaType) as SkiaFrameworkElement;
                skiaElement.WpfElement = element;
                return skiaElement;
            }
            else
            {
                SkiaFrameworkElement defaultElement = Activator.CreateInstance(typeof(SkiaFrameworkElement)) as SkiaFrameworkElement;
                defaultElement.WpfElement = element;
                return defaultElement;
            }
        }
    }
}
