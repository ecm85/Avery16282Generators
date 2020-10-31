using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avery16282Generator.Legendary.DTO;
using Avery16282Generator.Legendary.Enums;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using Path = System.IO.Path;

namespace Avery16282Generator.Legendary
{
    public static class LegendaryLabels
    {
        public static string CurrentPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static byte[] CreateLabels(IEnumerable<Expansion> selectedExpansions, bool includeSpecialSetupCards)
        {
            var allHeroes = DataAccess.GetHeroCardSets().Where(set => selectedExpansions.Contains(set.Expansion)).SelectMany(heroCardSet => heroCardSet.Heroes);
            var allMasterminds = DataAccess.GetMastermindCardSets().Where(set => selectedExpansions.Contains(set.Expansion)).SelectMany(mastermindCardSet => mastermindCardSet.Masterminds);
            var allVillains = DataAccess.GetVillainCardSets().Where(set => selectedExpansions.Contains(set.Expansion)).SelectMany(villainCardSet => villainCardSet.Villains);
            var allHenchmen = DataAccess.GetHenchmenCardSets().Where(set => selectedExpansions.Contains(set.Expansion)).SelectMany(henchmenCardSet => henchmenCardSet.Henchmen);
            var allStartingCards = DataAccess.GetStartingCardSets().Where(set => selectedExpansions.Contains(set.Expansion)).SelectMany(startingCardSet => startingCardSet.StartingCards);
            var allSetupCards = DataAccess.GetSetupCardSets()
                .Where(set => selectedExpansions.Contains(set.Expansion))
                .SelectMany(setupCardSet => setupCardSet.SetupCards)
                .Where(setupCard => includeSpecialSetupCards || !setupCard.IsSpecialCard);
            var allVillainSetupCards = DataAccess.GetVillainSetupCardSets().Where(set => selectedExpansions.Contains(set.Expansion)).SelectMany(setupCardSet => setupCardSet.VillainSetupCards);

            var mastermindBaseColor = new DeviceRgb(255, 61, 83);
            var villainBaseColor = new DeviceRgb(255, 102, 119);
            var henchmenBaseColor = new DeviceRgb(255, 173, 182);
            var startingCardBaseColor = new DeviceRgb(200, 200, 200);
            var setupCardBaseColor = new DeviceRgb(35, 255, 39);
            var villainSetupCardBaseColor = new DeviceRgb(255, 73, 197);

            var fontPath = Path.Combine(CurrentPath, "Fonts", "KOMIKAX.TTF");
            var font = PdfFontFactory.CreateFont(fontPath, true);
            var drawHeroActions = allHeroes.Select(hero => new Action<PdfCanvas, Rectangle>(
                (canvas, rectangle) =>
                {
                    var topCursor = new Cursor();
                    var bottomCursor = new Cursor();
                    const float startOfLabelOffset = 4f;
                    topCursor.AdvanceCursor(rectangle.GetTop() - startOfLabelOffset);
                    bottomCursor.AdvanceCursor(rectangle.GetBottom() + startOfLabelOffset);
                    TextSharpHelpers.DrawRoundedRectangle(canvas, rectangle, new DeviceRgb(168, 255, 247));

                    DrawFactionsAndTypes(hero, rectangle, canvas, topCursor, bottomCursor);

                    DrawCardText(rectangle, topCursor, bottomCursor, canvas, hero.Name, font);
                    DrawSetText(rectangle, topCursor, bottomCursor, canvas, hero.Expansion.GetExpansionName(), font);
                }
            )).ToList();
            var drawMastermindActions = allMasterminds.Select(mastermind =>
                CreateActionToDrawNameAndSet(
                    mastermind.Name,
                    mastermind.Expansion.GetExpansionName(),
                    font,
                    mastermindBaseColor
                ));
            var drawVillainActions = allVillains.Select(villain =>
                CreateActionToDrawNameAndSet(
                    villain.Name,
                    villain.Expansion.GetExpansionName(),
                    font,
                    villainBaseColor
                ));
            var drawHenchmenActions = allHenchmen.Select(henchmen =>
                CreateActionToDrawNameAndSet(
                    henchmen.Name,
                    henchmen.Expansion.GetExpansionName(),
                    font,
                    henchmenBaseColor
                ));
            var drawStartingCardsActions = allStartingCards.Select(startingCard =>
                CreateActionToDrawNameAndSet(
                    startingCard.Name,
                    startingCard.Expansion.GetExpansionName(),
                    font,
                    startingCardBaseColor
                ));
            var drawSetupCardsActions = allSetupCards.Select(setupCard =>
                CreateActionToDrawNameAndSet(
                    setupCard.Name,
                    setupCard.Expansion.GetExpansionName(),
                    font,
                    setupCardBaseColor
                ));
            var drawVillainSetupCardsActions = allVillainSetupCards.Select(villainSetupCard =>
                CreateActionToDrawNameAndSet(
                    villainSetupCard.Name,
                    villainSetupCard.Expansion.GetExpansionName(),
                    font,
                    villainSetupCardBaseColor
                ));


            var allActions = drawHeroActions
                .Concat(drawMastermindActions)
                .Concat(drawVillainActions)
                .Concat(drawHenchmenActions)
                .Concat(drawStartingCardsActions)
                .Concat(drawSetupCardsActions)
                .Concat(drawVillainSetupCardsActions);

            var drawActionRectangleQueue = new Queue<Action<PdfCanvas, Rectangle>>(allActions);
            return PdfGenerator.DrawRectangles(drawActionRectangleQueue, ColorConstants.WHITE);
        }

