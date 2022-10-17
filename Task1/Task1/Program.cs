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

            Bitmap picture1 = p.LoadPicture("C://test//lenna512noise.png");
            p.SavePicture(p.MedianFilter(picture1, 1));
        }
    }
}