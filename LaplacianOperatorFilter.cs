using System.IO;
using MatrixEssentials;

namespace LaplacianOperator
{
    public class LaplacianOperatorFilter
    {
        /// <summary>
        /// Applies laplacian operator to image
        /// </summary>
        /// <param name="imagePath">input image path</param>
        /// <param name="outputImagePath">output image path.</param>
        public void Apply(string imagePath, string outputImagePath)
        {
            var endResult = this.Apply(imagePath);
            var endResultRGBMatrix = MatrixUtils.ConvertMatrixToRGBMatrix(endResult);
            MatrixUtils.CreateImageFromMatrix(endResultRGBMatrix, outputImagePath);
        }

        /// <summary>
        /// Applies laplacian operator to image
        /// </summary>
        /// <param name="imagePath">input image path</param>
        /// <returns>matrix made from original image matrix and applied laplacian operator on it</returns>
        public IMatrix Apply(string imagePath)
        {
            var imageDirectoryPath = Path.GetDirectoryName(imagePath);
            
            var gaussMatrix = new GaussianFilter.GaussianFilter().Apply(imagePath, 3, 0.5f);

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

            SecondPass(convoluted);
            
            return convoluted;
        }
        
        private void SecondPass(IMatrix matrix)
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