using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Avery16282Generator
{
    public static class TextSharpHelpers
    {
        public static void DrawRectangle(PdfContentByte content, Rectangle rectangle, BaseColor baseColor)
        {
            content.SetColorFill(baseColor);
            content.Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, rectangle.Height);
            content.Fill();
        }

        private static bool WriteTextInRectangle(PdfContentByte contentByte, string text, Font font, Rectangle rectangle, int alignment, bool simulation = false)
        {
            var phrase = new Phrase(text, font);

            var columnText = new ColumnText(contentByte)
            {
                Alignment = alignment
            };
            columnText.SetSimpleColumn(phrase, rectangle.Left, rectangle.Bottom, rectangle.Right, rectangle.Top, font.Size, alignment);
            var result = columnText.Go(simulation);
            return !ColumnText.HasMoreText(result);
        }

        public static float GetFontSize(PdfContentByte contentByte, string text, float width, BaseFont baseFont, float maxFontSize, int alignment, int fontStyle)
        {
            var nextAttemptFontSize = maxFontSize;
            var rectangle = new Rectangle(0, 0, width, nextAttemptFontSize * 1.2f);
            while (true)
            {
                var font = new Font(baseFont, nextAttemptFontSize, fontStyle, BaseColor.BLACK);
                if (WriteTextInRectangle(contentByte, text, font, rectangle, alignment, true))
                {
                    return nextAttemptFontSize;
                }
                nextAttemptFontSize -= .2f;
                rectangle = new Rectangle(0, 0, width, nextAttemptFontSize * 1.2f);
            }
        }

        public static Image DrawImage(Rectangle rectangle, PdfContentByte canvas, string imagePath, float imageRotationInRadians, bool scaleAbsolute, bool center)
        {
            var image = Image.GetInstance(imagePath);
            image.Rotation = imageRotationInRadians;
            if (scaleAbsolute)
                image.ScaleAbsolute(rectangle.Rotate());
            else
                image.ScaleToFit(rectangle);
            var imageBottom = center ? rectangle.Bottom + (rectangle.Height - image.ScaledHeight) / 2 : rectangle.Bottom;
            image.SetAbsolutePosition(rectangle.Left, imageBottom);
            canvas.AddImage(image);
            return image;
        }

        public static void DrawText(PdfContentByte canvas, string text, Rectangle rectangle, float textWidthOffset,
            float textHeightOffset, Font font, int textRotation)
        {
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(text, font),
                rectangle.Left + textWidthOffset, rectangle.Top - textHeightOffset,
                textRotation);
        }
    }
}
