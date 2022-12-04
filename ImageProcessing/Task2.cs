using System.Drawing;

namespace ImageProcessing
{
    internal partial class ImageProcessing
    {
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

                    switch (channel)
                    {
                        case 0:
                            g.DrawLine(Pens.Red,
                                new Point(i, 255),
                                new Point(i, 255 - (int)scaledValue)
                                );
                            break;
                        case 1:
                            g.DrawLine(Pens.Green,
                                new Point(i, 255),
                                new Point(i, 255 - (int)scaledValue)
                                );
                            break;
                        case 2:
                            g.DrawLine(Pens.Blue,
                                new Point(i, 255),
                                new Point(i, 255 - (int)scaledValue)
                                );
                            break;
                        default:
                            g.DrawLine(Pens.Black,
                                new Point(i, 255),
                                new Point(i, 255 - (int)scaledValue)
                                );
                            break;
                    }

                }
            }

            return histogramBitmap;
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
        public Bitmap Raleigh(Bitmap image, int minBrightness, int maxBrightness)
        {
            // Becasue of .Negative() operation at the end, the values need to be adjusted
            int adjustedMinBrightness = 255 - maxBrightness;
            int adjustedMaxBrightness = 255 - minBrightness;

            int[] histogramValuesR = Histogram(image, 0);
            int[] histogramValuesG = Histogram(image, 1);
            int[] histogramValuesB = Histogram(image, 2);

            // g(f)
            int CalculateBrightness(int f, int[] histogramValues)
            {
                int sum = 0;
                int N = 0;

                // find prev max brightness and calculate correct alpha for it
                int prevMaxBrightness = 0;
                for (int i = 255; i > 0; i--)
                {
                    if (histogramValues[i] != 0)
                    {
                        prevMaxBrightness = i;
                        break;
                    }
                }
                float alpha = ((float)adjustedMaxBrightness - (float)adjustedMinBrightness) / (float)Math.Sqrt(2 * Math.Log((float)(image.Width * image.Height) / (float)histogramValues[prevMaxBrightness]));

                for (N = 0; N <= f; N++)
                {
                    sum += histogramValues[N];
                }

                float underRoot = (float)(2 * alpha * alpha * Math.Log(1.0 / (1.0 / (float)(image.Width * image.Height) * (float)sum)));

                return Math.Clamp(adjustedMinBrightness + (int)Math.Pow(underRoot, 0.5), 0, 255);
            }

            int[] newBrightnessR = new int[256];
            int[] newBrightnessG = new int[256];
            int[] newBrightnessB = new int[256];

            for (int i = 0; i < 256; i++)
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
                case 4:
                    mask = new int[,] {
                        { 0, 1, 0 },
                        { 1, -4, 1 },
                        { 0, 1, 0 }
                    };
                    break;
                default:
                    throw new Exception("Invalid mask variant");
            }

            return this.ApplyMask(image, mask);
        }

        public Bitmap LineIdentificationOptimized(Bitmap image)
        {
            // Optimized for this mask
            // { -1, 2, -1 }
            // { -1, 2, -1 }
            // { -1, 2, -1 }
            
            int[] r0 = new int[3], r1 = new int[3], r2 = new int[3];
            int[] g0 = new int[3], g1 = new int[3], g2 = new int[3];
            int[] b0 = new int[3], b1 = new int[3], b2 = new int[3];

            Bitmap processedImage = new Bitmap(image.Width, image.Height);

            for (int y = 1; y < image.Width - 1; y++)
            {
                r0[0] = image.GetPixel(0, y - 1).R;
                r0[1] = image.GetPixel(0, y).R;
                r0[2] = image.GetPixel(0, y + 1).R;

                r1[0] = image.GetPixel(1, y - 1).R;
                r1[1] = image.GetPixel(1, y).R;
                r1[2] = image.GetPixel(1, y + 1).R;

                g0[0] = image.GetPixel(0, y - 1).G;
                g0[1] = image.GetPixel(0, y).G;
                g0[2] = image.GetPixel(0, y + 1).G;

                g1[0] = image.GetPixel(1, y - 1).G;
                g1[1] = image.GetPixel(1, y).G;
                g1[2] = image.GetPixel(1, y + 1).G;

                b0[0] = image.GetPixel(0, y - 1).B;
                b0[1] = image.GetPixel(0, y).B;
                b0[2] = image.GetPixel(0, y + 1).B;

                b1[0] = image.GetPixel(1, y - 1).B;
                b1[1] = image.GetPixel(1, y).B;
                b1[2] = image.GetPixel(1, y + 1).B;

                for (int x = 1; x < image.Height - 1; x++)
                {
                    int r = 0, g = 0, b = 0;

                    if (x % 3 == 0)
                    {
                        r2[0] = image.GetPixel(x + 1, y - 1).R;
                        r2[1] = image.GetPixel(x + 1, y).R;
                        r2[2] = image.GetPixel(x + 1, y + 1).R;

                        g2[0] = image.GetPixel(x + 1, y - 1).G;
                        g2[1] = image.GetPixel(x + 1, y).G;
                        g2[2] = image.GetPixel(x + 1, y + 1).G;

                        b2[0] = image.GetPixel(x + 1, y - 1).B;
                        b2[1] = image.GetPixel(x + 1, y).B;
                        b2[2] = image.GetPixel(x + 1, y + 1).B;
                    }
                    else if (x % 3 == 1)
                    {
                        r0[0] = image.GetPixel(x + 1, y - 1).R;
                        r0[1] = image.GetPixel(x + 1, y).R;
                        r0[2] = image.GetPixel(x + 1, y + 1).R;

                        g0[0] = image.GetPixel(x + 1, y - 1).G;
                        g0[1] = image.GetPixel(x + 1, y).G;
                        g0[2] = image.GetPixel(x + 1, y + 1).G;

                        b0[0] = image.GetPixel(x + 1, y - 1).B;
                        b0[1] = image.GetPixel(x + 1, y).B;
                        b0[2] = image.GetPixel(x + 1, y + 1).B;
                    }
                    else 
                    {
                        r1[0] = image.GetPixel(x + 1, y - 1).R;
                        r1[1] = image.GetPixel(x + 1, y).R;
                        r1[2] = image.GetPixel(x + 1, y + 1).R;

                        g1[0] = image.GetPixel(x + 1, y - 1).G;
                        g1[1] = image.GetPixel(x + 1, y).G;
                        g1[2] = image.GetPixel(x + 1, y + 1).G;

                        b1[0] = image.GetPixel(x + 1, y - 1).B;
                        b1[1] = image.GetPixel(x + 1, y).B;
                        b1[2] = image.GetPixel(x + 1, y + 1).B;
                    }

                    r -= r0[0];
                    r += r0[1] + r0[1];
                    r -= r0[2];
                    r -= r1[0];
                    r += r1[1] + r1[1];
                    r -= r1[2];
                    r -= r2[0];
                    r += r2[1] + r2[1];
                    r -= r2[2];

                    g -= g0[0];
                    g += g0[1] + g0[1];
                    g -= g0[2];
                    g -= g1[0];
                    g += g1[1] + g1[1];
                    g -= g1[2];
                    g -= g2[0];
                    g += g2[1] + g2[1];
                    g -= g2[2];

                    b -= b0[0];
                    b += b0[1] + b0[1];
                    b -= b0[2];
                    b -= b1[0];
                    b += b1[1] + b1[1];
                    b -= b1[2];
                    b -= b2[0];
                    b += b2[1] + b2[1];
                    b -= b2[2];

                    processedImage.SetPixel(x, y, Color.FromArgb(
                        1,
                        Math.Clamp(r, 0, 255),
                        Math.Clamp(g, 0, 255),
                        Math.Clamp(b, 0, 255)
                        )
                    );
                }
            }

            return processedImage;
        }

        public Bitmap RobertsOperationI(Bitmap image)
        {
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {

                    if (y == image.Height - 1 || x == image.Width - 1)
                    {
                        image.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                        continue;
                    }
                    Color pixelColor = image.GetPixel(x, y);

                    int pixel = (int)(Math.Pow(Math.Pow(pixelColor.R - image.GetPixel(x + 1, y + 1).R, 2) + Math.Pow(image.GetPixel(x, y + 1).R - image.GetPixel(x + 1, y).R, 2), 0.5));
                    // double pixel = Math.Pow(Math.Pow(pixelColor.R - image.GetPixel(x + 1, y + 1).R, 2) + Math.Pow(image.GetPixel(x, y + 1).R - image.GetPixel(x + 1, y).R, 2), 0.5);
                    //double pixel = Math.Pow(Math.Pow(pixelColor.R - image.GetPixel(x + 1, y + 1).R, 2) + Math.Pow(image.GetPixel(x, y + 1).R - image.GetPixel(x + 1, y).R, 2), 0.5);
                    pixel = Math.Clamp(pixel, 0, 255);

                    image.SetPixel(x, y, Color.FromArgb(pixel, pixel, pixel));
                }
            }

            return image;
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

        public double VariationCoefficientI(Bitmap image, int channel)
        {
            return StandardDeviation(image, channel) / Mean(image, channel);
        }

        public double FlatteningCoefficient(Bitmap image, int channel)
        {
            int[] H = Histogram(image, channel);
            double o = StandardDeviation(image, channel);
            double b = Mean(image, channel);
            double sum = 0;
            for (int m = 0; m < 256; m++)
            {
                sum += Math.Pow((m - b), 4) * H[m] - 3.0;
            }
            return (1.0 / Math.Pow(o, 4)) * (1.0 / (image.Width * image.Height) * sum);
        }

        public double VariationCoefficientII(Bitmap image, int channel)
        {
            int[] H = Histogram(image, channel);
            double sum = 0;
            for (int m = 0; m < 256; m++)
            {
                sum += Math.Pow(H[m], 2);
            }
            return Math.Pow((1.0 / (image.Width * image.Height)), 2) * sum;
        }

        public double InformationSourceEntropy(Bitmap image, int channel)
        {
            int[] H = Histogram(image, channel);
            double sum = 0;
            int N = image.Width * image.Height;
            for (int m = 0; m < 256; m++)
            {
                if (H[m] > 0)
                {
                    sum += H[m] * Math.Log2((double)H[m] / N);
                }
            }
            return (-1.0 / N) * sum;
        }
    }
}
