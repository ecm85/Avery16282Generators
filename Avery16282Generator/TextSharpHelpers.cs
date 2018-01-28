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

        public static float GetFontSize(PdfContentByte contentByte, string text, Rectangle rectangle, BaseFont baseFont, float maxFontSize, int alignment, int fontStyle)
        {
            var nextAttemptFontSize = maxFontSize;
            while (true)
            {
                var font = new Font(baseFont, nextAttemptFontSize, fontStyle, BaseColor.BLACK);
                if (WriteTextInRectangle(contentByte, text, font, rectangle, alignment, true))
                {
                    return nextAttemptFontSize;
                }
                nextAttemptFontSize -= .2f;
            }
        }
    }
}
