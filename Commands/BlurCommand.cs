using System;
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

                    int KernelIndex5 = i * image.BackBufferStride + 4 * j;
                    int KernelIndex1 = (i - 1) * image.BackBufferStride + 4 * (j - 1);
                    int KernelIndex2 = (i - 1) * image.BackBufferStride + 4 * j;
                    int KernelIndex3 = (i - 1) * image.BackBufferStride + 4 * (j + 1);
                    int KernelIndex4 = i * image.BackBufferStride + 4 * (j - 1);
                    int KernelIndex6 = i * image.BackBufferStride + 4 * (j + 1);
                    int KernelIndex7 = (i + 1) * image.BackBufferStride + 4 * (j - 1);
                    int KernelIndex8 = (i + 1) * image.BackBufferStride + 4 * j ;
                    int KernelIndex9 = (i + 1) * image.BackBufferStride + 4 * (j + 1);

                    for (int c = 0; c < 3; ++c)
                    {
                        resultPixels[indexOld + c] = (byte)((pixels[KernelIndex1 + c] * 1 + pixels[KernelIndex2 + c] * 1 + pixels[KernelIndex3 + c] * 1 +
                                      pixels[KernelIndex4 + c] * 1 + pixels[KernelIndex5 + c] * 1 + pixels[KernelIndex6 + c] * 1 +
                                      pixels[KernelIndex7 + c] * 1 + pixels[KernelIndex8 + c] * 1 + pixels[KernelIndex9 + c] * 1) / 9);
                    }
                    resultPixels[indexOld + 3] = (byte)((pixels[KernelIndex1 + 3] * 1 + pixels[KernelIndex2 + 3] * 1 + pixels[KernelIndex3 + 3] * 1 +
                                      pixels[KernelIndex4 + 3] * 1 + pixels[KernelIndex5 + 3] * 1 + pixels[KernelIndex6 + 3] * 1 +
                                      pixels[KernelIndex7 + 3] * 1 + pixels[KernelIndex8 + 3] * 1 + pixels[KernelIndex9 + 3] * 1) / 9);

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