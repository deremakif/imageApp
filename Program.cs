using System;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using SkiaSharp;

namespace imageApp
{
    class Program
    {
        static void Main(string[] args)
        {
            #region With System.Drawing
            Bitmap result = new Bitmap(500, 500);
            var g = Graphics.FromImage(result);
            g.Clear(Color.Green);

            var inputImage = new Bitmap("assets/input.png");
            g.DrawImage(inputImage, new Point(0, 0));

            var jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            var myEncoder = Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);

            myEncoderParameters.Param[0] = new EncoderParameter(myEncoder, 100);
            result.Save("assets/outputImageWithSystemDrawing.jpg", jpgEncoder, myEncoderParameters);
            #endregion

            #region With SkiaSharp
            SKBitmap resultWithSkia = SKBitmap.Decode("assets/input.png");

            var info = new SKImageInfo(500, 500);
            using (var surface = SKSurface.Create(info))
            {
                // the the canvas and properties
                var canvas = surface.Canvas;

                canvas.Clear(SKColor.Parse("008000")); // Green Hex Code.

                if (resultWithSkia != null)
                {
                    SKImageInfo resizeInfo = new SKImageInfo(326, 296);
                    using (SKBitmap resizedSKBitmap = resultWithSkia.Resize(resizeInfo, SKFilterQuality.High))
                    using (SKImage newImg = SKImage.FromPixels(resizedSKBitmap.PeekPixels()))
                    using (SKData data = newImg.Encode(SKEncodedImageFormat.Png, 100))
                    using (Stream imgStream = data.AsStream())
                    {
                        canvas.DrawBitmap(resizedSKBitmap, new SKPoint(0, 0));
                    }
                }

                // save the file
                using (var image = surface.Snapshot())
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = System.IO.File.OpenWrite("assets/outputImageWithSkiaSharp.jpg"))
                {
                    data.SaveTo(stream);
                }
            }

            #endregion
        }

        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            try
            {
                var codecs = ImageCodecInfo.GetImageDecoders();
                foreach (var codec in codecs)
                    if (codec.FormatID == format.Guid)
                        return codec;
            }
            catch (System.Exception) { }
            return null;
        }
    }
}
