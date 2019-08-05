using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
//using System.Drawing.Imaging;
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

            new LaplacianOperatorFilter().Apply(imagePath, outputImagePath);
        }
    }
}