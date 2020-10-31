using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Path = System.IO.Path;

namespace Avery16282Generator.AeonsEnd
{
    public static class AeonsEndLabels
    {
        public static string CurrentPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static byte[] CreateLabels(IEnumerable<Expansion> selectedExpansions)
        {
            var garamond = Path.Combine(CurrentPath, "Fonts", "GARA.TTF");
            var garamondBold = Path.Combine(CurrentPath, "Fonts", "GARABD.TTF");
            var font = PdfFontFactory.CreateFont(garamond, true);
            var boldFont = PdfFontFactory.CreateFont(garamondBold, true);

            var dividers = DataAccess.GetDividers()
                .Where(divider => selectedExpansions.Contains(divider.Expansion))
                .ToList();
            var drawActionRectangles = dividers
                .Select((divider) => new Action<PdfCanvas, Rectangle>(
                    (canvas, rectangle) =>
                    {
                        var centeringCursor = new CenteringCursor(rectangle.GetTop(), rectangle.GetBottom());
                        var topCursor = centeringCursor.StartCursor;
                        var bottomCursor = centeringCursor.EndCursor;
                        const float dummyCostPadding = -27f;
                        topCursor.AdvanceCursor(rectangle.GetTop());
                        bottomCursor.AdvanceCursor(rectangle.GetBottom());

                        DrawBackground(canvas, rectangle, divider.Type);
                        if (divider.Cost != null)
                            DrawCost(canvas, rectangle, divider.Cost.Value, boldFont, topCursor);
                        else
                            topCursor.AdvanceCursor(dummyCostPadding);
                        DrawExpansionLogo(canvas, rectangle, divider.Expansion.GetAbbreviation(), boldFont, bottomCursor);
                        DrawName(canvas, rectangle, divider.Name, font, centeringCursor);
                    }))
                .ToList();
        

            var drawActionRectangleQueue = new Queue<Action<PdfCanvas, Rectangle>>(drawActionRectangles);
            return PdfGenerator.DrawRectangles(drawActionRectangleQueue, ColorConstants.WHITE);
        }

        private static void DrawName(
            PdfCanvas canvas,
            Rectangle rectangle,
            string name,
            PdfFont font,
            CenteringCursor centeringCursor
            )
        {
            const float maxFontSize = 10f;
            var potentialTextRectangleHeight = centeringCursor.GetCurrentStartWithCentering() - centeringCursor.GetCurrentEndWithCentering();
            var textFontSize = TextSharpHelpers.GetFontSize(canvas, name, potentialTextRectangleHeight, font, maxFontSize);
            var textRectangleHeight = textFontSize * .9f;
            
            var textWidthOffsetToCenter = (rectangle.GetWidth() / 2.0f - textRectangleHeight / 2.0f) - 3f;
            var extraWidthOffsetForBackgroundImage = textFontSize/3;
            var textRectangle = new Rectangle(
                rectangle.GetLeft() + textWidthOffsetToCenter + extraWidthOffsetForBackgroundImage,
                centeringCursor.GetCurrentEndWithCentering(),
                textWidthOffsetToCenter + extraWidthOffsetForBackgroundImage + textRectangleHeight,
                potentialTextRectangleHeight);
            DrawCenteredText(canvas, name, textRectangle, font, ColorConstants.BLACK, textFontSize, FontWeight.Regular);
        }

        private static void DrawExpansionLogo(PdfCanvas canvas, Rectangle rectangle, string expansion, PdfFont font, Cursor bottomCursor)
        {
            const float textHeight = 18f;
            const float fontSize = 8f;
            const float textWidthPadding = 8f;
            const float textHeightPadding = 1f;
            var textRectangle = new Rectangle(
                rectangle.GetLeft() + textWidthPadding,
                bottomCursor.GetCurrent() + textHeightPadding,
                rectangle.GetWidth(),
                textHeight);
            DrawText(canvas, expansion, textRectangle,font, ColorConstants.BLACK, fontSize, FontWeight.Regular);
            bottomCursor.AdvanceCursor(textRectangle.GetHeight() + textHeightPadding);
        }

        private static void DrawCost(PdfCanvas canvas, Rectangle rectangle, int dividerCost, PdfFont font, Cursor topCursor)
        {
            const float textHeight = 12f;
            const float fontSize = 14f;
            const float textWidthPadding = 6.5f;
            const float textHeightPadding = 4f;
            var textRectangle = new Rectangle(
                rectangle.GetLeft() + textWidthPadding,
                topCursor.GetCurrent() - (textHeight + textHeightPadding),
                rectangle.GetWidth(),
                textHeight);
            DrawText(canvas, dividerCost.ToString(), textRectangle, font, ColorConstants.BLACK, fontSize, FontWeight.Regular);
            topCursor.AdvanceCursor(-(textRectangle.GetHeight() + textHeightPadding));

            const float imageHeight = 15f;
            var imageRectangle = new Rectangle(
                rectangle.GetLeft(),
                topCursor.GetCurrent() + 3 - imageHeight,
                rectangle.GetWidth(),
                imageHeight);
            DrawImage(imageRectangle, canvas, Path.Combine(CurrentPath, "AeonsEnd", "Aether.png"), centerHorizontally:true, centerVertically:true);
            topCursor.AdvanceCursor(-imageRectangle.GetHeight());
        }

        private static void DrawBackground(PdfCanvas canvas, Rectangle rectangle, string dividerType)
        {
            DrawImage(rectangle, canvas, Path.Combine(CurrentPath, "AeonsEnd", $"{dividerType}.png"), centerHorizontally:true, centerVertically:true);
        }

        private static void DrawText(PdfCanvas canvas, string text, Rectangle rectangle, PdfFont font, Color color, float size, FontWeight fontWeight)
        {
            const float textRotation = (float)(3 * Math.PI / 2);
            TextSharpHelpers.WriteNonWrappingTextInRectangle(canvas, text, rectangle, 0, 0, font, color, size, textRotation, fontWeight);
        }

        private static void DrawCenteredText(PdfCanvas canvas, string text, Rectangle rectangle, PdfFont font, Color color, float size, FontWeight fontWeight)
        {
            const float textRotation = (float)(3 * Math.PI / 2);
            TextSharpHelpers.WriteCenteredNonWrappingTextInRectangle(canvas, text, rectangle, font, color, size, textRotation, fontWeight);
        }

        private static void DrawImage(
            Rectangle rectangle,
            PdfCanvas canvas,
            string imagePath,
            bool scaleAbsolute = false,
            bool centerVertically = false,
            bool centerHorizontally = false)
        {
            TextSharpHelpers.DrawImage(rectangle, canvas, imagePath, System.Drawing.RotateFlipType.Rotate90FlipNone, scaleAbsolute, centerVertically, centerHorizontally);
        }
    }
}
