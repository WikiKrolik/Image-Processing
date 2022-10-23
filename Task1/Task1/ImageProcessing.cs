using System.Drawing;
using System.Drawing.Imaging;

namespace ImageProcessing
{
    internal class ImageProcessing
    {
        // -- HELPERS --
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

        public Bitmap LoadPicture(string name)
        {
            Bitmap picture = (Bitmap)Bitmap.FromFile(name);
            return picture;
        }

        public void SavePicture(Bitmap picture, String path)
        {
            picture.Save(path, ImageFormat.Png);
        }

        public Bitmap AddPaddding(Bitmap picture, int border)
        {
            int newWidth = picture.Width + 2 * border;
            int newHeight = picture.Height + 2 * border;
            Bitmap newPicture = new Bitmap(newWidth, newHeight);

            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    newPicture.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                }
            }

            for (int x = 0; x < picture.Width; x++)
            {
                for (int y = 0; y < picture.Height; y++)
                {
                    newPicture.SetPixel(x + border, y + border, picture.GetPixel(x, y));
                }
            }

            return newPicture;
        }

        public Color Median(Color[] arr)
        {
            int[] red = new int[arr.Length];
            int[] green = new int[arr.Length];
            int[] blue = new int[arr.Length];

            for (int i = 0; i < arr.Length; i++)
            {
                red[i] = arr[i].R;
                green[i] = arr[i].G;
                blue[i] = arr[i].B;
            }

            Array.Sort(red);
            Array.Sort(green);
            Array.Sort(blue);

            int size = arr.Length;
            int mid = size / 2;

            if (arr.Length % 2 != 0)
            {
                return Color.FromArgb(red[mid], green[mid], blue[mid]);
            }

            return Color.FromArgb(
                (red[mid] + red[mid - 1]) / 2,
                (green[mid] + green[mid - 1]) / 2,
                (blue[mid] + blue[mid - 1]) / 2);
        }

        // -- BASIC OPERATIONS --
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
        public Bitmap ModifyContrast(Bitmap picture, float contrast) 
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

        // -- GEOMETRIC OPERATIONS --
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

        public Bitmap VerticalFlip(Bitmap picture)
        {
            for (int x = 0; x < picture.Width; x++)
            {
                for (int y = 0; y < picture.Height / 2; y++)
                {
                    Color pixelColor = picture.GetPixel(x, y);
                    Color pixel = picture.GetPixel(x, picture.Height - y - 1);
                    picture.SetPixel(x, y, pixel);
                    picture.SetPixel(x, picture.Height - y - 1, pixelColor);
                }
            }

            return picture;
        }

        public Bitmap DiagonalFlip(Bitmap picture)
        {
            for (int x = 0; x < picture.Width; x++)
            {
                for (int y = 0; y < picture.Height / 2; y++)
                {
                    Color pixelColor = picture.GetPixel(x, y);
                    Color pixel = picture.GetPixel(picture.Width - x - 1, picture.Height - y - 1);
                    picture.SetPixel(x, y, pixel);
                    picture.SetPixel(picture.Width - x - 1, picture.Height - y - 1, pixelColor);
                }
            }

            return picture;
        }

        public Bitmap Resize(Bitmap picture, float scale)
        {
            if (scale == 1)
            {
                return picture;
            }

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

        // -- NOISE REMOVAL --
        public Bitmap MedianFilter(Bitmap image, int radius)
        {
            int w = image.Width;
            int h = image.Height;

            Bitmap filteredImage = AddPaddding(image, 0);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    List<Color> filterMask = new List<Color>();

                    for (int fmx = -radius; fmx <= radius; fmx++)
                    {
                        for (int fmy = -radius; fmy <= radius; fmy++)
                        {
                            if (x + fmx >= h || y + fmy >= h || x + fmx < 0 || y + fmy < 0)
                            {
                                continue;
                            }

                            filterMask.Add(filteredImage.GetPixel(x + fmx, y + fmy));
                        }
                    }

                    filteredImage.SetPixel(x, y, Median(filterMask.ToArray()));
                }
            }

            return filteredImage;
        }

        // -- ERROR ANALYSIS --
        public float meanSquareError(Bitmap picture1, Bitmap picture2)
        {
            if (picture1.Width != picture2.Width || picture1.Height != picture2.Height)
            {
                return -1;
            }

            float meanSquareErorr = 0;

            for (int x = 0; x < picture1.Width; x++)
            {
                for (int y = 0; y < picture1.Height; y++)
                {
                    Color pixel1 = picture1.GetPixel(x, y);
                    Color pixel2 = picture2.GetPixel(x, y);

                    float redDif = pixel1.R - pixel2.R;
                    float greenDif = pixel1.G - pixel2.G;
                    float blueDif = pixel1.B - pixel2.B;

                    meanSquareErorr += (redDif * redDif + greenDif * greenDif + blueDif * blueDif) / 3;
                }
            }

            return meanSquareErorr / picture1.Width / picture1.Height;
        }

        public float PeakMeanSquareError(Bitmap picture1, Bitmap picture2)
        {
            if (picture1.Width != picture2.Width || picture1.Height != picture2.Height)
            {
                return -1;
            }

            float maxR = 0;
            float maxG = 0;
            float maxB = 0;
            float peakMeanSquareErorr = 0;

            for (int i = 0; i < picture1.Width; i++)
            {
                for (int j = 0; j < picture1.Height; j++)
                {
                    Color pixel1 = picture1.GetPixel(j, i);
                    Color pixel2 = picture2.GetPixel(j, i);

                    if (pixel1.R > maxR)
                        maxR = pixel1.R;
                    if (pixel1.G > maxG)
                        maxG = pixel1.G;
                    if (pixel1.B > maxB)
                        maxB = pixel1.B;

                    float redDif = pixel1.R - pixel2.R;
                    float greenDif = pixel1.G - pixel2.G;
                    float blueDif = pixel1.B - pixel2.B;

                    peakMeanSquareErorr += (redDif * redDif + greenDif * greenDif + blueDif * blueDif) / 3;
                }
            }
            float result = peakMeanSquareErorr / (picture1.Width / picture1.Height * ((maxR + maxG + maxB) / 3) * ((maxR + maxG + maxB) / 3));

            return result;
        }

        public float maximumDifference(Bitmap picture1, Bitmap picture2)
        {

            if (picture1.Width != picture2.Width || picture1.Height != picture2.Height)
            {
                return -1;
            }

            float maximumDifference = -1;

            for (int x = 0; x < picture1.Width; x++)
            {
                for (int y = 0; y < picture1.Height; y++)
                {
                    Color pixel1 = picture1.GetPixel(x, y);
                    Color pixel2 = picture2.GetPixel(x, y);

                    float redDif = Math.Abs(pixel1.R - pixel2.R);
                    float greenDif = Math.Abs(pixel1.G - pixel2.G);
                    float blueDif = Math.Abs(pixel1.B - pixel2.B);

                    float tempDifference = (redDif + greenDif + blueDif) / 3;

                    if (tempDifference > maximumDifference)
                    {
                        maximumDifference = tempDifference;
                    }
                }
            }

            return maximumDifference;
        }
    }
}
