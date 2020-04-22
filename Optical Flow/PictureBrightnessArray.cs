using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Optical_Flow
{
    //actually just a black and white image but I wasn't feeling very explicit when I named this
    public class BlackAndWhiteDoubleArray
    {
        public int height { get; }
        public int width { get; }
        public double[,] BrightnessArray { get; }

        //public constructor coverts RBG colour into a greyscale image
        public BlackAndWhiteDoubleArray(Bitmap bmp)
        {
            if (bmp == null) throw new NullReferenceException("Bitmap is null");
            height = bmp.Height;
            width = bmp.Width;
            BrightnessArray = new double[width,height];

            //locking the bitmap into memory improves performance considerably
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData data = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);

            IntPtr ptr = data.Scan0;

            int bytes = data.Stride * height;
            int stride = data.Stride;
            int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;

            //Save it into an array so that we can unlock the bitmap and forget about it
            byte[] rgbValues = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            bmp.UnlockBits(data);
            bmp.Dispose();

            //converts RBG to brightness. bitmaps apparently store them in Blue, Green, Red order
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //This particular way of converting to black and white may be to precise for our needs and a simple average may suffice. consider seeking and expert opinion
                    int index = y * stride + x * bytesPerPixel;
                    double fB = (double)rgbValues[index] / 255;
                    double fG = (double)rgbValues[index + 1] / 255;
                    double fR = (double)rgbValues[index + 2] / 255;
                    BrightnessArray[x, y] = Math.Sqrt(0.114f * fB * fB + 0.587f * fG * fG + 0.299f * fR * fR);
                }
            }
        }
    }
}
