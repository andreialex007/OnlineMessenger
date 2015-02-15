using System;
using System.Drawing;
using System.IO;

namespace OnlineMessenger.Domain.Utils
{
    public class ImageTools
    {
        public static byte[] ResizeImage(byte[] data, int width = 100, int height = 100)
        {
            var image = Image.FromStream(new MemoryStream(data));
            var converter = new ImageConverter();
            return (byte[]) converter.ConvertTo(new Bitmap(image, ResizeSide(image.Width, image.Height, width, height)),
                typeof (byte[]));
        }

        private static Size ResizeSide(int srcWidth, int srcHeight, int targetWidth, int targetHeight)
        {
            var size = new Size(targetWidth, targetHeight);
            var max = Math.Max(srcWidth, srcHeight);
            if (max != srcWidth)
            {
                var ratio = srcWidth / (float)srcHeight;
                size.Width = (int)(targetWidth * ratio);
            }
            if (max != srcHeight)
            {
                var ratio = srcHeight / (float)srcWidth;
                size.Height = (int)(targetHeight * ratio);
            }
            return size;
        }
    }



}
