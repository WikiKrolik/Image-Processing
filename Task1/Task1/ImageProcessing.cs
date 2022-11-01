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

        public Bitmap AddPaddding(Bitmap image, int border)
        {
            int newWidth = image.Width + 2 * border;
            int newHeight = image.Height + 2 * border;
            Bitmap newImage = new Bitmap(newWidth, newHeight);

            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    newImage.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                }
            }

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    newImage.SetPixel(x + border, y + border, image.GetPixel(x, y));
                }
            }

            return newImage;
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
        public Bitmap ModifyBrightness(Bitmap image, int brightness)
        {
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixelColor = image.GetPixel(x, y);

                    int r = brightness + pixelColor.R;
                    int g = brightness + pixelColor.G;
                    int b = brightness + pixelColor.B;

                    r = Math.Clamp(r, 0, 255);
                    g = Math.Clamp(g, 0, 255);
                    b = Math.Clamp(b, 0, 255);

                    image.SetPixel(x, y, Color.FromArgb(pixelColor.A, r, g, b));
                }
            }

            return image;
        }

        // 0 < contrast < 1 for less contrast
        // contrast > 1 for more contrast
        public Bitmap ModifyContrast(Bitmap image, int threshold) 
        {
         
            float contrast = (float)Math.Pow((100.0 + threshold) / 100.0, 2);

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixelColor = image.GetPixel(x, y);

                    int r = (int)(((pixelColor.R / 255.0 - 0.5) * contrast + 0.5) * 255.0);
                    int g = (int)(((pixelColor.G / 255.0 - 0.5) * contrast + 0.5) * 255.0);
                    int b = (int)(((pixelColor.B / 255.0 - 0.5) * contrast + 0.5) * 255.0);

                    r = Math.Clamp(r, 0, 255);
                    g = Math.Clamp(g, 0, 255);
                    b = Math.Clamp(b, 0, 255);

                    image.SetPixel(x, y, Color.FromArgb(pixelColor.A, r, g, b));
                }
            }
            
            return image;
        }

        public Bitmap Negative(Bitmap image)
        {
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixelColor = image.GetPixel(x, y);

                    int r = 255 - pixelColor.R;
                    int g = 255 - pixelColor.G;
                    int b = 255 - pixelColor.B;

                    image.SetPixel(x, y, Color.FromArgb(pixelColor.A, r, g, b));
                }
            }

            return image;
        }

        // -- GEOMETRIC OPERATIONS --
        public Bitmap HorizontalFlip(Bitmap image)
        {
            for (int x = 0; x < image.Width / 2; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    Color pixel = image.GetPixel(image.Width - x - 1, y);
                    image.SetPixel(x, y, pixel);
                    image.SetPixel(image.Width - x - 1, y, pixelColor);
                }
            }

            return image;
        }

        public Bitmap VerticalFlip(Bitmap image)
        {
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height / 2; y++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    Color pixel = image.GetPixel(x, image.Height - y - 1);
                    image.SetPixel(x, y, pixel);
                    image.SetPixel(x, image.Height - y - 1, pixelColor);
                }
            }

            return image;
        }

        public Bitmap DiagonalFlip(Bitmap image)
        {
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height / 2; y++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    Color pixel = image.GetPixel(image.Width - x - 1, image.Height - y - 1);
                    image.SetPixel(x, y, pixel);
                    image.SetPixel(image.Width - x - 1, image.Height - y - 1, pixelColor);
                }
            }

            return image;
        }

        public Bitmap Resize(Bitmap image, float scale)
        {
            if (scale == 1)
            {
                return image;
            }

            int startingWidth = image.Width;
            int startingHeight = image.Height;

            int newWidth = (int)(image.Width * scale);
            int newHeight = (int)(image.Height * scale);

            Bitmap resizedimage = new Bitmap(newWidth, newHeight,
                          PixelFormat.Format24bppRgb);

            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    // Map scaled coordinates to original
                    float originalX = ((float)x) / newWidth * (image.Width - 1);
                    float originalY = ((float)y) / newHeight * (image.Height - 1);
                    int originalIntX = (int)originalX;
                    int originalIntY = (int)originalY;

                    Color x00 = image.GetPixel(originalIntX, originalIntY);
                    Color x10 = image.GetPixel(originalIntX + 1, originalIntY);
                    Color x01 = image.GetPixel(originalIntX, originalIntY + 1);
                    Color x11 = image.GetPixel(originalIntX + 1, originalIntY + 1);

                    int r = (int)Blerp(x00.R, x10.R, x01.R, x11.R, originalX - originalIntX, originalY - originalIntY);
                    int g = (int)Blerp(x00.G, x10.G, x01.G, x11.G, originalX - originalIntX, originalY - originalIntY);
                    int b = (int)Blerp(x00.B, x10.B, x01.B, x11.B, originalX - originalIntX, originalY - originalIntY);
                    Color calculatedColor = Color.FromArgb(r, g, b);
                    resizedimage.SetPixel(x, y, calculatedColor);
                }
            }

            return resizedimage;
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

          public Bitmap HarmonicFilter(Bitmap image, int filterSizeWidth, int filterSizeHeight )
          {
            int w = image.Width;
            int h = image.Height;

            Bitmap filteredImage = AddPaddding(image, 0);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    float red = 0f;
                    float green = 0f;
                    float blue = 0f;
            
                    int pixel = 0;
                    
                    for (int i = x - filterSizeWidth / 2; i < x + filterSizeWidth - filterSizeWidth / 2; i++)
                    {
                        if (i < 0 || i >= filteredImage.Width) continue;
 
                        for (int j = y - filterSizeHeight / 2; j < y + filterSizeHeight - filterSizeHeight / 2; j++)
                        {
                            if (j < 0 || j >= filteredImage.Height) continue;

                            Color color = filteredImage.GetPixel(i, j);
                            if (color.R <= 0 || color.G <= 0 || color.B <= 0) continue;
                            red += 1 / (float)color.R;
                            green += 1 / (float)color.G;
                            blue += 1 / (float)color.B;
                            pixel++;
                        }
                    }
                    
                    int redHarmonic = (int)(pixel/ red);
                    int greenHarmonic = (int)(pixel/ green);
                    int blueHarmonic = (int)(pixel/ blue);

                    redHarmonic = Math.Clamp(redHarmonic, 0, 255);
                    greenHarmonic = Math.Clamp(greenHarmonic, 0, 255);
                    blueHarmonic = Math.Clamp(blueHarmonic, 0, 255);

                    filteredImage.SetPixel(x, y,Color.FromArgb(filteredImage.GetPixel(x, y).A, redHarmonic, greenHarmonic, blueHarmonic));
                }
            }

            return filteredImage;
        }

        // -- ERROR ANALYSIS --
        public float MeanSquareError(Bitmap image1, Bitmap image2)
        {
            if (image1.Width != image2.Width || image1.Height != image2.Height)
            {
                return -1;
            }

            float meanSquareErorr = 0;

            for (int x = 0; x < image1.Width; x++)
            {
                for (int y = 0; y < image1.Height; y++)
                {
                    Color pixel1 = image1.GetPixel(x, y);
                    Color pixel2 = image2.GetPixel(x, y);

                    float redDif = pixel1.R - pixel2.R;
                    float greenDif = pixel1.G - pixel2.G;
                    float blueDif = pixel1.B - pixel2.B;

                    meanSquareErorr += (redDif * redDif + greenDif * greenDif + blueDif * blueDif) / 3;
                }
            }

            return meanSquareErorr / image1.Width / image1.Height;
        }

        public float PeakMeanSquareError(Bitmap image1, Bitmap image2)
        {
            if (image1.Width != image2.Width || image1.Height != image2.Height)
            {
                return -1;
            }

            float maxR = 0;
            float maxG = 0;
            float maxB = 0;
            
            float peakMeanSquareErorr = 0;

            for (int i = 0; i < image1.Width; i++)
            {
                for (int j = 0; j < image1.Height; j++)
                {
                    Color pixel1 = image1.GetPixel(j, i);
                    Color pixel2 = image2.GetPixel(j, i);

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

            float result = (peakMeanSquareErorr / image1.Width / image1.Height) / (((maxR + maxG + maxB) / 3) * ((maxR + maxG + maxB) / 3));
            
            return result;
        }

        public double SignalToNoiseRatio(Bitmap image1, Bitmap image2)
        {
            if (image1.Width != image2.Width || image1.Height != image2.Height)
            {
                return -1;
            }

            float sum = 0;
            float dif = 0;

            for (int x = 0; x < image1.Width; x++)
            {
                for (int y = 0; y < image1.Height; y++)
                {
                    Color pixel1 = image1.GetPixel(x, y);
                    Color pixel2 = image2.GetPixel(x, y);

                    float redDif = pixel1.R - pixel2.R;
                    float greenDif = pixel1.G - pixel2.G;
                    float blueDif = pixel1.B - pixel2.B;

                    sum += (pixel1.R * pixel1.R + pixel1.G * pixel1.G + pixel1.B * pixel1.B) / 3;
                    dif += (redDif * redDif + greenDif * greenDif + blueDif * blueDif) / 3;
                }
            }
            
            return 10 * Math.Log10(sum / dif);
        }

        public double PeakSignalToNoiseRatio(Bitmap image1, Bitmap image2)
        {
            if (image1.Width != image2.Width || image1.Height != image2.Height)
            {
                return -1;
            }

            float result = (float)(10 * Math.Log10(255 * 255 / SignalToNoiseRatio(image1, image2)));

            return result;
        }

        public float MaximumDifference(Bitmap image1, Bitmap image2)
        {

            if (image1.Width != image2.Width || image1.Height != image2.Height)
            {
                return -1;
            }


            float maximumDifference = -1;

            for (int x = 0; x < image1.Width; x++)
            {
                for (int y = 0; y < image1.Height; y++)
                {
                    Color pixel1 = image1.GetPixel(x, y);
                    Color pixel2 = image2.GetPixel(x, y);

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