        private static Action<PdfCanvas, Rectangle> CreateActionToDrawNameAndSet(string name, string set, PdfFont baseFont, Color color)
        {
            return (canvas, rectangle) =>
            {
                var topCursor = new Cursor();
                var bottomCursor = new Cursor();
                const float startOfLabelOffset = 4f;
                topCursor.AdvanceCursor(rectangle.GetTop() - startOfLabelOffset);
                bottomCursor.AdvanceCursor(rectangle.GetBottom() + startOfLabelOffset);
                TextSharpHelpers.DrawRoundedRectangle(canvas, rectangle, color);
                DrawCardText(rectangle, topCursor, bottomCursor, canvas, name, baseFont);
                DrawSetText(rectangle, topCursor, bottomCursor, canvas, set, baseFont);
            };
        }

        private static void DrawFactionsAndTypes(Hero hero, Rectangle rectangle, PdfCanvas canvas, Cursor topCursor, Cursor bottomCursor)
        {
            foreach (var faction in hero.Factions)
            {
                DrawFaction(rectangle, canvas, topCursor, faction);
            }

            var leftHalfRectangle = new Rectangle(rectangle.GetLeft(), rectangle.GetBottom(), rectangle.GetWidth() / 2, rectangle.GetHeight());
            var rightHalfRectangle = new Rectangle(rectangle.GetLeft() + rectangle.GetWidth() / 2, rectangle.GetBottom(), rectangle.GetWidth() / 2, rectangle.GetHeight());
            var secondaryBottomCursor = new Cursor();
            secondaryBottomCursor.AdvanceCursor(bottomCursor.GetCurrent());
            for (var i = 0; i < hero.Cards.Count; i++)
            {
                var card = hero.Cards[i];
                var halfRectangleToUse = i % 2 != 0 ? leftHalfRectangle : rightHalfRectangle;
                var cursorToUse = i % 2 != 0 ? bottomCursor : secondaryBottomCursor;
                var cardTypes = card.HeroCardSection1.HeroCardTypes;
                if (card.HeroCardSection2 != null)
                    cardTypes = cardTypes.Concat(card.HeroCardSection2.HeroCardTypes).ToList();
                if (cardTypes.Count == 1)
                    DrawSingleCardType(cardTypes.Single(), halfRectangleToUse, canvas, cursorToUse);
                else
                {
                    DrawCompositeType(cardTypes, halfRectangleToUse, canvas, cursorToUse);
                }
            }
        }

        private static void DrawCompositeType(IList<HeroCardType> heroCardTypes, Rectangle rectangle, PdfCanvas canvas, Cursor bottomCursor)
        {
            if (heroCardTypes.Count != 2)
                throw new InvalidOperationException("Cannot have more than two card types for a single card.");
            if (heroCardTypes[0] == heroCardTypes[1])
                DrawSingleCardType(heroCardTypes[0], rectangle, canvas, bottomCursor);
            else
            {
                var orderedTypes = heroCardTypes.OrderBy(type => type).ToList();
                var imagePath = Path.Combine(CurrentPath, "Legendary", "images", "Types", $"{orderedTypes[0]}-{orderedTypes[1]}.png");
                DrawCardType(rectangle, canvas, bottomCursor, imagePath);
            }
        }

