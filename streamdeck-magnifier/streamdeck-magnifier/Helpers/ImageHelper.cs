using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Magnifier.Helpers
{
    internal static class ImageHelper
    {
        internal static Bitmap ResizeImage(Image image, int width, int height)
        {
            var newSize = new Rectangle(0, 0, width, height);
            var newImage = new Bitmap(width, height);

            newImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

                using (var mode = new ImageAttributes())
                {
                    mode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, newSize, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, mode);
                }
            }

            return newImage;
        }

        internal static Image CopyFromScreen(int zoomLevel, Point mouseLocation)
        {
            var endWidth = 144;
            var endHeight = 144;
            var startWidth = endWidth / zoomLevel;
            var startHeight = endHeight / zoomLevel;
            var xPos = Math.Max(0, mouseLocation.X - (startWidth / 2));
            var yPos = Math.Max(0, mouseLocation.Y - (startHeight / 2));
            var img = new Bitmap(startWidth, startHeight);
            using (var graphics = Graphics.FromImage(img))
            {
                graphics.CopyFromScreen(xPos, yPos, 0, 0, new Size(endWidth, endWidth));
            }

            return img;
        }

        internal static void DrawCrosshair(Image image)
        {
            using (var graphics = Graphics.FromImage(image))
            {
                var color = ((Bitmap)image).GetPixel(72, 72);
                var pen = new Pen(IsDarkColor(color) ? Color.White : Color.Black, 1);
                graphics.DrawLine(pen, 0, 72, 144, 72);
                graphics.DrawLine(pen, 72, 0, 72, 144);
            }
        }

        internal static bool IsDarkColor(Color color)
        {
            var luminance = 0.212655 * color.R + 0.715158 * color.G + 0.072187 * color.B;
            return luminance < 150;
        }
    }
}
