using System;
using System.Collections.Generic;
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
            var imagePath = args[0];
            var outputImagePath = args[1];
            
            if (!File.Exists(imagePath))
            {
                throw new ArgumentException("Source image not found");
            }

            var imageDirectoryPath = Path.GetDirectoryName(imagePath);

            Console.WriteLine();
            
            new GaussianFilter.GaussianFilter().Apply(imagePath, imageDirectoryPath + "/gauss.png", 7, 0.6f);

            MakeGrayScale(imageDirectoryPath + "/gauss.png", imageDirectoryPath + "/gauss-grayscale.png");
            
            var gaussMatrix = MatrixUtils.CreateMatrixFromImage(imageDirectoryPath + "/gauss-grayscale.png");
            var kernel = new List<IList<IMatrixData>>()
            {
                new[]
                {
                    new FloatNumberMatrixData(1), new FloatNumberMatrixData(1), new FloatNumberMatrixData(1),
                },
                new[]
                {
                    new FloatNumberMatrixData(1), new FloatNumberMatrixData(-8), new FloatNumberMatrixData(1),
                },
                new[]
                {
                    new FloatNumberMatrixData(1), new FloatNumberMatrixData(1), new FloatNumberMatrixData(1),
                }
            };
            var kernelMatrix = new Matrix(kernel);
            var convoluted = gaussMatrix.Convolute(kernelMatrix);
            
            SecondPass(convoluted);
            
            var endResult = MatrixUtils.ConvertMatrixToRGBMatrix(convoluted);

            MatrixUtils.CreateImageFromMatrix(endResult, outputImagePath);
            
            //clean garbage
            File.Delete(imageDirectoryPath + "gauss.png");
            File.Delete(imageDirectoryPath + "gauss-grayscale.png");
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