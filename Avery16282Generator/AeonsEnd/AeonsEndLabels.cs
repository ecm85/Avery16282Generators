using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Avery16282Generator.AeonsEnd
{
    public static class AeonsEndLabels
    {
        public static string GetCurrentPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";

        public static byte[] CreateLabels(IEnumerable<Expansion> includedSets)
        {
            var garamond = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "GARA.TTF");
            var garamondBold = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "GARABD.TTF");
            var baseFont = BaseFont.CreateFont(garamond, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var boldBaseFont = BaseFont.CreateFont(garamondBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            var dividers = DataAccess.GetDividers()
                .Where(divider => includedSets.Contains(divider.Expansion))
                .ToList();
            var drawActionRectangles = dividers
                .SelectMany(divider => new List<Action<PdfContentByte, Rectangle>>
                {
                    (canvas, rectangle) =>
                    {
                        var centeringCursor = new CenteringCursor(rectangle.Top, rectangle.Bottom);
                        var topCursor = centeringCursor.StartCursor;
                        var bottomCursor = centeringCursor.EndCursor;
                        const float topPadding = -4f;
                        const float dummyCostPadding = -27f;
                        topCursor.AdvanceCursor(topPadding);
                        
                        DrawBackground(canvas, rectangle, divider.Type, topCursor, bottomCursor);
                        if (divider.Cost != null)
                            DrawCost(canvas, rectangle, divider.Cost.Value, boldBaseFont, topCursor);
                        else
                            topCursor.AdvanceCursor(dummyCostPadding);
                        DrawExpansionLogo(canvas, rectangle, divider.Expansion.GetAbbreviation(), boldBaseFont, bottomCursor);
                        DrawName(canvas, rectangle, divider.Name, baseFont, centeringCursor);
                    }
                })
                .ToList();
        

            var drawActionRectangleQueue = new Queue<Action<PdfContentByte, Rectangle>>(drawActionRectangles);
            return PdfGenerator.DrawRectangles(drawActionRectangleQueue, BaseColor.WHITE);
        }

        private static void DrawName(
            PdfContentByte canvas,
            Rectangle rectangle,
            string name,
            BaseFont baseFont,
            CenteringCursor centeringCursor
            )
        {
            const float maxFontSize = 15f;
            var potentialTextRectangleHeight = centeringCursor.GetCurrentStartWithCentering() - centeringCursor.GetCurrentEndWithCentering();
            var textFontSize = TextSharpHelpers.GetFontSize(canvas, name, potentialTextRectangleHeight, baseFont, maxFontSize, Element.ALIGN_LEFT, Font.NORMAL);
            var font = new Font(baseFont, textFontSize, Font.NORMAL, BaseColor.BLACK);
            var textRectangleHeight = textFontSize * .9f;
            
            var textWidthOffsetToCenter = (rectangle.Width / 2.0f - textRectangleHeight / 2.0f);
            var extraWidthOffsetForBackgroundImage = textFontSize/3;
            var textRectangle = new Rectangle(
                rectangle.Left + textWidthOffsetToCenter + extraWidthOffsetForBackgroundImage,
                centeringCursor.GetCurrentEndWithCentering(),
                rectangle.Left + textWidthOffsetToCenter + extraWidthOffsetForBackgroundImage + textRectangleHeight,
                centeringCursor.GetCurrentStartWithCentering());
            DrawCenteredText(canvas, name, textRectangle, font);
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
            DrawText(canvas, expansion, textRectangle, font);
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
            DrawText(canvas, dividerCost.ToString(), textRectangle, font);
            topCursor.AdvanceCursor(-(textRectangle.Height + textHeightPadding));

            const float imageHeight = 15f;
            var imageRectangle = new Rectangle(
                rectangle.Left,
                topCursor.GetCurrent() + 3 - imageHeight,
                rectangle.Right,
                topCursor.GetCurrent() + 3);
            DrawImage(imageRectangle, canvas, GetCurrentPath + @"AeonsEnd\Aether.png", centerHorizontally:true, centerVertically:false);
            topCursor.AdvanceCursor(-imageRectangle.Height);
        }

        private static void DrawBackground(PdfContentByte canvas, Rectangle rectangle, string dividerType, Cursor topCursor, Cursor bottomCursor)
        {
            var image = DrawImage(rectangle, canvas, GetCurrentPath + $@"AeonsEnd\{dividerType}.png", centerHorizontally:true, centerVertically:true);
            bottomCursor.AdvanceCursor(image.AbsoluteY);
            topCursor.AdvanceCursor(bottomCursor.GetCurrent() + image.ScaledHeight);
        }

        private static void DrawText(PdfContentByte canvas, string text, Rectangle rectangle, Font font)
        {
            const int textRotation = 270;
            TextSharpHelpers.WriteNonWrappingTextInRectangle(canvas, text, rectangle, 0, 0, font, textRotation);
        }

        private static void DrawCenteredText(PdfContentByte canvas, string text, Rectangle rectangle, Font font)
        {
            const int textRotation = 270;
            TextSharpHelpers.WriteCenteredNonWrappingTextInRectangle(canvas, text, rectangle, font, textRotation);
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
