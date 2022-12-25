using System.Drawing;
using System.Numerics;

namespace ImageProcessing
{
    // Task 4
    internal partial class ImageProcessing
    {
        // Fourier transforms in frequency domain
        public List<List<Complex>> SlowFourierTransform(Bitmap image) 
        {
            Complex[,] result = new Complex[image.Height, image.Width];
            Complex[,] temp = new Complex[image.Height, image.Width];

            for (int row = 0; row < image.Height; row++)
            {
                for (int k = 0; k < image.Width; k++)
                {
                    Complex sum = new Complex(0.0, 0.0);

                    for (int n = 0; n < image.Width; n++)
                    {
                        Complex W = new Complex(Math.Cos(2 * Math.PI * n * k / image.Width), -Math.Sin(2 * Math.PI * n * k / image.Width));
                        sum = sum + image.GetPixel(n, row).R * W;
                    }

                    temp[row, k] = sum;
                }
            }

            for (int col = 0; col < image.Width; col++)
            {
                for (int k = 0; k < image.Height; k++)
                {
                    Complex sum = new Complex(0.0, 0.0);

                    for (int n = 0; n < image.Height; n++)
                    {
                        Complex W = new Complex(Math.Cos(2 * Math.PI * n * k / image.Height), -Math.Sin(2 * Math.PI * n * k / image.Height));
                        sum = sum + temp[n, col] * W;
                    }

                    result[k, col] = sum;
                }
            }

            return Enumerable.Range(0, result.GetLength(0))
            .Select(row => Enumerable.Range(0, result.GetLength(1))
            .Select(col => result[row, col]).ToList()).ToList();
        }

        public List<List<Complex>> SwapQuarters(List<List<Complex>> list)
        {
            List<List<Complex>> result = new List<List<Complex>>();

            for (int x = 0; x < list.Count; x++)
            {
                List<Complex> column = new List<Complex>();

                for (int y = 0; y < list[0].Count; y++)
                {
                    column.Add(list[x][y]);
                }

                result.Add(column);
            }

            for (int x = 0; x < list.Count / 2; x++)
            {
                for (int y = 0; y < list[0].Count / 2; y++)
                {
                    Complex temp = new Complex(result[x][y].Real, result[x][y].Imaginary);
                    result[x][y] = result[list.Count / 2 + x][list[0].Count / 2 + y];
                    result[list.Count / 2 + x][list[0].Count / 2 + y] = temp;

                    temp = new Complex(result[list.Count / 2 + x][y].Real, result[list.Count / 2 + x][y].Imaginary);
                    result[list.Count / 2 + x][y] = result[x][list[0].Count / 2 + y];
                    result[x][list[0].Count / 2 + y] = temp;
                }
            }

            return result;
        }

        private List<Complex> FastFourierTransform1D (List<Complex> list)
        {
            if (list.Count == 1)
            {
                return list;
            }

            List<Complex> result = new List<Complex>(list.Count);

            List<Complex> even = new List<Complex>(list.Count / 2);
            List<Complex> odd = new List<Complex>(list.Count / 2);

            for (int i = 0; i < list.Count / 2; i++)
            {
                even.Add(list[2 * i]);
                odd.Add(list[2 * i + 1]);
            }

            even = FastFourierTransform1D(even);
            odd = FastFourierTransform1D(odd);

            for (int i = 0; i < list.Count; i++)
            {
                result.Add(0);
            }

            for (int i = 0; i < list.Count / 2; i++)
            {
                Complex number = new Complex(Math.Cos(2 * Math.PI * i / list.Count), -Math.Sin(2 * Math.PI * i / list.Count));
                result[i] = even[i] + number * odd[i];
                result[i + list.Count / 2] = even[i] - number * odd[i];
            }

            return result;
        }

        public List<List<Complex>> FastFourierTransform(Bitmap image)
        {
            List<List<Complex>> result = new List<List<Complex>>();
            List<List<Complex>> temp = new List<List<Complex>>();

            for (int row = 0; row < image.Height; row++)
            {
                List<Complex> rowData = new List<Complex>();

                for (int x = 0; x < image.Width; x++)
                {
                    rowData.Add(image.GetPixel(x, row).R);
                }

                temp.Add(FastFourierTransform1D(rowData));
            }

            for (int col = 0; col < image.Width; col++)
            {
                List<Complex> colData = new List<Complex>();

                for (int y = 0; y < image.Height; y++)
                {
                    colData.Add(temp[y][col]);
                }

                result.Add(FastFourierTransform1D(colData));
            }

            return result;
        }

