
#region Imports

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Helpers;
using System.Web.Mvc;

#endregion

namespace cfares.site.modules.Helpers
{
	public static class WebImageExtensions
	{
		private static readonly IDictionary<string, ImageFormat> _transparencyFormats =
			new Dictionary<string, ImageFormat>(StringComparer.OrdinalIgnoreCase)
				{{"png", ImageFormat.Png}, {"gif", ImageFormat.Gif}};

		public static WebImage Resize(this WebImage image, int width)
		{
			double aspectRatio = (double)image.Width / image.Height;
			var height = Convert.ToInt32(width/aspectRatio);

			ImageFormat format;

			if (!_transparencyFormats.TryGetValue(image.ImageFormat.ToLower(), out format))
			{
				return image.Resize(width, height);
			}

			using (Bitmap resizedImage = new Bitmap(width, height))
			{
				using (var source = new Bitmap(new MemoryStream(image.GetBytes())))
				{
					using (Graphics g = Graphics.FromImage(resizedImage))
					{
						g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
						g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
						g.DrawImage(source, 0, 0, width, height);
					}
				}

				using (var ms = new MemoryStream())
				{
					resizedImage.Save(ms, format);
					return new WebImage(ms.ToArray());
				}
			}
		}
	}
}
