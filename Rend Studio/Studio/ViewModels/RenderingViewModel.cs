using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Studio.Assets.Project;

namespace Studio.ViewModels
{
    public class RenderingViewModel
    {
        public ImageSource Image { get; private set; }

        public RenderingViewModel(ImageSource image)
        {
            Image = image;
        }

        public RenderingViewModel(RenderingAsset rendering)
        {
            Image = ConvertToBitmapSource(rendering.Image);
        }

        private static BitmapSource ConvertToBitmapSource(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