        public Bitmap InverseSlowFourierTransform(List<List<Complex>> result)
        {
            Bitmap outputImage = new Bitmap(result[0].Count, result.Count);
            Complex[,] temp = new Complex[outputImage.Height, outputImage.Width];

            for (int col = 0; col < outputImage.Width; col++)
            {
                for (int k = 0; k < outputImage.Height; k++)
                {
                    Complex sum = new Complex(0.0, 0.0);

                    for (int n = 0; n < outputImage.Height; n++)
                    {
                        Complex W = new Complex(Math.Cos(2 * Math.PI * n * k / outputImage.Height), -Math.Sin(2 * Math.PI * n * k / outputImage.Height));
                        sum = sum + result[n][col] * W;
                    }

                    temp[k, col] = sum / (double)outputImage.Height;
                }
            }

            for (int row = 0; row < outputImage.Height; row++)
            {
                for (int k = 0; k < outputImage.Width; k++)
                {
                    Complex sum = new Complex(0.0, 0.0);

                    for (int n = 0; n < outputImage.Width; n++)
                    {
                        Complex W = new Complex(Math.Cos(2 * Math.PI * n * k / outputImage.Width), -Math.Sin(2 * Math.PI * n * k / outputImage.Width));
                        sum = sum + temp[row, n] * W;
                    }

                    int calculatedColor = (int)Math.Clamp(sum.Magnitude / outputImage.Width, 0, 255);
                    // x and y are shifted by 1 to fix image position problem
                    outputImage.SetPixel(
                        (k - 1 + outputImage.Width) % outputImage.Width,
                        (row - 1 + outputImage.Height) % outputImage.Height,
                        Color.FromArgb(1, calculatedColor, calculatedColor, calculatedColor)
                        );
                }
            }

            return DiagonalFlip(outputImage);
        }

        private List<Complex> InverseFastFourierTransform1D(List<Complex> list)
        {
            if (list.Count == 1)
            {
                return list;
            }

            List<Complex> result = new List<Complex>(list.Count);

            List<Complex> even = new List<Complex>(list.Count / 2);
            List<Complex> odd = new List<Complex>(list.Count / 2);

            for (int i = 0; i < list.Count / 2; i++)
            {
                even.Add(list[2 * i]);
                odd.Add(list[2 * i + 1]);
            }

            even = InverseFastFourierTransform1D(even);
            odd = InverseFastFourierTransform1D(odd);

            for (int i = 0; i < list.Count; i++)
            {
                result.Add(0);
            }

            for (int i = 0; i < list.Count / 2; i++)
            {
                Complex number = new Complex(Math.Cos(2 * Math.PI * i / list.Count), Math.Sin(2 * Math.PI * i / list.Count));
                result[i] = even[i] + number * odd[i];
                result[i + list.Count / 2] = even[i] - number * odd[i];
            }

            return result;
        }

        public Bitmap InverseFastFourierTransform(List<List<Complex>> result)
        {
            Bitmap outputImage = new Bitmap(result[0].Count, result.Count);
            List<List<Complex>> temp = new List<List<Complex>>();

            for (int x = 0; x < outputImage.Height; x++)
            {
                temp.Add(new List<Complex>());
            }

            for (int y = 0; y < outputImage.Height; y++)
            {
                List<Complex> rowData = new List<Complex>();

                for (int x = 0; x < outputImage.Width; x++)
                {
                    rowData.Add(result[y][x]);
                }

                rowData = InverseFastFourierTransform1D(rowData);

                for (int x = 0; x < outputImage.Width; x++)
                {
                    temp[y].Add(rowData[x] / outputImage.Width);
                }
            }

            for (int x = 0; x < outputImage.Width; x++)
            {
                List<Complex> colData = new List<Complex>();

                for (int y = 0; y < outputImage.Width; y++)
                {
                    colData.Add(temp[y][x]);
                }

                colData = InverseFastFourierTransform1D(colData);

                for (int y = 0; y < outputImage.Height; y++)
                {
                    int calculatedColor = (int)Math.Clamp(Math.Abs(colData[y].Magnitude / outputImage.Width), 0, 255);
                    outputImage.SetPixel(x, y, Color.FromArgb(1, calculatedColor, calculatedColor, calculatedColor));
                }

            }

            return outputImage;
        }

        // Visualization of Fourier spectrum
        public Bitmap VisualizationFourierSpectrum(List<List<Complex>> image)
        {
            Bitmap visualizationImage = new Bitmap(image[0].Count, image.Count);
            
            for (int x = 0; x < image[0].Count; x++)
            {
                for (int y = 0; y < image.Count; y++)
                {
                    int calculatedColor = (int)Math.Clamp(Math.Log(Math.Abs(image[y][x].Magnitude )) * 10, 0, 255);

                    visualizationImage.SetPixel(x, y, Color.FromArgb(1, calculatedColor, calculatedColor, calculatedColor));
                }
            }

            return visualizationImage;
        }

        // Filters in frequency domain
        public Bitmap LowpassFilter(Bitmap image, int threshold)
        {
            Bitmap transformedImage = new Bitmap(image.Width, image.Height);

            return transformedImage;
        }

        public Bitmap HighpassFilter(Bitmap image, int threshold)
        {
            Bitmap transformedImage = new Bitmap(image.Width, image.Height);

            return transformedImage;
        }

        public Bitmap BandpassFilter(Bitmap image, int threshold, int width)
        {
            Bitmap transformedImage = new Bitmap(image.Width, image.Height);

            return transformedImage;
        }

        public Bitmap BandcutFilter(Bitmap image, int threshold, int width)
        {
            Bitmap transformedImage = new Bitmap(image.Width, image.Height);

            return transformedImage;
        }

        public Bitmap HighpassFilterWithEdgeDetection(Bitmap image, double alpha, double beta, double radius)
        {
            Bitmap transformedImage = new Bitmap(image.Width, image.Height);

            return transformedImage;
        }

        public Bitmap PhaseModyfingFilter(Bitmap image, double k, double l)
        {
            Bitmap transformedImage = new Bitmap(image.Width, image.Height);

            return transformedImage;
        }
    }
}
