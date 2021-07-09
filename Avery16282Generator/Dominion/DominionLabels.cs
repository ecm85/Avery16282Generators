using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Path = System.IO.Path;

namespace Avery16282Generator.Dominion
{
    public static class DominionLabels
    {
        public static string CurrentPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static byte[] CreateLabels(IEnumerable<Expansion> expansionsToPrint)
        {
            var cardsToPrint = DominionCardDataAccess.GetCardsToPrint(expansionsToPrint);

            var trajan = Path.Combine(CurrentPath, "Fonts", "TRAJANPROREGULAR.TTF");
            var trajanBold = Path.Combine(CurrentPath, "Fonts", "TRAJANPROBOLD.TTF");
            var font = PdfFontFactory.CreateFont(trajan, true);
            var boldFont = PdfFontFactory.CreateFont(trajanBold, true);
            var drawActionRectangles = cardsToPrint.SelectMany(card => new List<Action<PdfCanvas, Rectangle>>
            {
                (canvas, rectangle) =>
                {
                    var topCursor = new Cursor();
                    var bottomCursor = new Cursor();
                    topCursor.AdvanceCursor(rectangle.GetTop());
                    bottomCursor.AdvanceCursor(rectangle.GetBottom());
                    DrawBackgroundImage(card.SuperType, rectangle, canvas);
                    DrawCosts(boldFont, card, rectangle, canvas, topCursor);
                    DrawSetImageAndReturnTop(rectangle, bottomCursor, card.Set.Image, canvas);

                    var cardName = card.GroupName ?? card.Name;
                    DrawCardText(rectangle, topCursor, bottomCursor, canvas, cardName, font, card.SuperType);
                }
            }).ToList();
            var drawActionRectangleQueue = new Queue<Action<PdfCanvas, Rectangle>>(drawActionRectangles);
            return PdfGenerator.DrawRectangles(drawActionRectangleQueue, ColorConstants.WHITE);
        }

        private static void DrawCardText(
            Rectangle rectangle,
            Cursor topCursor,
            Cursor bottomCursor,
            PdfCanvas canvas,
            string cardName,
            PdfFont font,
            CardSuperType cardSuperType)
        {
            const float textPadding = 2f;
            const float textHeight = 12f;
            const float maxFontSize = 10f;
            var textRectangleHeight = topCursor.GetCurrent() - bottomCursor.GetCurrent() - textPadding * 2;
            var textFontSize = TextSharpHelpers.GetFontSize(canvas, cardName, textRectangleHeight, font, maxFontSize);
            var fontWeight = GetMainTextFontWeight(cardSuperType);
            var fontColor = GetMainTextFontColor(cardSuperType);
            var textWidthOffset = 8 + (maxFontSize - textFontSize) * .35f;
            var textRectangle = new Rectangle(
                rectangle.GetLeft() + textWidthOffset,
                bottomCursor.GetCurrent() + textPadding,
                textHeight,
                topCursor.GetCurrent() - (bottomCursor.GetCurrent() + 2 * textPadding));
            DrawText(canvas, cardName, textRectangle, -2.5f, 0f, font, fontColor, textFontSize, fontWeight);
        }

        private static void DrawBackgroundImage(
            CardSuperType superType,
            Rectangle rectangle,
            PdfCanvas canvas)
        {
            var imageNameTokens = superType.Card_type_image.Split('.');
            var imagePath = Path.Combine(CurrentPath, "Dominion", $"{imageNameTokens[0]}.{imageNameTokens[1]}");
            DrawImage(rectangle, canvas, imagePath, true, true);
        }

        private static void DrawCosts(
            PdfFont boldFont,
            DominionCard card,
            Rectangle rectangle,
            PdfCanvas canvas,
            Cursor topCursor)
        {
            const float firstCostImageHeightOffset = 3f;
            topCursor.AdvanceCursor(-firstCostImageHeightOffset);
            const float costPadding = 1f;
            var hasAnyCoinCost = !string.IsNullOrWhiteSpace(card.Cost);
            var hasZeroCoinCost = card.Cost == "0";
            var hasPotionCost = card.Potcost == 1;
            var hasDebtCost = card.Debtcost.HasValue;
            var hasAnyCost = hasAnyCoinCost || hasPotionCost || hasDebtCost;
            var hasOnlyZeroCoinCost = hasZeroCoinCost && !hasPotionCost && !hasDebtCost;
            if (hasAnyCoinCost && (!hasZeroCoinCost || hasOnlyZeroCoinCost))
                DrawCost(boldFont, rectangle, canvas, topCursor, card.Cost, costPadding);
            if (hasPotionCost)
                DrawPotionCost(rectangle, canvas, topCursor, costPadding);
            if (hasDebtCost)
                DrawDebtCost(boldFont, rectangle, canvas, topCursor, card.Debtcost, costPadding);
            if (!hasAnyCost)
                topCursor.AdvanceCursor(-costPadding);
        }

