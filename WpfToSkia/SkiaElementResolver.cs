using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfToSkia
{
    public class SkiaElementResolver
    {
        private static SkiaElementResolver _instance;
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

        private Dictionary<Type, Type> _binders;

        public SkiaElementResolver()
        {
            _binders = new Dictionary<Type, Type>();
        }

        public void RegisterBinder<TWpf, TSkia>() where TWpf : FrameworkElement where TSkia : SkiaFrameworkElement
        {
            _binders.Add(typeof(TWpf), typeof(TSkia));
        }

        public void Clear()
        {
            _binders.Clear();
        }

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
