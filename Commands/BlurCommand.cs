using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Catel.MVVM;

namespace ConvolutionWpf.Commands
{
    public class BlurCommand : Command
    {
        private readonly Func<WriteableBitmap> _imageFactory;

        public BlurCommand(Func<WriteableBitmap> imageFactory)
            : base(() => { })
        {
            _imageFactory = imageFactory;
        }

        public void ExecuteCommand()
        {
            var image = _imageFactory();
            if (image == null)
                return;

            var pixels = new byte[image.PixelHeight * image.BackBufferStride];
            image.CopyPixels(pixels, image.BackBufferStride, 0);

            var resultPixels = new byte[image.PixelHeight * image.BackBufferStride];

            int matrixSize = 5;

            int matrixValue = 1;

            int[,] matrix = new int[matrixSize, matrixSize];

            for (int row = 0; row < matrixSize; row++)
            {
                for (int column = 0; column < matrixSize; column++)
                {
                    matrix[row,column] = matrixValue;
                }
            }

            int kernelMatrixFormula = (matrixSize - 1) / 2;


            for (int i = kernelMatrixFormula; i < (image.PixelHeight - kernelMatrixFormula); i++)
            {
                for (int j = kernelMatrixFormula; j < (image.PixelWidth - kernelMatrixFormula); j++)
                {
                    int indexOld = i * image.BackBufferStride + 4 * j;

                    for (int c = 0; c < 3; ++c)
                    {
                        int sumOfMatrixElements = 0;

                        for (int k = i - kernelMatrixFormula; k < i + kernelMatrixFormula + 1; k++)
                        {
                            int row = 0;
                            for (int m = j - kernelMatrixFormula; m < j + kernelMatrixFormula + 1; m++)
                            {
                                
                                int column = 0;
                                int elementOfMatrix = (byte) (pixels[(k * image.BackBufferStride + 4 * m) + c] * matrix[row,column]);
                                sumOfMatrixElements += elementOfMatrix;
                                column += 1;
                            }
                            row += 1;
                        }
                        resultPixels[indexOld + c] = (byte)(sumOfMatrixElements / (matrixSize * matrixSize));
                    }


                }
            }

            image.WritePixels(new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight), resultPixels, image.BackBufferStride, 0);
        }


        protected override void Execute(object parameter, bool ignoreCanExecuteCheck)
        {
            ExecuteCommand();
        }
    }
}