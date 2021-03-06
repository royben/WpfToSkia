﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfToSkia.Demo
{
    public class DataItem
    {
        private static Random _rnd = new Random();

        public String Name { get; set; }

        public Color Color { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public DataItem()
        {
            Color = Color.FromRgb((byte)_rnd.Next(0, 255), (byte)_rnd.Next(0, 255), (byte)_rnd.Next(0, 255));
            X = _rnd.Next(0, 1920);
            Y = _rnd.Next(0, 1080);
        }
    }
}
