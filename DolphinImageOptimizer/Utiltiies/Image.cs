using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace DolphinImageOptimizer.Utiltiies
{
    /// <summary>
    /// Provides various image manipulation utilities.
    /// </summary>
    public static class Image
    { 
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            // Create container
            var result = new Bitmap(width, height);
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution); // Set resolution to avoid cropping.

            // Use Graphics to draw resized image onto screen.
            using Graphics graphics = Graphics.FromImage(result);

            // Set the resize quality modes.
            graphics.PixelOffsetMode    = PixelOffsetMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode  = InterpolationMode.High;
            graphics.SmoothingMode      = SmoothingMode.HighQuality;

            // Draw
            graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            return result;
        }

        /// <summary>
        /// Checks if the whole image consists of just one colour.
        /// </summary>
        public static unsafe bool AllOneColor(Bitmap bmp)
        {
            // Lock native memory. 
            Rectangle rect     = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);

            // Get the address of the first line.
            byte* data = (byte*) bmpData.Scan0;
            int bytes  = bmpData.Stride * bmp.Height;

            bool allOneColor = true;
            for (int x = 0; x < bytes; x++)
            {
                //compare the current A or R or G or B with the A or R or G or B at position 0,0.
                if (data[x] != data[x % 4])
                {
                    allOneColor = false;
                    break;
                }
            }

            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            return allOneColor;
        }

        /// <summary>
        /// Checks if the image contains any transparency.
        /// </summary>
        public static unsafe bool HasTransparency(Bitmap bmp)
        {
            // Lock native memory. 
            Rectangle rect     = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);

            // Get the address of the first line.
            byte* data = (byte*) bmpData.Scan0;
            int bytes  = bmpData.Stride * bmp.Height;

            bool hasTransparency = false;
            for (int x = 3; x < bytes; x += 4)
            {
                if (data[x] != 255)
                {
                    hasTransparency = true;
                    break;
                }
            }

            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            return hasTransparency;
        }
    }
}