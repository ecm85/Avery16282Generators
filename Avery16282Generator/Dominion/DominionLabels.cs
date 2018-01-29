using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Avery16282Generator.Dominion
{
    public static class DominionLabels
    {
        public static void CreateLabels()
        {
            var cardsToPrint = DominionCardDataAccess.GetCardsToPrint();

            var minionProRegular = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "TrajanPro-Regular.otf");
            var minionProBold = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "TrajanPro-Bold.otf");
            var baseFont = BaseFont.CreateFont(minionProRegular, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var boldBaseFont = BaseFont.CreateFont(minionProBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            var drawActionRectangles = cardsToPrint.SelectMany(card => new List<Action<PdfContentByte, Rectangle>>
            {
                (canvas, rectangle) =>
                {
                    var backgroundImage = DrawBackgroundImage(card.SuperType, rectangle, canvas);
                    var backgroundImageBottom = backgroundImage.AbsoluteY;
                    var backgroundImageTop = backgroundImageBottom + backgroundImage.ScaledHeight;
                    var currentCostBottom = DrawCostsAndReturnBottom(boldBaseFont, card, rectangle, canvas, backgroundImageTop);
                    var setImageTop = DrawSetImageAndReturnTop(rectangle, backgroundImageBottom, card.Set.Image, canvas);

                    var cardName = card.GroupName ?? card.Name;
                    DrawCardText(rectangle, setImageTop, currentCostBottom, canvas, cardName, baseFont, card.SuperType);
                }
            }).ToList();
            var drawActionRectangleQueue = new Queue<Action<PdfContentByte, Rectangle>>(drawActionRectangles);
            PdfGenerator.DrawRectangles(drawActionRectangleQueue, BaseColor.WHITE, "Dominion");
        }

        private static void DrawCardText(Rectangle rectangle, float setImageTop, float currentCostBottom,
            PdfContentByte canvas, string cardName, BaseFont baseFont, CardSuperType cardSuperType)
        {
            const float textPadding = 2f;
            const float textWidthOffset = 9f;
            const float textHeight = 8f;
            const float maxFontSize = 10f;

            var textRectangle = new Rectangle(
                rectangle.Left + textWidthOffset,
                setImageTop + textPadding,
                rectangle.Left + textWidthOffset + textHeight,
                currentCostBottom - textPadding);
            var font = GetMainTextFont(textRectangle, canvas, cardName, baseFont, maxFontSize, cardSuperType);
            DrawText(canvas, cardName, textRectangle, 0, 0, font);
        }

        private static Image DrawBackgroundImage(CardSuperType superType, Rectangle rectangle, PdfContentByte canvas)
        {
            var imageNameTokens = superType.Card_type_image.Split('.');
            var imagePath = $@"Dominion\{imageNameTokens[0]}_nc.{imageNameTokens[1]}";
            return DrawImage(rectangle, canvas, imagePath, true, true);
        }

        private static float DrawCostsAndReturnBottom(BaseFont boldBaseFont, DominionCard card, Rectangle rectangle,
            PdfContentByte canvas, float backgroundImageTop)
        {
            const float firstCostImageHeightOffset = 1.5f;
            var currentCostBottom = backgroundImageTop - firstCostImageHeightOffset;
            const float costPadding = 1f;

            if (!string.IsNullOrWhiteSpace(card.Cost) && (card.Cost != "0" ||
                                                          (card.Cost == "0" && card.Potcost != 1 && !card.Debtcost.HasValue)))
                currentCostBottom = DrawCost(boldBaseFont, rectangle, canvas, currentCostBottom, card.Cost, costPadding);
            if (card.Potcost == 1)
                currentCostBottom = DrawPotionCost(rectangle, canvas, currentCostBottom, costPadding);
            if (card.Debtcost.HasValue)
                currentCostBottom = DrawDebtCost(boldBaseFont, rectangle, canvas, currentCostBottom, card.Debtcost, costPadding);
            return currentCostBottom;
        }

        private static float DrawCost(BaseFont boldBaseFont, Rectangle rectangle, PdfContentByte canvas, float currentCostBottom, string cardCost, float costPadding)
        {
            const float costFontSize = 7.5f;
            const float costTextWidthOffset = 3.5f;
            const float coinCostImageWidthOffset = 5f;
            const float costTextHeightOffset = 3.5f;
            const float coinCostRectangleHeight = 13f;
            currentCostBottom = currentCostBottom - (coinCostRectangleHeight + costPadding);
            var currentCostRectangle = new Rectangle(rectangle.Left + coinCostImageWidthOffset, currentCostBottom,
                rectangle.Right, currentCostBottom + coinCostRectangleHeight);
            DrawImage(currentCostRectangle, canvas, @"Dominion\coin_small.png");

            var font = new Font(boldBaseFont, costFontSize, Font.BOLD, BaseColor.BLACK);
            DrawText(canvas, cardCost, currentCostRectangle, costTextWidthOffset, costTextHeightOffset, font);
            return currentCostBottom;
        }

        private static float DrawPotionCost(Rectangle rectangle, PdfContentByte canvas, float currentCostBottom, float costPadding)
        {
            const float potionCostRectangleHeight = 6f;
            const float potionCostImageWidthOffset = 6f;
            currentCostBottom = currentCostBottom - (potionCostRectangleHeight + costPadding);
            var currentCostRectangle = new Rectangle(rectangle.Left + potionCostImageWidthOffset, currentCostBottom,
                rectangle.Right, currentCostBottom + potionCostRectangleHeight);
            DrawImage(currentCostRectangle, canvas, @"Dominion\potion.png");
            return currentCostBottom;
        }

        private static float DrawDebtCost(BaseFont boldBaseFont, Rectangle rectangle, PdfContentByte canvas, float currentCostBottom, int? debtCost, float costPadding)
        {
            const float debtCostImageWidthOffset = 5f;
            const float debtCostRectangleHeight = 13f;
            const float debtCostFontSize = 7.5f;

            currentCostBottom = currentCostBottom - (debtCostRectangleHeight + costPadding);
            var currentCostRectangle = new Rectangle(rectangle.Left + debtCostImageWidthOffset, currentCostBottom,
                rectangle.Right, currentCostBottom + debtCostRectangleHeight);
            DrawImage(currentCostRectangle, canvas, @"Dominion\debt.png");

            const float debtCostTextWidthOffset = 3.5f;
            const float debtCostTextHeightOffset = 4f;
            var costText = debtCost.ToString();
            var font = new Font(boldBaseFont, debtCostFontSize, Font.BOLD, BaseColor.BLACK);
            DrawText(canvas, costText, currentCostRectangle, debtCostTextWidthOffset, debtCostTextHeightOffset, font);
            return currentCostBottom;
        }

        private static float DrawSetImageAndReturnTop(Rectangle rectangle, float backgroundImageBottom, string image, PdfContentByte canvas)
        {
            const float setImageHeight = 7f;
            const float setImageWidthOffset = 7f;
            const float setImageHeightOffset = 4.5f;
            var setImageRectangle = new Rectangle(rectangle.Left + setImageWidthOffset,
                backgroundImageBottom + setImageHeightOffset,
                rectangle.Right,
                backgroundImageBottom + setImageHeightOffset + setImageHeight);
            if (!string.IsNullOrWhiteSpace(image))
                DrawImage(setImageRectangle, canvas, $@"Dominion\{image}");
            return setImageRectangle.Top;
        }

        private static void DrawText(PdfContentByte canvas, string text, Rectangle rectangle,
            float textWidthOffset, float textHeightOffset, Font font)
        {
            const int textRotation = 270;
            TextSharpHelpers.DrawText(canvas, text, rectangle, textWidthOffset, textHeightOffset, font, textRotation);
        }

        private static Image DrawImage(Rectangle rectangle, PdfContentByte canvas, string imagePath, bool scaleAbsolute = false, bool center = false)
        {
            const float imageRotationInRadians = 4.71239f;
            return TextSharpHelpers.DrawImage(rectangle, canvas, imagePath, imageRotationInRadians, scaleAbsolute, center);
        }

        private static Font GetMainTextFont(Rectangle textRectangle, PdfContentByte canvas, string cardName,
            BaseFont baseFont, float maxFontSize, CardSuperType superType)
        {
            var rotatedRectangle = new Rectangle(textRectangle.Left, textRectangle.Bottom,
                textRectangle.Left + textRectangle.Height, textRectangle.Bottom + textRectangle.Width);
            var textFontSize = TextSharpHelpers.GetFontSize(canvas, cardName, rotatedRectangle, baseFont, maxFontSize,
                Element.ALIGN_LEFT, Font.NORMAL);
            var fontColor = superType.Card_type_image == "night.png"
                ? BaseColor.WHITE
                : BaseColor.BLACK;
            var font = new Font(baseFont, textFontSize, Font.NORMAL, fontColor);
            return font;
        }
    }
}