        private static void DrawSingleCardType(HeroCardType heroCardType, Rectangle rectangle, PdfCanvas canvas, Cursor bottomCursor)
        {
            var imagePath = Path.Combine(CurrentPath, "Legendary", "images", "Types", $"{heroCardType}.png");

            DrawCardType(rectangle, canvas, bottomCursor, imagePath);
        }

        private static void DrawCardType(Rectangle rectangle, PdfCanvas canvas, Cursor bottomCursor, string imagePath)
        {
            const float cardImageHeight = 10f;
            const float cardImageHeightPadding = 3f;
            var cardRectangle = new Rectangle(
                rectangle.GetLeft(),
                bottomCursor.GetCurrent(),
                rectangle.GetWidth(),
                cardImageHeight);
            DrawImage(cardRectangle, canvas, imagePath,
                centerHorizontally: true);

            bottomCursor.AdvanceCursor(cardRectangle.GetHeight() + cardImageHeightPadding);
        }

        private static void DrawFaction(Rectangle rectangle, PdfCanvas canvas, Cursor topCursor, HeroFaction heroFaction)
        {
            const float factionImageHeight = 10f;
            const float factionImageHeightPadding = 3f;
            var factionRectangle = new Rectangle(
                rectangle.GetLeft(),
                topCursor.GetCurrent() - factionImageHeight,
                rectangle.GetWidth(),
                factionImageHeight);
            if (heroFaction != HeroFaction.Unaffiliated)
            {
                DrawImage(
                    factionRectangle,
                    canvas,
                    Path.Combine(CurrentPath, "Legendary", "images", "Factions", $"{heroFaction}.png"),
                    centerHorizontally: true);
            }

            topCursor.AdvanceCursor(-(factionRectangle.GetHeight() + factionImageHeightPadding));
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

        private static void DrawCardText(
            Rectangle rectangle,
            Cursor topCursor,
            Cursor bottomCursor,
            PdfCanvas canvas,
            string heroName,
            PdfFont font)
        {
            const float textPadding = 0f;
            const float maxFontSize = 10f;
            var potentialTextRectangleHeight = topCursor.GetCurrent() - bottomCursor.GetCurrent(); // - textPadding * 2
            var textFontSize = TextSharpHelpers.GetFontSize(canvas, heroName, potentialTextRectangleHeight, font, maxFontSize);
            var textRectangleHeight = textFontSize * .9f;
            var textWidthOffset = rectangle.GetWidth() / 2.0f - textRectangleHeight / 2.0f;
            var textRectangle = new Rectangle(
                rectangle.GetLeft() + textWidthOffset,
                bottomCursor.GetCurrent() + textPadding,
                textRectangleHeight,
                topCursor.GetCurrent() - (bottomCursor.GetCurrent() + textPadding * 2));
            DrawText(canvas, heroName, textRectangle, -2f, 0, font, ColorConstants.BLACK, textFontSize);
        }

        private static void DrawSetText(
            Rectangle rectangle,
            Cursor topCursor,
            Cursor bottomCursor,
            PdfCanvas canvas,
            string heroName,
            PdfFont font)
        {
            const float maxFontSize = 4f;
            var potentialTextRectangleHeight = topCursor.GetCurrent() - bottomCursor.GetCurrent(); // - textPadding * 2
            var textFontSize = TextSharpHelpers.GetFontSize(canvas, heroName, potentialTextRectangleHeight, font, maxFontSize);
            var textRectangleHeight = textFontSize * .9f;
            var textWidthOffset = 2f;
            var textRectangle = new Rectangle(
                rectangle.GetLeft() + textWidthOffset,
                bottomCursor.GetCurrent(),
                textRectangleHeight,
                topCursor.GetCurrent()  - bottomCursor.GetCurrent());
            DrawText(canvas, heroName, textRectangle, -1f, 0, font, ColorConstants.BLACK, textFontSize);
        }

        private static void DrawText(
            PdfCanvas canvas,
            string text,
            Rectangle rectangle,
            float textWidthOffset,
            float textHeightOffset,
            PdfFont font,
            Color color,
            float size)
        {
            const float textRotation = (float)(3 * Math.PI / 2);
            TextSharpHelpers.WriteNonWrappingTextInRectangle(canvas, text, rectangle, textWidthOffset, textHeightOffset, font, color, size, textRotation, FontWeight.Regular);
        }
    }
}
