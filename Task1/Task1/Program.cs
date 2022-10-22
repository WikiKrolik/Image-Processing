﻿using System;
using System.Drawing;

namespace ImageProcessing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CommandProcessing.start();

            /* 
            var p = new ImageProcessing();
            

            string name = "C://test//lenac.bmp";
            Bitmap picture = p.LoadPicture(name);

            // picture = p.resizeImage(picture, 1);
            // p.SavePicture(picture);

            Bitmap picOriginal = p.LoadPicture("C://test//lenna512.png");
            Bitmap picNoise = p.LoadPicture("C://test//lenna512noise.png");
            Bitmap picFiltered = p.MedianFilter(picNoise, 1);

            Console.WriteLine(p.maximumDifference(picOriginal, picNoise));
            Console.WriteLine(p.maximumDifference(picOriginal, picFiltered));

            Console.WriteLine(p.meanSquareError(picOriginal, picNoise));
            Console.WriteLine(p.meanSquareError(picOriginal, picFiltered));
            */
        }
    }
}