using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfToSkia.ExtensionsMethods
{
    public static class DoubleExtensions
    {
        public static float ToFloat(this double value)
        {
            return (float)value;
        }

        public static int ToInt32(this double value)
        {
            return (int)value;
        }

        public static double NaNDefault(this double value,double defaultValue)
        {
            return double.IsNaN(value) ? defaultValue : value;
        }

        public static bool IsNaN(this double value)
        {
            return double.IsNaN(value);
        }
    }
}
