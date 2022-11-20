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

        public int[] Histogram(Bitmap image, int channel)
        {
            int[] histogramValues = new int[256];

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    int value = -1;

                    switch (channel)
                    {
                        case 0:
                            value = image.GetPixel(i, j).R;
                            break;
                        case 1:
                            value = image.GetPixel(i, j).G;
                            break;
                        case 2:
                            value = image.GetPixel(i, j).B;
                            break;
                        default:
                            continue;
                    }

                    histogramValues[value]++;
                }
            }

            return histogramValues;
        }

        // Task 2
        public Bitmap HistogramToImage(Bitmap image, int channel)
        {
            int[] histogramValues = Histogram(image, channel);

            Bitmap histogramBitmap = new Bitmap(256, 256);

            using (Graphics g = Graphics.FromImage(histogramBitmap))
            {
                g.FillRectangle(Brushes.White,
                    0,
                    0,
                    histogramBitmap.Width,
                    histogramBitmap.Height
                    );

                for (int i = 0; i < histogramValues.Length; i++)
                {
                    float scaledValue = (float)histogramValues[i] / (float)histogramValues.Max() * (float)256;

                    g.DrawLine(Pens.Black,
                        new Point(i, 255),
                        new Point(i, 255 - (int)scaledValue)
                        );
                }
            }

            return histogramBitmap;
        }

        public Bitmap Raleigh(Bitmap image, float alpha, int minBrightness)
        {
            int[] histogramValuesR = Histogram(image, 0);
            int[] histogramValuesG = Histogram(image, 1);
            int[] histogramValuesB = Histogram(image, 2);

            // g(f)
            int CalculateBrightness(int f, int[] histogramValues)
            {
                int sum = 0;
                int N = 0;

                for (N = 0; N < f; N++)
                {
                    sum += histogramValues[N];
                }

                float underRoot = (float)(2 * alpha * alpha * Math.Log(1.0 / (1.0 / (float)(image.Width * image.Height) * (float)sum)));

                if (underRoot >= 0)
                {
                    return Math.Clamp(minBrightness + (int)Math.Pow(underRoot, 0.5), 0, 255);
                }

                return f;

            }

            int[] newBrightnessR = new int[256];
            int[] newBrightnessG = new int[256];
            int[] newBrightnessB = new int[256];

            for (int i  = 0; i < 256; i++)
            {
                newBrightnessR[i] = CalculateBrightness(i, histogramValuesR);
            }

            for (int i = 0; i < 256; i++)
            {
                newBrightnessG[i] = CalculateBrightness(i, histogramValuesG);
            }

            for (int i = 0; i < 256; i++)
            {
                newBrightnessB[i] = CalculateBrightness(i, histogramValuesB);
            }

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixelColor = image.GetPixel(x, y);

                    image.SetPixel(x, y, Color.FromArgb(
                        pixelColor.A,
                        newBrightnessR[pixelColor.R],
                        newBrightnessG[pixelColor.G],
                        newBrightnessB[pixelColor.B]
                        )
                    );
                }
            }

            return this.Negative(image);
        }

        public Bitmap ApplyMask(Bitmap image, int[,] mask)
        {
            Bitmap processedImage = new Bitmap(image.Width, image.Height);

            for (int x = 1; x < image.Width - 1; x++)
            {
                for (int y = 1; y < image.Height - 1; y++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    int r = 0;
                    int g = 0;
                    int b = 0;

                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            r += mask[i, j] * image.GetPixel(x + i - 1, y + j - 1).R;
                            g += mask[i, j] * image.GetPixel(x + i - 1, y + j - 1).G;
                            b += mask[i, j] * image.GetPixel(x + i - 1, y + j - 1).B;
                        }
                    }

                    processedImage.SetPixel(x, y, Color.FromArgb(
                        pixelColor.A,
                        Math.Clamp(r, 0, 255),
                        Math.Clamp(g, 0, 255),
                        Math.Clamp(b, 0, 255)
                        )
                    );
                }
            }

            return processedImage;
        }

        public Bitmap LineIdentification(Bitmap image, int variant)
        {
            int[,] mask;

            switch (variant)
            {
                case 0:
                    mask = new int[,] {
                        { -1, 2, -1 },
                        { -1, 2, -1 },
                        { -1, 2, -1 }
                    };
                    break;
                case 1:
                    mask = new int[,] {
                        { -1, -1, 2 },
                        { -1, 2, -1 },
                        { 2, -1, -1 }
                    };
                    break;
                case 2:
                    mask = new int[,] {
                        { -1, -1, -1 },
                        { 2, 2, 2 },
                        { -1, -1, -1 }
                    };
                    break;
                case 3:
                    mask = new int[,] {
                        { 2, -1, -1 },
                        { -1, 2, -1 },
                        { -1, -1, 2 }
                    };
                    break;
                default:
                    throw new Exception("Invalid mask variant");
            }

            return this.ApplyMask(image, mask);
        }

        public double Mean(Bitmap image, int channel)
        {
            int[] histogramValues = Histogram(image, channel);

            double sum = 0;

            for (int m = 0; m < 256; m++)
            {
                sum += m * histogramValues[m];
            }

            return 1.0 / (image.Width * image.Height) * sum;
        }

        public double Variance(Bitmap image, int channel)
        {
            int[] histogramValues = Histogram(image, channel);

            // D^2
            double sum = 0;

            double mean = Mean(image, channel);

            for (int m = 0; m < 256; m++)
            {
                sum += (m - mean) * (m - mean) * histogramValues[m];
            }

            return 1.0 / (image.Width * image.Height) * sum;
        }

        public double StandardDeviation(Bitmap image, int channel)
        {
            return Math.Sqrt(Variance(image, channel));
        }

        public double AsymmetryCoefficient(Bitmap image, int channel)
        {
            int[] histogramValues = Histogram(image, channel);

            double mean = Mean(image, channel);
            double standardDeviation = StandardDeviation(image, channel);
            double sum = 0;

            for (int m = 0; m < 256; m++)
            {
                sum += (m - mean) * (m - mean) * (m - mean) * histogramValues[m];
            }

            return 1 / (standardDeviation * standardDeviation * standardDeviation) * 1 / (image.Width * image.Height) * sum;
        }

        // Task 1
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

                            filterMask.Add(image.GetPixel(x + fmx, y + fmy));
                        }
                    }

                    image.SetPixel(x, y, Median(filterMask.ToArray()));
                }
            }

            return image;
        }

          public Bitmap HarmonicFilter(Bitmap image, int radius)
          {
            int w = image.Width;
            int h = image.Height;

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    float red = 0f;
                    float green = 0f;
                    float blue = 0f;
            
                    int pixel = 0;
                    
                    for (int i = x - radius; i <= x + radius; i++)
                    {
                        if (i < 0 || i >= image.Width) continue;
 
                        for (int j = y - radius; j <= y + radius; j++)
                        {
                            if (j < 0 || j >= image.Height) continue;

                            Color color = image.GetPixel(i, j);
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

                    image.SetPixel(x, y,Color.FromArgb(image.GetPixel(x, y).A, redHarmonic, greenHarmonic, blueHarmonic));
                }
            }

            return image;
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
