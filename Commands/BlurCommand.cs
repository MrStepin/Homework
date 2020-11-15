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

            for (int i = (matrixSize -1)/2; i < (image.PixelHeight - (matrixSize - 1) / 2); i++)
            {
                for (int j = (matrixSize - 1) / 2; j < (image.PixelWidth - (matrixSize - 1) / 2); j++)
                {
                    int indexOld = i * image.BackBufferStride + 4 * j;

                    for (int c = 0; c < 3; ++c)
                    {
                        int sumOfMatrixElements = 0;

                        for (int k = i - (matrixSize - 1) / 2; k < i + ((matrixSize -1)/2) +1; k++)
                        {
                            for (int m = j - (matrixSize - 1) / 2; m < j + ((matrixSize - 1)/2) + 1; m++)
                            {
                                int elementOfMatrix = (byte) (pixels[(k * image.BackBufferStride + 4 * m) + c] * matrixValue);
                                sumOfMatrixElements += elementOfMatrix;
                            }
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