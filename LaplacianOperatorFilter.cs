using System.IO;
using System.Threading.Tasks;
using MatrixEssentials;
using MatrixEssentials.Arithmetics;

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
            MatrixUtils.CreateImageFromMatrixParalleled(endResultRGBMatrix, outputImagePath);
        }

        /// <summary>
        /// Applies laplacian operator to image
        /// </summary>
        /// <param name="imagePath">input image path</param>
        /// <returns>matrix made from original image matrix and applied laplacian operator on it</returns>
        public IMatrix Apply(string imagePath)
        {
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
            var convoluted = gaussMatrix.ConvoluteParalleled(kernelMatrix);

            SecondPass(convoluted);
            
            return convoluted;
        }
        
        private void SecondPass(IMatrix matrix)
        {
            var highestValue = matrix.HighestValue;
            var floatArithmeticsController = new FloatNumberMatrixDataArithmetics();
            var unsafeRGBarithmeticsController = highestValue.GetArithmeticsController();

            Parallel.For(0, matrix.Height, (int i) =>
            {
                for (int j = 0; j < matrix.Width; j++)
                {
                    var matrixValue = matrix.GetValue(j, i);
                    matrixValue = unsafeRGBarithmeticsController.Multiply(matrixValue, floatArithmeticsController.Divide(new FloatNumberMatrixData(255), highestValue));
                    matrix.SetValue(j, i, matrixValue);
                } 
            });
        }
    }
}
