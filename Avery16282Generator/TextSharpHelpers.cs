using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Avery16282Generator
{
	public static class TextSharpHelpers
	{
		public static void DrawRoundedRectangle(PdfCanvas canvas, Rectangle rectangle, Color baseColor)
		{
			canvas.SetFillColor(baseColor);
			canvas.SetStrokeColor(ColorConstants.BLACK);
			canvas.RoundRectangle(rectangle.GetLeft(), rectangle.GetBottom(), rectangle.GetWidth(), rectangle.GetHeight(), 5f);
			canvas.FillStroke();
		}

		public static void DrawRectangle(PdfCanvas canvas, Rectangle rectangle, Color baseColor)
		{
			canvas.SetFillColor(baseColor);
			canvas.Rectangle(rectangle.GetLeft(), rectangle.GetBottom(), rectangle.GetWidth(), rectangle.GetHeight());
			canvas.Fill();
		}

		private static bool SimulateWriteWrappingTextInRectangle(PdfCanvas pdfCanvas, string text, PdfFont font, float nextAttemptFontSize, Rectangle rectangle)
		{
			var paragraph = new Paragraph(text);
			paragraph.SetFont(font);
			paragraph.SetFontColor(ColorConstants.BLACK);
			paragraph.SetFontSize(nextAttemptFontSize);
			var canvas = new Canvas(pdfCanvas, rectangle);
			var childRenderer = paragraph.CreateRendererSubTree().SetParent(canvas.GetRenderer());
			var layoutContext = new LayoutContext(new LayoutArea(1, rectangle));
			var result = childRenderer.Layout(layoutContext);
			return result.GetStatus() == LayoutResult.FULL;
		}

		private static LayoutResult SimulateWriteNonWrappingTextInRectangle(PdfCanvas pdfCanvas, string text, PdfFont font, float nextAttemptFontSize, Rectangle rectangle)
		{
			var textElement = new Text(text);
			textElement.SetFont(font);
			textElement.SetFontColor(ColorConstants.BLACK);
			textElement.SetFontSize(nextAttemptFontSize);
			var canvas = new Canvas(pdfCanvas, rectangle);
			var childRenderer = textElement.CreateRendererSubTree().SetParent(canvas.GetRenderer());
			var layoutContext = new LayoutContext(new LayoutArea(1, rectangle));
			return childRenderer.Layout(layoutContext);
		}

		public static float GetMultiLineFontSize(PdfCanvas canvas, string text, Rectangle rectangle, PdfFont font, float maxFontSize)
		{
			var nextAttemptFontSize = maxFontSize;
			while (true)
			{
				if (SimulateWriteWrappingTextInRectangle(canvas, text, font, nextAttemptFontSize, rectangle))
				{
					return nextAttemptFontSize;
				}
				nextAttemptFontSize -= .2f;
			}
		}

		public static float GetFontSize(PdfCanvas canvas, string text, float width, PdfFont font, float maxFontSize)
		{
			var nextAttemptFontSize = maxFontSize;
			var rectangle = new Rectangle(0, 0, width, nextAttemptFontSize * 3f);
			while (true)
			{
				var layoutResult = SimulateWriteNonWrappingTextInRectangle(canvas, text, font, nextAttemptFontSize, rectangle);
				if (layoutResult.GetStatus() == LayoutResult.FULL)
				{
					return nextAttemptFontSize;
				}
				nextAttemptFontSize -= .2f;
				rectangle = new Rectangle(0, 0, width, nextAttemptFontSize * 3f);
			}
		}

		public static Image DrawImage(Rectangle rectangle, PdfCanvas canvas, string imagePath, System.Drawing.RotateFlipType rotateFlipType, bool scaleAbsolute, bool centerVertically, bool centerHorizontally)
		{
			using (var originalImage = System.Drawing.Image.FromFile(imagePath))
			{
				originalImage.RotateFlip(rotateFlipType);
				using (var memoryStream = new MemoryStream())
				{
					originalImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
					var imageData = ImageDataFactory.Create(memoryStream.ToArray());
					var image = new Image(imageData);
					if (scaleAbsolute)
						image.ScaleAbsolute(rectangle.GetWidth(), rectangle.GetHeight());
					else
						image.ScaleToFit(rectangle.GetWidth(), rectangle.GetHeight());
					var xOffset = centerHorizontally ? (rectangle.GetWidth() / 2 - image.GetImageScaledWidth() / 2) : 0;
					var yOffset = centerVertically ? (rectangle.GetHeight() / 2 - image.GetImageScaledHeight() / 2) : 0;
					var imageRectangle = new Rectangle(
						rectangle.GetX() + xOffset,
						rectangle.GetY() + yOffset,
						image.GetImageScaledWidth(),
						image.GetImageScaledHeight());
					canvas.AddImageFittedIntoRectangle(imageData, imageRectangle, false);
					return image;
				}
			}
		}

		public static Image DrawImage(Rectangle rectangle, PdfCanvas canvas, PdfImageXObject imageXObject, System.Drawing.RotateFlipType rotateFlipType, bool scaleAbsolute, bool centerVertically, bool centerHorizontally)
		{
			var image = new Image(imageXObject);
			if (scaleAbsolute)
				image.ScaleAbsolute(rectangle.GetWidth(), rectangle.GetHeight());
			else
				image.ScaleToFit(rectangle.GetWidth(), rectangle.GetHeight());
			var xOffset = centerHorizontally ? (rectangle.GetWidth() / 2 - image.GetImageScaledWidth() / 2) : 0;
			var yOffset = centerVertically ? (rectangle.GetHeight() / 2 - image.GetImageScaledHeight() / 2) : 0;
			var imageRectangle = new Rectangle(
				rectangle.GetX() + xOffset,
				rectangle.GetY() + yOffset,
				image.GetImageScaledWidth(),
				image.GetImageScaledHeight());
			canvas.AddXObjectFittedIntoRectangle(imageXObject, imageRectangle);
			return image;
		}

		public static void WriteCenteredNonWrappingTextInRectangle(PdfCanvas canvas, string text, Rectangle rectangle, PdfFont font, Color color, float size, float textRotationInRadians, FontWeight fontWeight)
		{
			var x = rectangle.GetLeft();
			var y = rectangle.GetBottom() + rectangle.GetHeight()/2;
			var textCanvas = new Canvas(canvas, rectangle);
			textCanvas.SetFont(font);
			textCanvas.SetFontColor(color);
			textCanvas.SetFontSize(size);
			if (fontWeight == FontWeight.Bold)
			{
				textCanvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.FILL_STROKE);
				textCanvas.SetStrokeWidth(.3f);
			}
			textCanvas.ShowTextAligned(text, x, y, iText.Layout.Properties.TextAlignment.CENTER, textRotationInRadians);
		}

		public static void WriteNonWrappingTextInRectangle(PdfCanvas canvas, string text, Rectangle rectangle, float textWidthOffset, float textHeightOffset, PdfFont font, Color color, float size, float textRotationInRadians, FontWeight fontWeight)
		{
			var x = rectangle.GetLeft() + textWidthOffset;
			var y = rectangle.GetTop() - textHeightOffset;
			var textCanvas = new Canvas(canvas, rectangle);
			textCanvas.SetFont(font);
			textCanvas.SetFontColor(color);
			textCanvas.SetFontSize(size);
			if (fontWeight == FontWeight.Bold)
			{
				textCanvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.FILL_STROKE);
				textCanvas.SetStrokeWidth(.3f);
			}
			textCanvas.ShowTextAligned(text, x, y, iText.Layout.Properties.TextAlignment.LEFT, iText.Layout.Properties.VerticalAlignment.BOTTOM, textRotationInRadians);
		}

		public static void WriteWrappingTextInRectangle(PdfCanvas canvas, string text, Rectangle rectangle, PdfFont font, Color color, float size, float textRotationInRadians)
		{
			var lines = GetLines(canvas, text, font, rectangle.GetHeight(), size).ToList();
			var fudgeFactor = (1f / size) * 20;
			var width = rectangle.GetWidth() - fudgeFactor * 2;
			var lineHeight = width / lines.Count;
			for (var lineIndex = 0; lineIndex < lines.Count; lineIndex++)
			{
				var line = lines[lineIndex];
				var x = rectangle.GetRight() - (fudgeFactor + (lineIndex + 1) * lineHeight);
				var y = rectangle.GetTop();
				var textCanvas = new Canvas(canvas, rectangle);
				textCanvas.SetFont(font);
				textCanvas.SetFontColor(color);
				textCanvas.SetFontSize(size);
				textCanvas.ShowTextAligned(line, x, y, iText.Layout.Properties.TextAlignment.LEFT, iText.Layout.Properties.VerticalAlignment.BOTTOM, textRotationInRadians);
			}
		}

		private static IEnumerable<string> GetLines(PdfCanvas canvas, string text, PdfFont font, float width, float size)
		{
			var remainingWords = new Queue<string> (text.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList());
			var singleLineRectangle = new Rectangle(0, 0, width, size * 3f);
			do
			{
				var line = "";
				bool tryAddAnotherWordToLine;
				do
				{
					var lineWithNextWord = $"{line} {remainingWords.Peek()}";
					var result = SimulateWriteNonWrappingTextInRectangle(canvas, lineWithNextWord, font, size, singleLineRectangle);
					if (result.GetStatus() == LayoutResult.FULL)
					{
						line = lineWithNextWord;
						remainingWords.Dequeue();
						tryAddAnotherWordToLine = remainingWords.Any();
					}
					else
					{
						tryAddAnotherWordToLine = false;
					}
				} while (tryAddAnotherWordToLine);
				yield return line;
			} while (remainingWords.Any());
		}
	}
}
