using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Avery16282Generator.AeonsEnd
{
    public static class AeonsEndLabels
    {
        public static void CreateLabels()
        {
            
            var garamond = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "GARA.TTF");
            var garamondBold = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "GARABD.TTF");
            var baseFont = BaseFont.CreateFont(garamond, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var boldBaseFont = BaseFont.CreateFont(garamondBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            var dividers = DataAccess.GetDividers();
            var drawActionRectangles = dividers
                .SelectMany(divider => new List<Action<PdfContentByte, Rectangle>>
                {
                    (canvas, rectangle) =>
                    {
                        var topCursor = new Cursor();
                        const float topPadding = -4f;
                        const float dummyCostPadding = -27f;
                        topCursor.AdvanceCursor(topPadding);
                        var bottomCursor = new Cursor();
                        DrawBackground(canvas, rectangle, divider.Type, topCursor, bottomCursor);
                        if (divider.Cost != null)
                            DrawCost(canvas, rectangle, divider.Cost.Value, boldBaseFont, topCursor);
                        else
                            topCursor.AdvanceCursor(dummyCostPadding);
                        DrawExpansionLogo(canvas, rectangle, divider.Expansion, boldBaseFont, bottomCursor);
                        DrawName(canvas, rectangle, divider.Name, baseFont, topCursor, bottomCursor);
                    }
                })
                .ToList();
        

            var drawActionRectangleQueue = new Queue<Action<PdfContentByte, Rectangle>>(drawActionRectangles);
            PdfGenerator.DrawRectangles(drawActionRectangleQueue, BaseColor.WHITE, "AeonsEnd");
        }

        private static void DrawName(PdfContentByte canvas, Rectangle rectangle, string name, BaseFont baseFont, Cursor topCursor, Cursor bottomCursor)
        {
            const float textHeightPadding = 3f;
            const float textWidthPadding = 10f;
            const float maxFontSize = 15f;

            var textRectangle = new Rectangle(
                rectangle.Left + textWidthPadding,
                bottomCursor.GetCurrent() + textHeightPadding,
                rectangle.Right,
                topCursor.GetCurrent());
            var fontSize = TextSharpHelpers.GetFontSize(canvas, name, textRectangle.Height, baseFont, maxFontSize,Element.ALIGN_LEFT, Font.NORMAL );
            var font = new Font(baseFont, fontSize, Font.NORMAL, BaseColor.BLACK);
            DrawText(canvas, name, textRectangle, 0, 0, font);
        }

        private static void DrawExpansionLogo(PdfContentByte canvas, Rectangle rectangle, string expansion, BaseFont baseFont, Cursor bottomCursor)
        {
            const float textHeight = 18f;
            const float fontSize = 8f;
            const float textHeightPadding = 1f;
            const float textWidthPadding = 10f;
            var font = new Font(baseFont, fontSize, Font.NORMAL, BaseColor.BLACK);
            var textRectangle = new Rectangle(
                rectangle.Left + textWidthPadding,
                bottomCursor.GetCurrent() + textHeightPadding,
                rectangle.Right,
                bottomCursor.GetCurrent() + textHeightPadding + textHeight);
            DrawText(canvas, expansion, textRectangle, 0, 0, font);
            bottomCursor.AdvanceCursor(textRectangle.Height + textHeightPadding);
        }

        private static void DrawCost(PdfContentByte canvas, Rectangle rectangle, int dividerCost, BaseFont baseFont, Cursor topCursor)
        {
            const float textHeight = 12f;
            const float fontSize = 14f;
            const float textHeightPadding = 0f;
            const float textWidthPadding = 10f;
            var font = new Font(baseFont, fontSize, Font.NORMAL, BaseColor.BLACK);
            var textRectangle = new Rectangle(
                rectangle.Left + textWidthPadding,
                topCursor.GetCurrent() - (textHeight + textHeightPadding),
                rectangle.Right,
                topCursor.GetCurrent() - textHeightPadding);
            DrawText(canvas, dividerCost.ToString(), textRectangle, 0, 0, font);
            topCursor.AdvanceCursor(-(textRectangle.Height + textHeightPadding));

            const float imageHeight = 15f;
            var imageRectangle = new Rectangle(
                rectangle.Left,
                topCursor.GetCurrent() + 3 - imageHeight,
                rectangle.Right,
                topCursor.GetCurrent() + 3);
            DrawImage(imageRectangle, canvas, @"AeonsEnd\Aether.png", centerHorizontally:true, centerVertically:false);
            topCursor.AdvanceCursor(-imageRectangle.Height);
        }

        private static void DrawBackground(PdfContentByte canvas, Rectangle rectangle, string dividerType, Cursor topCursor, Cursor bottomCursor)
        {
            var image = DrawImage(rectangle, canvas, $@"AeonsEnd\{dividerType}.png", centerHorizontally:true, centerVertically:true);
            bottomCursor.AdvanceCursor(image.AbsoluteY);
            topCursor.AdvanceCursor(bottomCursor.GetCurrent() + image.ScaledHeight);
        }

        private static void DrawText(PdfContentByte canvas, string text, Rectangle rectangle,
            float textWidthOffset, float textHeightOffset, Font font)
        {
            const int textRotation = 270;
            TextSharpHelpers.WriteNonWrappingTextInRectangle(canvas, text, rectangle, textWidthOffset, textHeightOffset, font, textRotation, Element.ALIGN_LEFT);
        }


        private static Image DrawImage(
            Rectangle rectangle,
            PdfContentByte canvas,
            string imagePath,
            bool scaleAbsolute = false,
            bool centerVertically = false,
            bool centerHorizontally = false)
        {
            const float imageRotationInRadians = 4.71239f;
            return TextSharpHelpers.DrawImage(rectangle, canvas, imagePath, imageRotationInRadians, scaleAbsolute, centerVertically, centerHorizontally);
        }
    }
}
