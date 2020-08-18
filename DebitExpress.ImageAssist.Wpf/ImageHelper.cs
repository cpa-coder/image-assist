using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace DebitExpress.ImageAssist.Wpf
{
    public static class ImageHelper
    {
        /// <summary>
        /// Convert byte array to Image
        /// </summary>
        /// <param name="imageArray"></param>
        /// <returns></returns>
        public static BitmapImage ToImage(this byte[] imageArray)
        {
            if (imageArray == null) return null;

            using var stream = new MemoryStream(imageArray);

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        /// <summary>
        /// Convert image to byte array with the smallest size based on the <see cref="maxWidth"/> and <see cref="maxHeight"/>
        /// </summary>
        /// <param name="image">
        /// The image to be converted
        /// </param>
        /// <param name="maxWidth">
        /// Crop from center to the maximum width
        /// </param>
        /// <param name="maxHeight">
        /// Crop from center to the maximum height
        /// </param>
        /// <returns></returns>
        public static byte[] ToByteArray(this Image image, int maxWidth, int maxHeight)
        {
            Bitmap bitmap = null;
            try
            {
                var left = 0;
                var top = 0;
                int srcWidth;
                int srcHeight;

                bitmap = new Bitmap(maxWidth, maxHeight);
                var croppedHeightToWidth = (double)maxHeight / maxWidth;
                var croppedWidthToHeight = (double)maxWidth / maxHeight;

                if (image.Width > image.Height)
                {
                    srcWidth = (int)Math.Round(image.Height * croppedWidthToHeight);
                    if (srcWidth < image.Width)
                    {
                        srcHeight = image.Height;
                        left = (image.Width - srcWidth) / 2;
                    }
                    else
                    {
                        srcHeight = (int)Math.Round(image.Height * ((double)image.Width / srcWidth));
                        srcWidth = image.Width;
                        top = (image.Height - srcHeight) / 2;
                    }
                }
                else
                {
                    srcHeight = (int)Math.Round(image.Width * croppedHeightToWidth);
                    if (srcHeight < image.Height)
                    {
                        srcWidth = image.Width;
                        top = (image.Height - srcHeight) / 2;
                    }
                    else
                    {
                        srcWidth = (int)Math.Round(image.Width * ((double)image.Height / srcHeight));
                        srcHeight = image.Height;
                        left = (image.Width - srcWidth) / 2;
                    }
                }

                using var g = Graphics.FromImage(bitmap);
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    new Rectangle(left, top, srcWidth, srcHeight), GraphicsUnit.Pixel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            var jpgInfo = ImageCodecInfo.GetImageEncoders()
                .First(codecInfo => codecInfo.MimeType == image.GetMimeType());

            return bitmap.ToByteArray(jpgInfo);
        }

        private static string GetMimeType(this Image image)
        {
            var guid = image.RawFormat.Guid;
            foreach (var codec in ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == guid)
                    return codec.MimeType;
            }

            return "image/unknown";
        }

        private static byte[] ToByteArray(this Image bitmap, ImageCodecInfo jpgInfo)
        {
            using var encParams = new EncoderParameters(1)
            {
                Param = { [0] = new EncoderParameter(Encoder.Quality, (long)100) }
            };

            using var ms = new MemoryStream();
            bitmap?.Save(ms, jpgInfo, encParams);
            return ms.ToArray();
        }
    }
}