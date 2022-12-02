using System.Drawing;

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
            Bitmap erodedImage = AddPaddding(image, 0);

            for (int x = 1; x < image.Width - 1; x++)
            {
                for (int y = 1; y < image.Height - 1; y++)
                {
                    if (image.GetPixel(x, y).R != 0 || image.GetPixel(x, y).G != 0 || image.GetPixel(x, y).B != 0)
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

                            erodedImage.SetPixel(x + maskX, y + maskY, Color.FromArgb(0, 0, 0));
                        }
                    }
                }
            }

            return erodedImage;
        }

        public Bitmap Opening(Bitmap image, int structuralElementVariant)
        {
            return Dilation(Erosion(image, structuralElementVariant), structuralElementVariant);
        }

        public Bitmap Closing(Bitmap image, int structuralElementVariant)
        {
            return Erosion(Dilation(image, structuralElementVariant), structuralElementVariant);
        }
    }
}
