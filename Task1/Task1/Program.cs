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
            picture = p.resizeImage(picture, 8);
            p.SavePicture(picture);
        }

    }
}