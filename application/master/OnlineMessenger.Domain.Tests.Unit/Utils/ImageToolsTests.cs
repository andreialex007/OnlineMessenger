using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OnlineMessenger.Domain.Utils;

namespace OnlineMessenger.Domain.Tests.Unit.Utils
{
    public class ImageToolsTests : TestsBase
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void ResizeImage_DataAndSizesPassed_ImageCorrectlyResized()
        {
            //arrange
            var expectedWidth = 100;
            var expectedHeight = 50;
            var image = Image.FromHbitmap(new Bitmap(1000, 500).GetHbitmap());
            var converter = new ImageConverter();
            var bytes = (byte[])converter.ConvertTo(image, typeof(byte[]));

            //act
            var resizedBytes = ImageTools.ResizeImage(bytes);

            //assert
            var resizedImage = Image.FromStream(new MemoryStream(resizedBytes));
            Assert.IsTrue(resizedImage.Width == expectedWidth);
            Assert.IsTrue(resizedImage.Height == expectedHeight);
        }
    }
}
