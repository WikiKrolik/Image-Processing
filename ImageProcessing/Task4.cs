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
            List<List<Complex>> temp = new List<List<Complex>>();
            List<List<Complex>> result = new List<List<Complex>>();

            for (int y = 0; y < image.Height; y++)
            {
                List<Complex> row = new List<Complex>(image.Width);
                temp.Add(row);

                for (int x = 0; x < image.Width; x++)
                {
                    Complex sum = new Complex(0.0, 0.0);

                    for (int xx = 0; xx < image.Width; xx++)
                    {
                        Complex comp = new Complex(Math.Cos(2 * Math.PI * xx * x / image.Width), -Math.Sin(2 * Math.PI * xx * x / image.Width));
                        sum = Complex.Add(sum, Complex.Multiply((double)image.GetPixel(xx, y).R, comp));
                    }

                    temp[y].Add(sum);
                }
            }

            for (int y = 0; y < image.Height; y++)
            {
                List<Complex> col = new List<Complex>(image.Width);
                result.Add(col);

                for (int x = 0; x < image.Width; x++)
                {
                    Complex sum = new Complex(0.0, 0.0);

                    for (int yy = 0; yy < image.Width; yy++)
                    {
                        Complex comp = new Complex(Math.Cos(2 * Math.PI * yy * y / image.Width), -Math.Sin(2 * Math.PI * yy * y / image.Width));
                        sum = Complex.Add(sum, Complex.Multiply((double)image.GetPixel(yy, x).R, comp));
                    }

                    result[y].Add(sum);
                }
            }

            return result;
        }

        public List<List<Complex>> FastFourierTransform(Bitmap image)
        {
            List<List<Complex>> result = new List<List<Complex>>();

            return result;
        }

        public Bitmap InverseSlowFourierTransform(List<List<Complex>> result)
        {
            Bitmap outputImage = new Bitmap(result[0].Count, result.Count);

            return outputImage;
        }

        public Bitmap InverseFastFourierTransform(List<List<Complex>> result)
        {
            Bitmap outputImage = new Bitmap(result[0].Count, result.Count);

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
                    int calculatedColor = (int)Math.Clamp(Math.Log(Math.Abs(image[x][y].Magnitude * 15.0)) * 10, 0, 255);

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
