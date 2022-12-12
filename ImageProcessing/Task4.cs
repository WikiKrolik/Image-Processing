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
            List<List<Complex>> result = new List<List<Complex>>();

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
        public Bitmap VisualizationFourierSpectrum(Bitmap image)
        {
            Bitmap visualizationImage = new Bitmap(image.Width, image.Height);

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
