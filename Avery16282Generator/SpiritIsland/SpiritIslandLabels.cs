using Avery16282Generator.Brewcrafters;
using Avery16282Generator.Dominion;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Element;
using iText.Svg.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Path = System.IO.Path;

namespace Avery16282Generator.SpiritIsland
{
	public static class SpiritIslandLabels
	{
		public static string CurrentPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		public static byte[] CreateLabels()
		{
			var dkSnemand = Path.Combine(CurrentPath, "Fonts", "DK Snemand.otf");
			var font = PdfFontFactory.CreateFont(dkSnemand, true);

			var labelNames = GetLabelNames();
			var drawActionRectangles = labelNames.Select((labelName, index) => new Action<PdfCanvas, Rectangle>(
				(canvas, rectangle) =>
				{
					DrawBackgroundImage(rectangle, canvas);
					const float offsetPercent = .9f;
					var textRectangle = new Rectangle(
						rectangle.GetX() + rectangle.GetWidth() * ((1 - offsetPercent * .8f) / 2),
						rectangle.GetY() + rectangle.GetHeight() * ((1 - offsetPercent) / 2),
						rectangle.GetWidth() * offsetPercent * .8f,
						rectangle.GetHeight() * offsetPercent);

					const float textRotation = (float)(3 * Math.PI / 2);
					var fontSize = TextSharpHelpers.GetFontSize(
						canvas,
						labelName,
						textRectangle.GetHeight(),
						font,
						18) - .5f;

					TextSharpHelpers.WriteCenteredNonWrappingTextInRectangle(canvas, labelName, textRectangle, font, ColorConstants.WHITE, fontSize, textRotation, FontWeight.Regular);
				}
			)).ToList();

			var drawActionRectangleQueue = new Queue<Action<PdfCanvas, Rectangle>>(drawActionRectangles);
			return PdfGenerator.DrawRectangles(
				drawActionRectangleQueue,
				ColorConstants.WHITE);
		}

		private static void DrawBackgroundImage(
		   Rectangle rectangle,
		   PdfCanvas canvas)
		{
			var imagePath = Path.Combine(CurrentPath, "SpiritIsland", "backgroundImage.png");
			DrawImage(rectangle, canvas, imagePath, true, true, true);
		}

		private static Image DrawImage(
			Rectangle rectangle,
			PdfCanvas canvas,
			string imagePath,
			bool scaleAbsolute = false,
			bool centerVertically = false,
			bool centerHorizontally = false)
		{
			return TextSharpHelpers.DrawImage(rectangle, canvas, imagePath, System.Drawing.RotateFlipType.Rotate90FlipNone, scaleAbsolute, centerVertically, centerHorizontally);
		}

		public static string[] GetLabelNames()
		{
			return new string[] {
				"MINOR POWERS",
				"MAJOR POWERS",
				"EVENTS",
				"FEAR CARDS",
				"PROGRESSIONS",
				"PLAYER AIDS",
				"BLIGHTED ISLAND",
				"REMINDERS",
				"INVADER CARDS",
			};
		}
	}
}
