using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Avery16282Generator.Legendary
{
    public static class LegendaryLabels
    {
        public static void CreateLabels()
        {
            var cardsSets = DataAccess.GetCardSets();
            var allHeroes = cardsSets.SelectMany(card => card.Heroes);
            var fontName = "KOMIKAX.ttf";
            var fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), fontName);
            //var labelBackground = new CMYKColor(100, 100, 100, 100);
            //var boldFontName = "TrajanPro-Bold.otf";
            //var boldFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), boldFontName);
            var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            //var boldBaseFont = BaseFont.CreateFont(boldFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var drawActionRectangles = allHeroes.SelectMany(hero => new List<Action<PdfContentByte, Rectangle>>
            {
                (canvas, rectangle) =>
                {
                    var topCursor = new Cursor();
                    const float startOfLabelOffset = 4f;
                    topCursor.AdvanceCursor(rectangle.Top - startOfLabelOffset);
                    TextSharpHelpers.DrawRectangle(canvas, rectangle, BaseColor.CYAN);

                    //DrawBackgroundImage(card.SuperType, rectangle, canvas, topCursor, bottomCursor);
                    DrawFactionAndTypes(hero, rectangle, canvas, topCursor);
                    //DrawSetImageAndReturnTop(rectangle, bottomCursor, card.Set.Image, canvas);

                    //var cardName = card.GroupName ?? card.Name;
                    //DrawCardText(rectangle, topCursor, bottomCursor, canvas, cardName, baseFont, card.SuperType);
                    DrawCardText(rectangle, topCursor, canvas, hero.Name, baseFont);
                }
            }).ToList();
            var drawActionRectangleQueue = new Queue<Action<PdfContentByte, Rectangle>>(drawActionRectangles);
            PdfGenerator.DrawRectangles(drawActionRectangleQueue, BaseColor.WHITE, "Legendary");
        }

        private static void DrawFactionAndTypes(Hero hero, Rectangle rectangle, PdfContentByte canvas, Cursor topCursor)
        {
            const float barrelImageHeight = 12f;
            const float barrelImageHeightPadding = 3f;
            var barrelRectangle = new Rectangle(
                rectangle.Left,
                topCursor.GetCurrent() - barrelImageHeight,
                rectangle.Right,
                topCursor.GetCurrent());
            //TODO: Don't just draw the first faction
            //TODO: Draw types
            //TODO: Remove this check and get valid iamges for these factions
            var heroFaction = hero.Factions.First();
            if (!(heroFaction == HeroFaction.Unaffiliated || heroFaction == HeroFaction.Champions || heroFaction == HeroFaction.MercsForMoney))
            {
                DrawImage(barrelRectangle, canvas, $"Legendary\\Images\\Factions\\{heroFaction}.png", centerHorizontally: true);
            }

            topCursor.AdvanceCursor(-(barrelRectangle.Height + barrelImageHeightPadding));
        }

        private static void DrawImage(
            Rectangle rectangle,
            PdfContentByte canvas,
            string imagePath,
            bool scaleAbsolute = false,
            bool centerVertically = false,
            bool centerHorizontally = false)
        {
            const float imageRotationInRadians = 4.71239f;
            TextSharpHelpers.DrawImage(rectangle, canvas, imagePath, imageRotationInRadians, scaleAbsolute, centerVertically, centerHorizontally);
        }

        private static void DrawCardText(Rectangle rectangle, Cursor topCursor, 
            PdfContentByte canvas, string heroName, BaseFont baseFont)
        {
            const float textPadding = 2f;
            const float textHeight = 12f;
            const float maxFontSize = 10f;
            var textRectangleHeight = topCursor.GetCurrent() - textPadding * 2;
            var textFontSize = TextSharpHelpers.GetFontSize(canvas, heroName, textRectangleHeight, baseFont, maxFontSize, Element.ALIGN_LEFT, Font.NORMAL);
            var font = new Font(baseFont, textFontSize, Font.NORMAL, BaseColor.BLACK);
            
            var textWidthOffset = 8 + (maxFontSize - font.Size) * .35f;
            var textRectangle = new Rectangle(
                rectangle.Left + textWidthOffset,
                textPadding,
                rectangle.Left + textWidthOffset + textHeight,
                topCursor.GetCurrent() - textPadding);
            DrawText(canvas, heroName, textRectangle, 0, 0, font);
        }

        private static void DrawText(PdfContentByte canvas, string text, Rectangle rectangle,
            float textWidthOffset, float textHeightOffset, Font font)
        {
            const int textRotation = 270;
            TextSharpHelpers.WriteNonWrappingTextInRectangle(canvas, text, rectangle, textWidthOffset, textHeightOffset, font, textRotation, Element.ALIGN_LEFT);
        }
    }
}
