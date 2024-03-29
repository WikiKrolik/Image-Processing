﻿using System.Drawing;

namespace ImageProcessing
{
    // Task 3
    internal partial class ImageProcessing
    {
        // structural elements, where:
        // 1 represents dark spot
        // 0 represents white spot
        // -1 represents inactive points (skip)
        // 
        // center of each structural elements is at point (1, 1)

        int[,,] structuralElements = {
            { // [0] - I
                { -1, -1, -1 },
                { -1,  1,  1 },
                { -1, -1, -1 }
            },
            { // [1] - II
                { -1, -1, -1 },
                { -1,  1, -1 },
                { -1,  1, -1 }
            },
            { // [2] - III
                {  1,  1,  1 },
                {  1,  1,  1 },
                {  1,  1,  1 }
            },
            { // [3] - IV
                { -1,  1, -1 },
                {  1,  1,  1 },
                { -1,  1, -1 }
            },
            { // [4] - V
                { -1, -1, -1 },
                { -1,  1,  1 },
                { -1,  1, -1 }
            },
            { // [5] - VI
                { -1, -1, -1 },
                { -1,  0,  1 },
                { -1,  1, -1 }
            },
            { // [6] - VII
                { -1, -1, -1 },
                {  1,  1,  1 },
                { -1, -1, -1 }
            },
            { // [7] - VIII
                { -1, -1, -1 },
                {  1,  0,  1 },
                { -1, -1, -1 }
            },
            { // [8] - IX
                { -1, -1, -1 },
                {  1,  1, -1 },
                {  1, -1, -1 }
            },
            { // [9] - X
                { -1,  1,  1 },
                { -1,  1, -1 },
                { -1, -1, -1 }
            },
            { // [10] - XI.1
                {  1, -1, -1 },
                {  1,  0, -1 },
                {  1, -1, -1 }
            },
            { // [11] - XI.2
                {  1,  1,  1 },
                { -1,  0, -1 },
                { -1, -1, -1 }
            },
            { // [12] - XI.3
                { -1, -1,  1 },
                { -1,  0,  1 },
                { -1, -1,  1 }
            },
            { // [13] - XI.4
                { -1, -1, -1 },
                { -1,  0, -1 },
                {  1,  1,  1 }
            },
            { // [14] - XII.1
                {  0,  0,  0 },
                { -1,  1, -1 },
                {  1,  1,  1 }
            },
            { // [15] - XII.2
                { -1,  0,  0 },
                {  1,  1,  0 },
                {  1,  1, -1 }
            },
            { // [16] - XII.3
                { 1,  -1,  0 },
                { 1,   1,  0 },
                { 1,  -1,  0 }
            },
            { // [17] - XII.4
                {  1,  1, -1 },
                {  1,  1,  0 },
                { -1,  0,  0 }
            },
            { // [18] -  XII.5
                {  1,  1,  1 },
                { -1,  1, -1 },
                {  0,  0,  0 }
            },
            { // [19] - XII.6
                { -1,  1,  1 },
                {  0,  1,  1 },
                {  0,  0, -1 }
            },
            { // [20] - XII.7
                {  0, -1,  1 },
                {  0,  1,  1 },
                {  0, -1,  1 }
            },
            { // [21] - XII.8
                {  0,  0, -1 },
                {  0,  1,  1 },
                { -1,  1,  1 }
            },
        };

        public Bitmap Dilation(Bitmap image, int structuralElementVariant)
        {
            Bitmap dilatedImage = AddPaddding(image, 0);

            for (int x = 1; x < image.Width - 1; x++)
            {
                for (int y = 1; y < image.Height - 1; y++)
                {
                    if (image.GetPixel(x, y).R != 255 || image.GetPixel(x, y).G != 255 || image.GetPixel(x, y).B != 255)
                    {
                        continue;
                    }

                    for (int maskX = -1; maskX < 2; maskX++)
                    {
                        for (int maskY = -1; maskY < 2; maskY++)
                        {
                            if (structuralElements[structuralElementVariant, maskY + 1, maskX + 1] == -1)
                            {
                                continue;
                            }

                            dilatedImage.SetPixel(x + maskX, y + maskY, Color.FromArgb(255, 255, 255));
                        }
                    }
                }
            }

            return dilatedImage;
        }

        public Bitmap Erosion(Bitmap image, int structuralElementVariant)
        {
            Bitmap erodedImage = new Bitmap(image.Width, image.Height);

            for (int x = 1; x < image.Width - 1; x++)
            {
                for (int y = 1; y < image.Height - 1; y++)
                {
                    bool isFullyContained = true;

                    for (int maskX = -1; maskX < 2; maskX++)
                    {
                        for (int maskY = -1; maskY < 2; maskY++)
                        {
                            if (structuralElements[structuralElementVariant, maskY + 1, maskX + 1] == -1)
                            {
                                continue;
                            }

                            if (structuralElements[structuralElementVariant, maskY + 1, maskX + 1] * 255 != image.GetPixel(maskX + x, maskY + y).R)
                            {
                                isFullyContained = false;
                                break;
                            }
                        }
                    }

                    if (isFullyContained)
                    {
                        erodedImage.SetPixel(x, y, Color.White);
                    }
                }
            }

            return erodedImage;
        }

