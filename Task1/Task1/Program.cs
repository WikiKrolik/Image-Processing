using System;
using System.Drawing;

namespace ImageProcessing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var p = new ImageProcessing();
            string name = "C://test//lenac.bmp";
            Bitmap picture = p.LoadPicture(name);

            // picture = p.resizeImage(picture, 1);
            // p.SavePicture(picture);

            // Bitmap picture1 = p.LoadPicture("C://test//lenna512.png");
            // Bitmap picture2 = p.LoadPicture("C://test//lenna512noise.png");
            // Bitmap picture3 = p.ArithmeticMean(picture2);

            // Console.WriteLine(p.meanSquareError(picture1, picture2));
            // Console.WriteLine(p.meanSquareError(picture1, picture3));

            // p.SavePicture(picture3);
        }
    }
}