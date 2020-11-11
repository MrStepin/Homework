using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Catel.MVVM;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConvolutionWpf.Commands
{
	public class FlipCommand : Command
	{
		private readonly Func<WriteableBitmap> _imageFactory;

		public event Action<WriteableBitmap> OnImageChanged;

		public FlipCommand(Func<WriteableBitmap> imageFactory)
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


			var imageRes = new WriteableBitmap(2 * image.PixelWidth, image.PixelHeight, image.DpiX, image.DpiY, image.Format, image.Palette);
			var resultPixels = new byte[imageRes.PixelHeight * imageRes.BackBufferStride];

			for (int i = 0; i < image.PixelHeight; i++)
			{
				for (int j = 0; j < image.PixelWidth; j++)
				{
					int indexOld = i * image.BackBufferStride + 4 * j;

					int indexNew = i * imageRes.BackBufferStride + 4 * j;

					for (int c = 0; c < 3; ++c)
					{
						resultPixels[indexNew + c] = pixels[indexOld + c];
					}


				}
			}

			for (int i = 0; i < image.PixelHeight; i++)
			{
				int k = 1;
				for (int j = imageRes.PixelWidth -1; j > image.PixelWidth; j--)
				{
					int indexFlip = i * imageRes.BackBufferStride + 4 * j;

					int index = i * image.BackBufferStride + 4 * (j + k - imageRes.PixelWidth);

					for (int c = 0; c < 3; c++)
					{
						resultPixels[indexFlip + c] = pixels[index + c];
					}
					k += 2;
				}

			}

					imageRes.WritePixels(new Int32Rect(0, 0, imageRes.PixelWidth, imageRes.PixelHeight), resultPixels, imageRes.BackBufferStride, 0);

			OnImageChanged?.Invoke(imageRes);
		}

		protected override void Execute(object parameter, bool ignoreCanExecuteCheck)
		{
			ExecuteCommand();
		}
	}
}