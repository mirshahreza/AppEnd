using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;

namespace AppEndCommon
{
    public static class ExtensionsForByteArray
    {
        public static byte[] ResizeImage(this byte[] imageFile, int targetSize)
        {
			using Image oldImage = Image.FromStream(new MemoryStream(imageFile), true, true);
			Size newSize = CalculateIntelligentDimensions(oldImage.Size, targetSize);
			using Bitmap newImage = new(newSize.Width, newSize.Height, PixelFormat.Format24bppRgb);
			using Graphics canvas = Graphics.FromImage(newImage);
			canvas.SmoothingMode = SmoothingMode.HighQuality;
			canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
			canvas.PixelOffsetMode = PixelOffsetMode.HighQuality;
			canvas.DrawImage(oldImage, new Rectangle(new Point(0, 0), newSize));
			MemoryStream m = new();

			ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
			EncoderParameters encoderParams = new(1);
			encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 255L);
			newImage.Save(m, info[1], encoderParams);

			return m.GetBuffer();
		}

        private static Size CalculateIntelligentDimensions(Size oldSize, int targetSize)
        {
            Size newSize = new();
            if (oldSize.Height > oldSize.Width)
            {
                newSize.Width = (int)(oldSize.Width * (targetSize / (float)oldSize.Height));
                newSize.Height = targetSize;
            }
            else
            {
                newSize.Width = targetSize;
                newSize.Height = (int)(oldSize.Height * (targetSize / (float)oldSize.Width));
            }
            return newSize;
        }
    }
}