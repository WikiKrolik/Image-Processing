using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
    internal class CommandProcessing
    {
        const string invalidMessage = "Invalid parameters. Use --help command.";
        const string helpMessage =
@"Available operations:
--brightness <path:string> <modifier:int>
  Increase or decrease the brightness of an image.

--contrast <path:string> <modifier:float (0 < mod < 1 to decrease, mod > 1 to increase contrast)>
  Increase or decrease the contrast of an image.

--negative <path:string>
  Invert the colors of an image.

--hflip <path:string>
  Flip an image horizontally.

--vflip <path:string>
  Flip an image vertically.

--dflip <path:string>
  Flip an image diagonally.

--shrink <path:string> <modifier:float (mod > 1)>
  Decrease the size of an image.

--enlarge <path:string> <modifier:float (mod > 1)>
  Increase the size of an image.

--median <path:string> <radius:int (rad >= 0)>
  Apply median filter denoising to an image.

--hmean <path:string> <radius:int (rad >= 0)>
  Apply harmonic mean filter denoising to an image.

--mse <path1:string> <path2:string>
  Calculate mean square error between two images.

--pmse <path1:string> <path2:string>
  Calculate peak mean square error between two images.

--snr <path1:string> <path2:string>
  Calculate signal to noise ratio between two images.

--psnr <path1:string> <path2:string>
  Calculate peak signal to noise ratio between two images.

--md <path1:string> <path2:string>
  Calculate maximum differece between two images.

Remember that the float needs to be passed accordingly to your system localization, e.g.:
POL: 1,4 (comma)
US: 1.4 (dot)";
        static ImageProcessing p = new ImageProcessing();

        private static void saveOutput(Bitmap original, Bitmap output, String operation)
        {
            String path = $".\\{ DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{ operation}";
            Directory.CreateDirectory(path);
            p.SavePicture(original, $"{path}\\original.bmp");
            p.SavePicture(output, $"{path}\\output.bmp");
        }

        public static void start()
        {
            string[] arguments = Environment.GetCommandLineArgs();

            if (arguments.Length < 2)
            {
                Console.WriteLine(invalidMessage);
                return;
            }
            int intModifier = 0;
            int intModifier2 = 0;
            float floatModifier = 0;

            Bitmap inputPicture1;
            Bitmap inputPicture2;
            Bitmap outputPicture;

            switch (arguments[1])
            {
                case "--brightness":
                    if (arguments.Length != 4 || !Int32.TryParse(arguments[3], out intModifier))
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputPicture1 = p.LoadPicture(arguments[2]);
                    outputPicture = p.ModifyBrightness(p.LoadPicture(arguments[2]), intModifier);

                    saveOutput(inputPicture1, outputPicture, "brightness");

                    break;
                case "--contrast":
                    if (arguments.Length != 4 || !float.TryParse(arguments[3], out floatModifier) || floatModifier < 0)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputPicture1 = p.LoadPicture(arguments[2]);
                    outputPicture = p.ModifyContrast(p.LoadPicture(arguments[2]), floatModifier);

                    saveOutput(inputPicture1, outputPicture, "contrast");

                    break;
                case "--negative":
                    if (arguments.Length != 3)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputPicture1 = p.LoadPicture(arguments[2]);
                    outputPicture = p.Negative(p.LoadPicture(arguments[2]));

                    saveOutput(inputPicture1, outputPicture, "negative");

                    break;
                case "--hflip":
                    if (arguments.Length != 3)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputPicture1 = p.LoadPicture(arguments[2]);
                    outputPicture = p.HorizontalFlip(p.LoadPicture(arguments[2]));

                    saveOutput(inputPicture1, outputPicture, "hflip");

                    break;
                case "--vflip":
                    if (arguments.Length != 3)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputPicture1 = p.LoadPicture(arguments[2]);
                    outputPicture = p.VerticalFlip(p.LoadPicture(arguments[2]));

                    saveOutput(inputPicture1, outputPicture, "vflip");

                    break;
                case "--dflip":
                    if (arguments.Length != 3)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputPicture1 = p.LoadPicture(arguments[2]);
                    outputPicture = p.DiagonalFlip(p.LoadPicture(arguments[2]));

                    saveOutput(inputPicture1, outputPicture, "dflip");

                    break;
                case "--shrink":
                    if (arguments.Length != 4 || !float.TryParse(arguments[3], out floatModifier) || floatModifier < 0)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputPicture1 = p.LoadPicture(arguments[2]);
                    outputPicture = p.Resize(p.LoadPicture(arguments[2]), 1 / floatModifier);

                    saveOutput(inputPicture1, outputPicture, "shrink");

                    break;
                case "--enlarge":
                    if (arguments.Length != 4 || !float.TryParse(arguments[3], out floatModifier) || floatModifier < 0)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputPicture1 = p.LoadPicture(arguments[2]);
                    outputPicture = p.Resize(p.LoadPicture(arguments[2]), floatModifier);

                    saveOutput(inputPicture1, outputPicture, "enlarge");

                    break;
                case "--median":
                    if (arguments.Length != 4 || !Int32.TryParse(arguments[3], out intModifier) || intModifier < 0)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputPicture1 = p.LoadPicture(arguments[2]);
                    outputPicture = p.MedianFilter(p.LoadPicture(arguments[2]), intModifier);

                    saveOutput(inputPicture1, outputPicture, "median");

                    break;
                case "--hmean":
                    if (arguments.Length != 5 || !Int32.TryParse(arguments[3], out intModifier) || !Int32.TryParse(arguments[4], out intModifier2) || intModifier < 0 || intModifier2 < 0)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }
                    inputPicture1 = p.LoadPicture(arguments[2]);
                    outputPicture = p.HarmonicFilter(p.LoadPicture(arguments[2]), intModifier, intModifier2);

                    saveOutput(inputPicture1, outputPicture, "harmonic");

                    break;
                case "--mse":
                    inputPicture1 = p.LoadPicture(arguments[2]);
                    inputPicture2 = p.LoadPicture(arguments[3]);

                    Console.WriteLine($"Mean square error: {p.meanSquareError(inputPicture1, inputPicture2)}");

                    break;
                case "--pmse":
                    inputPicture1 = p.LoadPicture(arguments[2]);
                    inputPicture2 = p.LoadPicture(arguments[3]);

                    Console.WriteLine($"Peak mean square error: {p.PeakMeanSquareError(inputPicture1, inputPicture2)}");

                    break;
                case "--snr":
                    Console.WriteLine("Not implemented");
                    
                    break;
                case "--psnr":
                    Console.WriteLine("Not implemented");
                    
                    break;
                case "--md":
                    inputPicture1 = p.LoadPicture(arguments[2]);
                    inputPicture2 = p.LoadPicture(arguments[3]);

                    Console.WriteLine($"Maximum difference: {p.maximumDifference(inputPicture1, inputPicture2)}");

                    break;
                case "--help":
                    Console.WriteLine(helpMessage);

                    break;
                default:
                    Console.WriteLine(invalidMessage);
                    break;
            }
        }
    }
}