        private static void DrawCost(PdfFont font, Rectangle rectangle, PdfCanvas canvas, Cursor topCursor, string cardCost, float costPadding)
        {
            const float costFontSize = 7.5f;
            const float costTextWidthOffset = 2.5f;
            const float coinCostImageWidthOffset = 4.5f;
            const float costTextHeightOffset = 4.5f;
            const float coinCostRectangleHeight = 14.5f;
            topCursor.AdvanceCursor(-(coinCostRectangleHeight + costPadding));
            var currentCostRectangle = new Rectangle(
                rectangle.GetLeft() + coinCostImageWidthOffset,
                topCursor.GetCurrent(),
                rectangle.GetWidth() - coinCostImageWidthOffset,
                coinCostRectangleHeight);
            DrawImage(currentCostRectangle, canvas, Path.Combine(CurrentPath, "Dominion", "coin_small.png"));

            DrawText(canvas, cardCost, currentCostRectangle, costTextWidthOffset, costTextHeightOffset, font, ColorConstants.BLACK, costFontSize, FontWeight.Bold);
        }

        private static void DrawPotionCost(Rectangle rectangle, PdfCanvas canvas, Cursor topCursor, float costPadding)
        {
            const float potionCostRectangleHeight = 6f;
            const float potionCostImageWidthOffset = 6f;
            topCursor.AdvanceCursor(-(potionCostRectangleHeight + costPadding));
            var currentCostRectangle = new Rectangle(
                rectangle.GetLeft() + potionCostImageWidthOffset,
                topCursor.GetCurrent(),
                rectangle.GetWidth() - potionCostImageWidthOffset,
                potionCostRectangleHeight);
            DrawImage(currentCostRectangle, canvas, Path.Combine(CurrentPath, "Dominion", "potion.png"));
        }

        private static void DrawDebtCost(PdfFont font, Rectangle rectangle, PdfCanvas canvas, Cursor topCursor, int? debtCost, float costPadding)
        {
            const float debtCostImageWidthOffset = 5f;
            const float debtCostRectangleHeight = 13f;
            const float debtCostFontSize = 7.5f;

            topCursor.AdvanceCursor(-(debtCostRectangleHeight + costPadding));
            var currentCostRectangle = new Rectangle(
                rectangle.GetLeft() + debtCostImageWidthOffset,
                topCursor.GetCurrent(),
                rectangle.GetWidth() - debtCostImageWidthOffset,
                debtCostRectangleHeight);
            DrawImage(currentCostRectangle, canvas, Path.Combine(CurrentPath, "Dominion", "debt.png"));

            const float debtCostTextWidthOffset = 1f;
            const float debtCostTextHeightOffset = 4f;
            var costText = debtCost.ToString();
            DrawText(canvas, costText, currentCostRectangle, debtCostTextWidthOffset, debtCostTextHeightOffset, font, ColorConstants.BLACK, debtCostFontSize, FontWeight.Bold);
        }

        private static void DrawSetImageAndReturnTop(Rectangle rectangle, Cursor bottomCursor, string image, PdfCanvas canvas)
        {
            const float setImageHeight = 7f;
            const float setImageWidthOffset = 7f;
            const float setImageHeightOffset = 7f;
            var setImageRectangle = new Rectangle(
                rectangle.GetLeft() + setImageWidthOffset,
                bottomCursor.GetCurrent() + setImageHeightOffset,
                rectangle.GetWidth() - setImageWidthOffset,
                setImageHeight);
            if (!string.IsNullOrWhiteSpace(image))
                DrawImage(setImageRectangle, canvas, Path.Combine(CurrentPath, "Dominion", $"{image}"));
            bottomCursor.AdvanceCursor(setImageRectangle.GetHeight() + setImageHeightOffset);
        }

        private static void DrawText(
            PdfCanvas canvas,
            string text,
            Rectangle rectangle,
            float textWidthOffset,
            float textHeightOffset,
            PdfFont font,
            Color color,
            float size,
            FontWeight fontWeight)
        {
            const float textRotation = (float)(3 * Math.PI / 2);
            TextSharpHelpers.WriteNonWrappingTextInRectangle(canvas, text, rectangle, textWidthOffset, textHeightOffset, font, color, size, textRotation, fontWeight);
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

        private static Color GetMainTextFontColor(CardSuperType superType)
        {
            var hasBlackBackground = superType.Card_type_image == "night.png";
            return hasBlackBackground
                ? ColorConstants.WHITE
                : ColorConstants.BLACK;
        }

        private static FontWeight GetMainTextFontWeight(CardSuperType superType)
        {
            var hasBlackBackground = superType.Card_type_image == "night.png";
            return hasBlackBackground
                ? FontWeight.Bold
                : FontWeight.Regular;
        }
    }
}
