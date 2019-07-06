using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using MatrixEssentials;
using Matrix = MatrixEssentials.Matrix;

namespace LaplacianOperator
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            
            var imagePath = args[0];
            var outputImagePath = args[1];
            
            if (!File.Exists(imagePath))
            {
                throw new ArgumentException("Source image not found");
            }

            var imageDirectoryPath = Path.GetDirectoryName(imagePath);
            
            stopwatch.Start();
            
            MakeGrayScale(imagePath, imageDirectoryPath + "/grayscale.png");

            PrintMeasured(stopwatch, "Grayscale");
 
            stopwatch.Restart();
            
            var gaussMatrix = new GaussianFilter.GaussianFilter().Apply(imageDirectoryPath + "/grayscale.png", 3, 0.6f);

            PrintMeasured(stopwatch, "Gauss");
            
            stopwatch.Restart();

            var kernel = new IMatrixData[3][]
            {
                new IMatrixData[]
                {
                    new IntegerNumberMatrixData(1), new IntegerNumberMatrixData(1), new IntegerNumberMatrixData(1),
                },
                new IMatrixData[]
                {
                    new IntegerNumberMatrixData(1), new IntegerNumberMatrixData(-8), new IntegerNumberMatrixData(1),
                },
                new IMatrixData[]
                {
                    new IntegerNumberMatrixData(1), new IntegerNumberMatrixData(1), new IntegerNumberMatrixData(1),
                }
            };
            var kernelMatrix = new Matrix(kernel);
            var convoluted = gaussMatrix.Convolute(kernelMatrix);
            
            PrintMeasured(stopwatch, "Convolution");
            
            stopwatch.Restart();
            
            SecondPass(convoluted);
            
            PrintMeasured(stopwatch, "Second pass");
            
            stopwatch.Restart();
            
            var endResult = MatrixUtils.ConvertMatrixToRGBMatrix(convoluted);
            
            MatrixUtils.CreateImageFromMatrix(endResult, outputImagePath);
            
            PrintMeasured(stopwatch, "To image");
            
            stopwatch.Stop();

            //clean garbage
            File.Delete(imageDirectoryPath + "/grayscale.png");
        }

        static void PrintMeasured(Stopwatch stopwatch, string printMessage)
        {
            Console.WriteLine(printMessage);
            Console.WriteLine(stopwatch.ElapsedMilliseconds + " milliseconds");
            Console.WriteLine();
        }

        //https://web.archive.org/web/20110827032809/http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
        static void MakeGrayScale(string imagePath, string grayscaleImagePath)
        {
            var original = Bitmap.FromFile(imagePath);
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
                new float[][]
                {
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            newBitmap.Save(grayscaleImagePath);
            newBitmap.Dispose();
        }

        static void SecondPass(IMatrix matrix)
        {
            var highestValue = matrix.HighestValue;

            for (int i = 0; i < matrix.Height; i++)
            {
                for (int j = 0; j < matrix.Width; j++)
                {
                    var matrixValue = matrix.GetValue(j, i);
                    matrixValue = matrixValue.MultiplyBy(new FloatNumberMatrixData(255f).Divide(highestValue));
                    matrix.SetValue(j, i, matrixValue);
                }
            }
        }
    }
}