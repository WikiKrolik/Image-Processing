using System;
using System.Drawing;

namespace ImageProcessing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var p = new ImageProcessing();
            string name = "C://test//lenac_small.bmp";
            Bitmap picture = p.LoadPicture(name);
            picture = p.VerticalFlip(picture);
            p.SavePicture(picture);

        }

    }
}