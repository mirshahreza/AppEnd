using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace AppEndCommon
{
    public static class ExtensionsForByteArray
    {
        public static byte[]? ResizeImage(this byte[] imageFile, int targetSize)
        {
            try
            {
                using Image image = Image.Load(imageFile);
                using var ms = new MemoryStream();
                Size newSize = CalculateIntelligentDimensions(image.Size, targetSize);
                int width = newSize.Width;
                int height = newSize.Height;
                image.Mutate(x => x.Resize(width, height));
                image.Save(ms, new JpegEncoder { Quality = 80 });
                return ms.ToArray();
            }
            catch 
            {
                return null;
            }
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