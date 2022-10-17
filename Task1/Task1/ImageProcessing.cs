using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
    internal class ImageProcessing
    {

        public Bitmap LoadPicture(string name)
        {
            Bitmap picture = (Bitmap)Bitmap.FromFile(name);
            return picture;
        }

        public void SavePicture(Bitmap picture)
        {
            picture.Save("C://test//Light.png", ImageFormat.Png);
        }

        public Bitmap ModifyBrightness(Bitmap picture, int brightness)
        {
            for (int x = 0; x < picture.Width; x++)
            {
                for (int y = 0; y < picture.Height; y++)
                {
                    Color pixelColor = picture.GetPixel(x, y);

                    int r = brightness + pixelColor.R;
                    int g = brightness + pixelColor.G;
                    int b = brightness + pixelColor.B;

                    r = Math.Clamp(r, 0, 255);
                    g = Math.Clamp(g, 0, 255);
                    b = Math.Clamp(b, 0, 255);

                    picture.SetPixel(x, y, Color.FromArgb(pixelColor.A, r, g, b));
                }

            }
            return picture;
        }

        // 0 < contrast < 1 for less contrast
        // contrast > 1 for more contrast
        public Bitmap ModifyContrast(Bitmap picture, double contrast)
        {
            for (int x = 0; x < picture.Width; x++)
            {
                for (int y = 0; y < picture.Height; y++)
                {
                    Color pixelColor = picture.GetPixel(x, y);

                    int r = (int)(contrast * (pixelColor.R - 128) + 128);
                    int g = (int)(contrast * (pixelColor.G - 128) + 128);
                    int b = (int)(contrast * (pixelColor.B - 128) + 128);

                    r = Math.Clamp(r, 0, 255);
                    g = Math.Clamp(g, 0, 255);
                    b = Math.Clamp(b, 0, 255);

                    picture.SetPixel(x, y, Color.FromArgb(pixelColor.A, r, g, b));
                }

            }
            return picture;
        }

        public Bitmap Negative(Bitmap picture)
        {
            for (int x = 0; x < picture.Width; x++)
            {
                for (int y = 0; y < picture.Height; y++)
                {
                    Color pixelColor = picture.GetPixel(x, y);

                    int r = 255 - pixelColor.R;
                    int g = 255 - pixelColor.G;
                    int b = 255 - pixelColor.B;

                    picture.SetPixel(x, y, Color.FromArgb(pixelColor.A, r, g, b));
                }
            }
            return picture;
        }

        public Bitmap VerticalFlip(Bitmap picture)
        {
            for (int x = 0; x < picture.Width; x++)
            {
                for (int y = 0; y < picture.Height / 2; y++)
                {
                    Color pixelColor = picture.GetPixel(x, y);
                    Color pixel = picture.GetPixel(x, picture.Height - y -1);
                    picture.SetPixel(x, y, pixel);
                    picture.SetPixel(x, picture.Height - y -1, pixelColor);
                }

            }
            return picture;
        }

        public Bitmap HorizontalFlip(Bitmap picture)
        {
            for (int x = 0; x < picture.Width / 2; x++)
            {
                for (int y = 0; y < picture.Height; y++)
                {
                    Color pixelColor = picture.GetPixel(x, y);
                    Color pixel = picture.GetPixel(picture.Width - x - 1, y);
                    picture.SetPixel(x, y, pixel);
                    picture.SetPixel(picture.Width - x - 1, y, pixelColor);
                }

            }
            return picture;
        }

        public Bitmap DiagonalFlip(Bitmap picture)
        {
            for (int x = 0; x < picture.Width ; x++)
            {
                for (int y = 0; y < picture.Height/2 ; y++)
                {
                    Color pixelColor = picture.GetPixel(x, y);
                    Color pixel = picture.GetPixel(picture.Width - x - 1, picture.Height - y - 1);
                    picture.SetPixel(x, y, pixel);
                    picture.SetPixel(picture.Width - x - 1, picture.Height - y - 1, pixelColor);

                }

            }
            return picture;
        }

        // Linear interpolation
        // Params: start, end, interpolating value
        private static float Lerp(float s, float e, float t)
        {
            return s + (e - s) * t;
        }
        
        // Bilinear interpolation (interpolate between two interpolations)
        // Params: point x0y0, x1y0, x0y1, x1y1, interpolating value x, interpolating value y
        private static float Blerp(float x00, float x10, float x01, float x11, float tx, float ty)
        {
            return Lerp(Lerp(x00, x10, tx), Lerp(x01, x11, tx), ty);
        }

        public Bitmap resizeImage(Bitmap picture, double scale)
        {
            int startingWidth = picture.Width;
            int startingHeight = picture.Height;

            int newWidth = (int)(picture.Width * scale);
            int newHeight = (int)(picture.Height * scale);

            Bitmap resizedPicture = new Bitmap(newWidth, newHeight,
                          PixelFormat.Format24bppRgb);

            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    // Map scaled coordinates to original
                    float originalX = ((float)x) / newWidth * (picture.Width - 1);
                    float originalY = ((float)y) / newHeight * (picture.Height - 1);
                    int originalIntX = (int)originalX;
                    int originalIntY = (int)originalY;

                    Color x00 = picture.GetPixel(originalIntX, originalIntY);
                    Color x10 = picture.GetPixel(originalIntX + 1, originalIntY);
                    Color x01 = picture.GetPixel(originalIntX, originalIntY + 1);
                    Color x11 = picture.GetPixel(originalIntX + 1, originalIntY + 1);

                    int r = (int)Blerp(x00.R, x10.R, x01.R, x11.R, originalX - originalIntX, originalY - originalIntY);
                    int g = (int)Blerp(x00.G, x10.G, x01.G, x11.G, originalX - originalIntX, originalY - originalIntY);
                    int b = (int)Blerp(x00.B, x10.B, x01.B, x11.B, originalX - originalIntX, originalY - originalIntY);
                    Color calculatedColor = Color.FromArgb(r, g, b);
                    resizedPicture.SetPixel(x, y, calculatedColor);
                }
            }

            return resizedPicture;
        }
    }
}