        public Bitmap Intersection(Bitmap image1, Bitmap image2)
        {
            int height = image1.Height;
            int width = image1.Width;

            Bitmap result = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (image1.GetPixel(x,y).R == 255 && image2.GetPixel(x, y).R == 255)
                    {
                        result.SetPixel(x, y, Color.White);
                    }
                    else { 
                        result.SetPixel(x, y, Color.Black);
                    }
                }
            }
            return result;
        }

        public Bitmap Sum(Bitmap image1, Bitmap image2)
        {
            int height = image1.Height;
            int width = image1.Width;

            Bitmap result = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (image1.GetPixel(x, y).R == 0 && image2.GetPixel(x, y).R == 0)
                    {
                        result.SetPixel(x, y, Color.Black);
                    }
                    else 
                    {
                        result.SetPixel(x, y, Color.White);
                    }
                        
                }
            }
            return result;
        }
        public Bitmap Complement(Bitmap image)
        {
            int height = image.Height;
            int width = image.Width;

            Bitmap result = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int newPixel = 255 - pixel.R;
                    result.SetPixel(x, y, Color.FromArgb(newPixel, newPixel, newPixel));
                }
            }
            return result;
        }

        public Bitmap Opening(Bitmap image, int structuralElementVariant)
        {
            return Dilation(Erosion(image, structuralElementVariant), structuralElementVariant);
        }

        public Bitmap Closing(Bitmap image, int structuralElementVariant)
        {
            return Erosion(Dilation(image, structuralElementVariant), structuralElementVariant);
        }

        public Bitmap HitOrMiss(Bitmap image, int structuralElementVariant)
        {
            Bitmap transformedImage = new Bitmap(image.Width, image.Height);

            for (int x = 1; x < image.Width - 1; x++)
            {
                for (int y = 1; y < image.Height - 1; y++)
                {
                    bool HMTSatisfied = true;

                    for (int maskX = -1; maskX < 2; maskX++)
                    {
                        for (int maskY = -1; maskY < 2; maskY++)
                        {
                            if (structuralElements[structuralElementVariant, maskY + 1, maskX + 1] == -1)
                            {
                                continue;
                            }

                            if (structuralElements[structuralElementVariant, maskY + 1, maskX + 1] * 255 != image.GetPixel(x + maskX, y + maskY).R)
                            {
                                HMTSatisfied = false;
                            }
                        }
                    }

                    transformedImage.SetPixel(x, y, HMTSatisfied ? Color.White : Color.Black);
                }
            }

            return transformedImage;
        }

        public bool Comparison(Bitmap image1, Bitmap image2)
        {
            if (image1.Width != image2.Width || image1.Height != image2.Height)
            {
                return false;
            }


            for (int x = 0; x < image1.Width; x++)
            {
                for (int y = 0; y < image1.Height; y++)
                {
                    Color color1 = image1.GetPixel(x, y);
                    Color color2 = image2.GetPixel(x, y);

                    if (color1.R != color2.R || color1.G != color2.G || color1.B != color2.B)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public Bitmap M2(Bitmap image, int structuralElementVariant, int x, int y)
        {
            int width = image.Width;
            int height = image.Height;

            int iteration = 1;

            Bitmap result = new Bitmap(width, height);
            Bitmap previousResult;
            Bitmap imageComplement = Complement(image);


            result.SetPixel(x, y, Color.White);

            do
            {
                previousResult = (Bitmap)result.Clone();
                result = Intersection(Dilation(previousResult, structuralElementVariant), imageComplement);

                Console.WriteLine($"Iteration {iteration++}");

            } while (!Comparison(result, previousResult));

            return Sum(image, result);
        }

        public Bitmap RegionGrowing(Bitmap image, int x, int y, int threshold, int neigborhoodType, int shouldBeOverlayed)
        {
            Bitmap result = new Bitmap(image.Width, image.Height);

            if (shouldBeOverlayed == 1)
            {
                result = (Bitmap)image.Clone();
            }

            Queue<Point> queue = new Queue<Point>();
            bool[,] processed = new bool[image.Width, image.Height];
            queue.Enqueue(new Point(x, y));

            int minViableValue = Math.Clamp(image.GetPixel(x, y).R - threshold, 0, 255);
            int maxViableValue = Math.Clamp(image.GetPixel(x, y).R + threshold, 0, 255);

            while (queue.Count > 0)
            {
                Point point = queue.Dequeue();
                if (point.X >= 0 && point.X < image.Width && point.Y >= 0 && point.Y < image.Height && !processed[point.X, point.Y])
                {
                    processed[point.X, point.Y] = true;

                    if (image.GetPixel(point.X, point.Y).R >= minViableValue && image.GetPixel(point.X, point.Y).R < maxViableValue)
                    {
                        result.SetPixel(point.X, point.Y, Color.White);

                        switch (neigborhoodType)
                        {
                            case 0:
                                queue.Enqueue(new Point(point.X - 1, point.Y));
                                queue.Enqueue(new Point(point.X, point.Y + 1));
                                queue.Enqueue(new Point(point.X, point.Y - 1));
                                queue.Enqueue(new Point(point.X + 1, point.Y));

                                break;
                            case 1:
                                queue.Enqueue(new Point(point.X - 1, point.Y));
                                queue.Enqueue(new Point(point.X, point.Y + 1));
                                queue.Enqueue(new Point(point.X, point.Y - 1));
                                queue.Enqueue(new Point(point.X + 1, point.Y));
                                queue.Enqueue(new Point(point.X - 1, point.Y - 1));
                                queue.Enqueue(new Point(point.X - 1, point.Y + 1));
                                queue.Enqueue(new Point(point.X + 1, point.Y - 1));
                                queue.Enqueue(new Point(point.X + 1, point.Y + 1));

                                break;
                        }
                    }
                }
            }

            return result;
        }
    }
}
