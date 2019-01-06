using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avery16282Generator.Legendary.DTO;
using Avery16282Generator.Legendary.Enums;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Avery16282Generator.Legendary
{
    public static class LegendaryLabels
    {
        public static string CreateLabels(string directory, IEnumerable<Expansion> includedSets, bool includeSpecialSetupCards)
        {
            var allHeroes = DataAccess.GetHeroCardSets().Where(set => includedSets.Contains(set.Expansion)).SelectMany(heroCardSet => heroCardSet.Heroes);
            var allMasterminds = DataAccess.GetMastermindCardSets().Where(set => includedSets.Contains(set.Expansion)).SelectMany(mastermindCardSet => mastermindCardSet.Masterminds);
            var allVillains = DataAccess.GetVillainCardSets().Where(set => includedSets.Contains(set.Expansion)).SelectMany(villainCardSet => villainCardSet.Villains);
            var allHenchmen = DataAccess.GetHenchmenCardSets().Where(set => includedSets.Contains(set.Expansion)).SelectMany(henchmenCardSet => henchmenCardSet.Henchmen);
            var allStartingCards = DataAccess.GetStartingCardSets().Where(set => includedSets.Contains(set.Expansion)).SelectMany(startingCardSet => startingCardSet.StartingCards);
            var allSetupCards = DataAccess.GetSetupCardSets()
                .Where(set => includedSets.Contains(set.Expansion))
                .SelectMany(setupCardSet => setupCardSet.SetupCards)
                .Where(setupCard => includeSpecialSetupCards || !setupCard.IsSpecialCard);
            var allVillainSetupCards = DataAccess.GetVillainSetupCardSets().Where(set => includedSets.Contains(set.Expansion)).SelectMany(setupCardSet => setupCardSet.VillainSetupCards);
            
            var mastermindBaseColor = new BaseColor(255, 61, 83);
            var villainBaseColor = new BaseColor(255, 102, 119);
            var henchmenBaseColor = new BaseColor(255, 173, 182);
            var startingCardBaseColor = new BaseColor(200, 200, 200);
            var setupCardBaseColor = new BaseColor(35, 255, 39);
            var villainSetupCardBaseColor = new BaseColor(255, 73, 197);

            var fontName = "KOMIKAX.ttf";
            var fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), fontName);
            var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var drawHeroActions = allHeroes.Select(hero => new Action<PdfContentByte, Rectangle>(
                (canvas, rectangle) =>
                {
                    var topCursor = new Cursor();
                    var bottomCursor = new Cursor();
                    const float startOfLabelOffset = 4f;
                    topCursor.AdvanceCursor(rectangle.Top - startOfLabelOffset);
                    bottomCursor.AdvanceCursor(rectangle.Bottom + startOfLabelOffset);
                    TextSharpHelpers.DrawRoundedRectangle(canvas, rectangle, new BaseColor(168, 255, 247));

                    DrawFactionsAndTypes(hero, rectangle, canvas, topCursor, bottomCursor);

                    DrawCardText(rectangle, topCursor, bottomCursor, canvas, hero.Name, baseFont);
                    DrawSetText(rectangle, topCursor, bottomCursor, canvas, hero.Expansion.GetExpansionName(), baseFont);
                }
            )).ToList();
            var drawMastermindActions = allMasterminds.Select(mastermind =>
                CreateActionToDrawNameAndSet(
                    mastermind.Name,
                    mastermind.Expansion.GetExpansionName(),
                    baseFont,
                    mastermindBaseColor
                ));
            var drawVillainActions = allVillains.Select(villain =>
                CreateActionToDrawNameAndSet(
                    villain.Name,
                    villain.Expansion.GetExpansionName(),
                    baseFont,
                    villainBaseColor
                ));
            var drawHenchmenActions = allHenchmen.Select(henchmen =>
                CreateActionToDrawNameAndSet(
                    henchmen.Name,
                    henchmen.Expansion.GetExpansionName(),
                    baseFont,
                    henchmenBaseColor
                ));
            var drawStartingCardsActions = allStartingCards.Select(startingCard =>
                CreateActionToDrawNameAndSet(
                    startingCard.Name,
                    startingCard.Expansion.GetExpansionName(),
                    baseFont,
                    startingCardBaseColor
                ));
            var drawSetupCardsActions = allSetupCards.Select(setupCard =>
                CreateActionToDrawNameAndSet(
                    setupCard.Name,
                    setupCard.Expansion.GetExpansionName(),
                    baseFont,
                    setupCardBaseColor
                ));
            var drawVillainSetupCardsActions = allVillainSetupCards.Select(villainSetupCard =>
                CreateActionToDrawNameAndSet(
                    villainSetupCard.Name,
                    villainSetupCard.Expansion.GetExpansionName(),
                    baseFont,
                    villainSetupCardBaseColor
                ));


            var allActions = drawHeroActions
                .Concat(drawMastermindActions)
                .Concat(drawVillainActions)
                .Concat(drawHenchmenActions)
                .Concat(drawStartingCardsActions)
                .Concat(drawSetupCardsActions)
                .Concat(drawVillainSetupCardsActions);

            var drawActionRectangleQueue = new Queue<Action<PdfContentByte, Rectangle>>(allActions);
            return PdfGenerator.DrawRectangles(drawActionRectangleQueue, BaseColor.WHITE, directory, "Legendary");
        }

        private static Action<PdfContentByte, Rectangle> CreateActionToDrawNameAndSet(string name, string set, BaseFont baseFont, BaseColor baseColor)
        {
            return (canvas, rectangle) =>
            {
                var topCursor = new Cursor();
                var bottomCursor = new Cursor();
                const float startOfLabelOffset = 4f;
                topCursor.AdvanceCursor(rectangle.Top - startOfLabelOffset);
                bottomCursor.AdvanceCursor(rectangle.Bottom + startOfLabelOffset);
                TextSharpHelpers.DrawRoundedRectangle(canvas, rectangle, baseColor);
                DrawCardText(rectangle, topCursor, bottomCursor, canvas, name, baseFont);
                DrawSetText(rectangle, topCursor, bottomCursor, canvas, set, baseFont);
            };
        }

        private static void DrawFactionsAndTypes(Hero hero, Rectangle rectangle, PdfContentByte canvas, Cursor topCursor, Cursor bottomCursor)
        {
            foreach (var faction in hero.Factions)
            {
                DrawFaction(rectangle, canvas, topCursor, faction);
            }

            var leftHalfRectangle = new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Left + rectangle.Width/2, rectangle.Top);
            var rightHalfRectangle = new Rectangle(rectangle.Left + rectangle.Width/2, rectangle.Bottom, rectangle.Right, rectangle.Top);
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

        private static void DrawCompositeType(IList<HeroCardType> heroCardTypes, Rectangle rectangle, PdfContentByte canvas, Cursor bottomCursor)
        {
            if (heroCardTypes.Count != 2)
                throw new InvalidOperationException("Cannot have more than two card types for a single card.");
            var orderedTypes = heroCardTypes.OrderBy(type => type).ToList();
            var imagePath = $"Legendary\\Images\\Types\\{orderedTypes[0]}-{orderedTypes[1]}.png";
            DrawCardType(rectangle, canvas, bottomCursor, imagePath);
        }

        private static void DrawSingleCardType(HeroCardType heroCardType, Rectangle rectangle, PdfContentByte canvas, Cursor bottomCursor)
        {
            var imagePath = $"Legendary\\Images\\Types\\{heroCardType}.png";

            DrawCardType(rectangle, canvas, bottomCursor, imagePath);
        }

        private static void DrawCardType(Rectangle rectangle, PdfContentByte canvas, Cursor bottomCursor, string imagePath)
        {
            const float cardImageHeight = 10f;
            const float cardImageHeightPadding = 3f;
            var cardRectangle = new Rectangle(
                rectangle.Left,
                bottomCursor.GetCurrent(),
                rectangle.Right,
                bottomCursor.GetCurrent() + cardImageHeight);
            DrawImage(cardRectangle, canvas, imagePath,
                centerHorizontally: true);

            bottomCursor.AdvanceCursor(cardRectangle.Height + cardImageHeightPadding);
        }

        private static void DrawFaction(Rectangle rectangle, PdfContentByte canvas, Cursor topCursor, HeroFaction heroFaction)
        {
            const float factionImageHeight = 10f;
            const float factionImageHeightPadding = 3f;
            var factionRectangle = new Rectangle(
                rectangle.Left,
                topCursor.GetCurrent() - factionImageHeight,
                rectangle.Right,
                topCursor.GetCurrent());
            if (heroFaction != HeroFaction.Unaffiliated)
            {
                DrawImage(factionRectangle, canvas, $"Legendary\\Images\\Factions\\{heroFaction}.png",
                    centerHorizontally: true);
            }

            topCursor.AdvanceCursor(-(factionRectangle.Height + factionImageHeightPadding));
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

        private static void DrawCardText(
            Rectangle rectangle,
            Cursor topCursor,
            Cursor bottomCursor, 
            PdfContentByte canvas,
            string heroName,
            BaseFont baseFont)
        {
            const float textPadding = 0f;
            const float maxFontSize = 10f;
            var potentialTextRectangleHeight = topCursor.GetCurrent() - bottomCursor.GetCurrent(); // - textPadding * 2
            var textFontSize = TextSharpHelpers.GetFontSize(canvas, heroName, potentialTextRectangleHeight, baseFont, maxFontSize, Element.ALIGN_LEFT, Font.NORMAL);
            var font = new Font(baseFont, textFontSize, Font.NORMAL, BaseColor.BLACK);
            var textRectangleHeight = textFontSize * .9f;
            var textWidthOffset = rectangle.Width/2.0f - textRectangleHeight / 2.0f;
            var textRectangle = new Rectangle(
                rectangle.Left + textWidthOffset,
                bottomCursor.GetCurrent() + textPadding,
                rectangle.Left + textWidthOffset + textRectangleHeight,
                topCursor.GetCurrent() - textPadding);
            DrawText(canvas, heroName, textRectangle, 0, 0, font);
        }

        private static void DrawSetText(
            Rectangle rectangle,
            Cursor topCursor,
            Cursor bottomCursor,
            PdfContentByte canvas,
            string heroName,
            BaseFont baseFont)
        {
            const float maxFontSize = 4f;
            var potentialTextRectangleHeight = topCursor.GetCurrent() - bottomCursor.GetCurrent(); // - textPadding * 2
            var textFontSize = TextSharpHelpers.GetFontSize(canvas, heroName, potentialTextRectangleHeight, baseFont, maxFontSize, Element.ALIGN_LEFT, Font.NORMAL);
            var font = new Font(baseFont, textFontSize, Font.NORMAL, BaseColor.BLACK);
            var textRectangleHeight = textFontSize * .9f;
            var textWidthOffset = 2f;
            var textRectangle = new Rectangle(
                rectangle.Left + textWidthOffset,
                bottomCursor.GetCurrent(),
                rectangle.Left + textWidthOffset + textRectangleHeight,
                topCursor.GetCurrent());
            DrawText(canvas, heroName, textRectangle, 0, 0, font);
        }

        private static void DrawText(PdfContentByte canvas, string text, Rectangle rectangle,
            float textWidthOffset, float textHeightOffset, Font font)
        {
            const int textRotation = 270;
            TextSharpHelpers.WriteNonWrappingTextInRectangle(canvas, text, rectangle, textWidthOffset, textHeightOffset, font, textRotation);
        }
    }
}
