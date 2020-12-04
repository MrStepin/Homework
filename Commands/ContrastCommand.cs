
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

            int[] sourceColors = Enumerable.Range(0, 256).ToArray();

            List<int> histogram = new List<int>();

            int pixelsCount = 3 * image.PixelWidth * image.PixelHeight;

            int colorCount = 0;

            foreach (byte color in sourceColors)
            {
                colorCount = 0;

                for (int i = 0; i < image.PixelWidth; ++i)
                {
                    for (int j = 0; j < image.PixelHeight; ++j)
                    {
                        int index = j * image.BackBufferStride + i * 4;

                        if (pixels[index] == color)
                        {
                            colorCount += 1;
                        }
                    }
                }

                histogram.Add(colorCount);
            }

            int previousValue = 0;
            for (int histogramIndex = 0; histogramIndex < 256; histogramIndex++)
            {

                histogram[histogramIndex] = histogram[histogramIndex] + previousValue;
                previousValue = histogram[histogramIndex];

            }

            List<int> low = new List<int>();
            List<int> high = new List<int>();

            int countOfCumulativeHistogram = 0;
            foreach (var value in histogram)
            {
                if (value < 0.05 * pixelsCount)
                {
                    low.Add(countOfCumulativeHistogram);
                }
                else if (value > 0.95 * pixelsCount)
                {
                    high.Add(countOfCumulativeHistogram);
                }
                countOfCumulativeHistogram += 1;
            }

            int minValue = 0;
            int maxValue = 255;
            byte resultByte = 0;

            if (low.Count != 0)
            {
                minValue = low.Max();
            }

            if (high.Count != 0)
            {
                maxValue = high.Min();
            }

            for (int i = 0; i < image.PixelWidth; ++i)
            {
                for (int j = 0; j < image.PixelHeight; ++j)
                {
                    int index = j * image.BackBufferStride + 4 * i;

                    for (int c = 0; c < 3; ++c)
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
                            resultByte = (byte)(pixels[index + c] - minValue / (maxValue - minValue));
                        }
                        resultPixels[index + c] = resultByte;
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