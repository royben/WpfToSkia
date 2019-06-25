using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfToSkia.ExtensionMethods
{
    public static class BitmapSourceExtensions
    {
        public static SKBitmap ToSKBitmap(this BitmapSource bitmap)
        {
            var info = new SKImageInfo(bitmap.PixelWidth, bitmap.PixelHeight);
            var skiaBitmap = new SKBitmap(info);
            using (var pixmap = skiaBitmap.PeekPixels())
            {
                bitmap.ToSKPixmap(pixmap);
            }
            return skiaBitmap;
        }

        public static SKImage ToSKImage(this BitmapSource bitmap)
        {
            var info = new SKImageInfo(bitmap.PixelWidth, bitmap.PixelHeight);
            var image = SKImage.Create(info);
            using (var pixmap = image.PeekPixels())
            {
                bitmap.ToSKPixmap(pixmap);
            }
            return image;
        }

        public static void ToSKPixmap(this BitmapSource bitmap, SKPixmap pixmap)
        {
            if (pixmap.ColorType == SKImageInfo.PlatformColorType)
            {
                var info = pixmap.Info;
                var converted = new FormatConvertedBitmap(bitmap, PixelFormats.Pbgra32, null, 0);
                converted.CopyPixels(new Int32Rect(0, 0, info.Width, info.Height), pixmap.GetPixels(), info.BytesSize, info.RowBytes);
            }
            else
            {
                using (var tempImage = bitmap.ToSKImage())
                {
                    tempImage.ReadPixels(pixmap, 0, 0);
                }
            }
        }
    }
}
