﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
US: 1.4 (dot)
";
        static ImageProcessing p = new ImageProcessing();

        public static Bitmap LoadImage(string name)
        {
            Bitmap picture = (Bitmap)Bitmap.FromFile(name);
            return picture;
        }

        public static void SaveImage(Bitmap picture, String path)
        {
            picture.Save(path, ImageFormat.Bmp);
        }

        private static void SaveOutput(Bitmap original, Bitmap output, String operation)
        {
            String path = $".\\{ DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{ operation}";
            Directory.CreateDirectory(path);
            SaveImage(original, $"{path}\\original.bmp");
            SaveImage(output, $"{path}\\output.bmp");
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

            Bitmap inputImage1;
            Bitmap inputImage2;
            Bitmap outputPicture;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Clean the memory to calculate used memory more precisely
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var memoryBefore = System.Diagnostics.Process.GetCurrentProcess().VirtualMemorySize64;

            switch (arguments[1])
            {
                case "--brightness":
                    if (arguments.Length != 4 || !Int32.TryParse(arguments[3], out intModifier))
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputImage1 = LoadImage(arguments[2]);
                    outputPicture = p.ModifyBrightness(LoadImage(arguments[2]), intModifier);

                    SaveOutput(inputImage1, outputPicture, "brightness");

                    break;
                case "--contrast":
                    if (arguments.Length != 4 || !float.TryParse(arguments[3], out floatModifier) || floatModifier < 0)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputImage1 = LoadImage(arguments[2]);
                    outputPicture = p.ModifyContrast(LoadImage(arguments[2]), floatModifier);

                    SaveOutput(inputImage1, outputPicture, "contrast");

                    break;
                case "--negative":
                    if (arguments.Length != 3)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputImage1 = LoadImage(arguments[2]);
                    outputPicture = p.Negative(LoadImage(arguments[2]));

                    SaveOutput(inputImage1, outputPicture, "negative");

                    break;
                case "--hflip":
                    if (arguments.Length != 3)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputImage1 = LoadImage(arguments[2]);
                    outputPicture = p.HorizontalFlip(LoadImage(arguments[2]));

                    SaveOutput(inputImage1, outputPicture, "hflip");

                    break;
                case "--vflip":
                    if (arguments.Length != 3)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputImage1 = LoadImage(arguments[2]);
                    outputPicture = p.VerticalFlip(LoadImage(arguments[2]));

                    SaveOutput(inputImage1, outputPicture, "vflip");

                    break;
                case "--dflip":
                    if (arguments.Length != 3)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputImage1 = LoadImage(arguments[2]);
                    outputPicture = p.DiagonalFlip(LoadImage(arguments[2]));

                    SaveOutput(inputImage1, outputPicture, "dflip");

                    break;
                case "--shrink":
                    if (arguments.Length != 4 || !float.TryParse(arguments[3], out floatModifier) || floatModifier < 0)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputImage1 = LoadImage(arguments[2]);
                    outputPicture = p.Resize(LoadImage(arguments[2]), 1 / floatModifier);

                    SaveOutput(inputImage1, outputPicture, "shrink");

                    break;
                case "--enlarge":
                    if (arguments.Length != 4 || !float.TryParse(arguments[3], out floatModifier) || floatModifier < 0)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputImage1 = LoadImage(arguments[2]);
                    outputPicture = p.Resize(LoadImage(arguments[2]), floatModifier);

                    SaveOutput(inputImage1, outputPicture, "enlarge");

                    break;
                case "--median":
                    if (arguments.Length != 4 || !Int32.TryParse(arguments[3], out intModifier) || intModifier < 0)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputImage1 = LoadImage(arguments[2]);
                    outputPicture = p.MedianFilter(LoadImage(arguments[2]), intModifier);

                    SaveOutput(inputImage1, outputPicture, "median");

                    break;
                case "--hmean":
                    if (arguments.Length != 5 || !Int32.TryParse(arguments[3], out intModifier) || !Int32.TryParse(arguments[4], out intModifier2) || intModifier < 0 || intModifier2 < 0)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }
                    inputImage1 = LoadImage(arguments[2]);
                    outputPicture = p.HarmonicFilter(LoadImage(arguments[2]), intModifier, intModifier2);

                    SaveOutput(inputImage1, outputPicture, "harmonic");

                    break;
                case "--mse":
                    if (arguments.Length != 4)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputImage1 = LoadImage(arguments[2]);
                    inputImage2 = LoadImage(arguments[3]);

                    Console.WriteLine($"Mean square error: {p.MeanSquareError(inputImage1, inputImage2)}");

                    break;
                case "--pmse":
                    if (arguments.Length != 4)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputImage1 = LoadImage(arguments[2]);
                    inputImage2 = LoadImage(arguments[3]);

                    Console.WriteLine($"Peak mean square error: {p.PeakMeanSquareError(inputImage1, inputImage2)}");

                    break;
                case "--snr":
                    if (arguments.Length != 4)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputImage1 = LoadImage(arguments[2]);
                    inputImage2 = LoadImage(arguments[3]);

                    Console.WriteLine($"Signal to noise ratio error: {p.SignalToNoiseRatio(inputImage1, inputImage2)}");

                    break;
                case "--psnr":
                    if (arguments.Length != 4)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputImage1 = LoadImage(arguments[2]);
                    inputImage2 = LoadImage(arguments[3]);

                    Console.WriteLine($"Peak signal to noise ratio error: {p.PeakSignalToNoiseRatio(inputImage1, inputImage2)}");

                    break;
                case "--md":
                    if (arguments.Length != 4)
                    {
                        Console.WriteLine(invalidMessage);
                        return;
                    }

                    inputImage1 = LoadImage(arguments[2]);
                    inputImage2 = LoadImage(arguments[3]);

                    Console.WriteLine($"Maximum difference: {p.MaximumDifference(inputImage1, inputImage2)}");

                    break;
                case "--help":
                    Console.WriteLine(helpMessage);
                    break; // return to prevent showing elapsed time
                default:
                    Console.WriteLine(invalidMessage);
                    break;
            }

            stopwatch.Stop();
            var memoryAfter = System.Diagnostics.Process.GetCurrentProcess().VirtualMemorySize64;

            Console.WriteLine("Elapsed time: {0} ms", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Memory usage: {0} MB", Math.Round((decimal)(memoryAfter - memoryBefore) / 1024 / 1024, 2));
        }
    }
}
