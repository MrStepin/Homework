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

            for (int i = 1; i < (image.PixelHeight -1); i++)
            {
                for (int j = 1; j < (image.PixelWidth -1); j++)
                {
                    int indexOld = i * image.BackBufferStride + 4 * j;

                    for (int c = 0; c < 3; ++c)
                    {
                        int resultIndex = 0;

                        for (int k = i - 1; k < i + 2; k++)
                        {
                            for (int m = j - 1; m < j + 2; m++)
                            {
                                int KernelIndex = (byte) (pixels[(k * image.BackBufferStride + 4 * m) + c] *1);
                                resultIndex += KernelIndex;
                            }
                        }
                        resultPixels[indexOld + c] = (byte)(resultIndex/ 9);
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