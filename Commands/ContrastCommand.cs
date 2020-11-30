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

            Dictionary<int, int> cumulativeHistogram = new Dictionary<int, int>();

            int pixelsCount = image.PixelWidth * image.PixelHeight;

            foreach (byte color in sourceColors) 
            {
                int colorCount = 0;

                for (int i = 0; i < image.PixelWidth; ++i)
                {
                    for (int j = 0; j < image.PixelHeight; ++j)
                    {
                        int index = j * image.BackBufferStride + i;

                        if (pixels[index] == color)
                            {
                            colorCount += 1;
                            }
                    }
                }

                cumulativeHistogram.Add(color, colorCount);
            }

            List<int> low = new List<int>();
            List<int> high = new List<int>();

            int keyOfCumulativeHistogram = 0;
            foreach (var value in cumulativeHistogram.Values)
            {
                if (value < 0.05 * pixelsCount)
                {
                    low.Add(keyOfCumulativeHistogram);
                }
                else if(value > 0.95 * pixelsCount)
                {
                    high.Add(keyOfCumulativeHistogram);
                }
                keyOfCumulativeHistogram += 1;
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

                    if (pixels[index] <= minValue)
                    {
                        resultByte = 0;
                    }
                    else if (pixels[index] >= maxValue)
                    {
                        resultByte = 255;
                    }
                    else
                    {
                        resultByte = (byte)(pixels[index] - minValue / (maxValue - minValue));
                    }

                    for (int c = 0; c < 3; ++c)
                    {
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