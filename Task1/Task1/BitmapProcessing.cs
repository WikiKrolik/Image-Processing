using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
    internal class BitmapProcessing
    {

        public Bitmap LoadPicture(string name)
        {
            Bitmap picture = (Bitmap)Bitmap.FromFile(name);
            return picture;
        }

        public void SavePicture(Bitmap picture)
        {
            picture.Save("C://test//Light.png", ImageFormat.Png);
        }

        public Bitmap ModifyBrightness(Bitmap picture, int brightness)
        {
            for (int x = 0; x < picture.Width; x++)
            {
                for (int y = 0; y < picture.Height; y++)
                {
                    Color pixelColor = picture.GetPixel(x, y);

                    int r = brightness + pixelColor.R;
                    int g = brightness + pixelColor.G;
                    int b = brightness + pixelColor.B;

                    r = Math.Clamp(r, 0, 255);
                    g = Math.Clamp(g, 0, 255);
                    b = Math.Clamp(b, 0, 255);

                    picture.SetPixel(x, y, Color.FromArgb(pixelColor.A, r, g, b));
                }

            }
            return picture;
        }

        /*public Bitmap ModifyContrast(Bitmap picture, int contrast)
        {
            for (int x = 0; x < picture.Width; x++)
            {
                for (int y = 0; y < picture.Height; y++)
                {
                    Color pixelColor = picture.GetPixel(x, y);
                    double v =  Math.Pow((100.0 + contrast) / 100.0, 2);

                    int r = (int)((pixelColor.R / 255 - 0.5) * v + 0.5) * 255;
                    int g = (int)((pixelColor.G / 255 - 0.5) * v + 0.5) * 255;
                    int b = (int)((pixelColor.B / 255 - 0.5) * v + 0.5) * 255;

                    r = Math.Clamp(r, 0, 255);
                    g = Math.Clamp(g, 0, 255);
                    b = Math.Clamp(b, 0, 255);

                    picture.SetPixel(x, y, Color.FromArgb(pixelColor.A, r, g, b));
                }

            }
            return picture;
        }*/
    }
}

