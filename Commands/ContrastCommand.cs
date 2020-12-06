
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Catel.MVVM;

namespace ConvolutionWpf.Commands
{
    public class ContrastCommand : Command
    {
        private readonly Func<WriteableBitmap> _imageFactory;

        public ContrastCommand(Func<WriteableBitmap> imageFactory)
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

            int[] histogram = Enumerable.Range(0, 256).ToArray();

            int pixelsCount = 3 * image.PixelWidth * image.PixelHeight;

                for (int i = 0; i < image.PixelWidth; ++i)
                {
                    for (int j = 0; j < image.PixelHeight; ++j)
                    {
                        int index = j * image.BackBufferStride + i * 4;

                        for (int c = 0; c < 3; ++c)
                        {
                            histogram[pixels[index + c]] = histogram[pixels[index + c]]+1;
                        
                        }
                    }
                }

            int previousValue = 0;
            for (int histogramIndex = 0; histogramIndex < 256; histogramIndex++)
            {

                histogram[histogramIndex] = histogram[histogramIndex] + previousValue;
                previousValue = histogram[histogramIndex];

            }

            int minValue = 0;
            int maxValue = 0;

            for (int i = 0; i < 256; i++)
            {
                if (histogram[i] >= pixelsCount * 0.05)
                {
                    minValue = i;
                    break;
                }
            }

            for (int i = 255; i > 0; i--)
            {
                if (histogram[i] <= pixelsCount * 0.95)
                {
                    maxValue = i;
                    break;
                }
            }

            double resultByte = 0;

            for (int i = 0; i < image.PixelWidth; ++i)
            {
                for (int j = 0; j < image.PixelHeight; ++j)
                {
                    int index = j * image.BackBufferStride + 4 * i;

                    for (int c = 0; c < 3; c++)
                    {
                        if (pixels[index + c] <= minValue)
                        {
                            resultByte = 0;
                        }
                        else if (pixels[index + c] >= maxValue)
                        {
                            resultByte = 255;
                        }
                        else
                        {
                            resultByte = (((double)pixels[index + c] - minValue) / (maxValue - minValue))*255;
                        }
                        resultPixels[index + c] = (byte) resultByte;
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
