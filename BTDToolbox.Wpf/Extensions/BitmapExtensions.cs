using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace BTDToolbox.Wpf
{
    public static class BitmapExtensions
    {
        public static BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                           bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions());
            return bitmapSource;
        }
    }
}
