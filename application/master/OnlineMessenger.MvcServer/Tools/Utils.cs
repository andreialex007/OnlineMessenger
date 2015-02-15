using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace OnlineMessenger.MvcServer.Tools
{
    public static class Utils
    {
        public static string GetRandomString()
        {
            var path = Path.GetRandomFileName().Substring(7);
            path = path.Replace(".", "");
            return path;
        }

        public static Stream ToStream(this Image image, ImageFormat formaw)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, formaw);
            stream.Position = 0;
            return stream;
        }

        public static string GetMimeType(this ImageFormat imageFormat)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.First(codec => codec.FormatID == imageFormat.Guid).MimeType;
        }
    }
}