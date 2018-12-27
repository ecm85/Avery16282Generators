using System;
using System.Collections.Generic;
using System.Configuration;
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
        public static void CreateLabels()
        {
            //TODO:
            //Schemes (Plots)
            //Scheme(Plot) Twists
            //Master(Command) Strikes

            //TODO: Configurable to print 'special' labels
            var includedSets = ConfigurationManager.AppSettings["LegendarySetsToUse"].Split(',').Select(set => set.ToLower());
            var allHeroes = DataAccess.GetHeroCardSets().Where(set => includedSets.Contains(set.SetName.ToLower())).SelectMany(heroCardSet => heroCardSet.Heroes);
            var allMasterminds = DataAccess.GetMastermindCardSets().Where(set => includedSets.Contains(set.SetName.ToLower())).SelectMany(mastermindCardSet => mastermindCardSet.Masterminds);
            var allVillains = DataAccess.GetVillainCardSets().Where(set => includedSets.Contains(set.SetName.ToLower())).SelectMany(villainCardSet => villainCardSet.Villains);
            var allHenchmen = DataAccess.GetHenchmenCardSets().Where(set => includedSets.Contains(set.SetName.ToLower())).SelectMany(henchmenCardSet => henchmenCardSet.Henchmen);
            var allStartingCards = DataAccess.GetStartingCardSets().Where(set => includedSets.Contains(set.SetName.ToLower())).SelectMany(startingCardSet => startingCardSet.StartingCards);
            var allSetupCards = DataAccess.GetSetupCardSets().Where(set => includedSets.Contains(set.SetName.ToLower())).SelectMany(setupCardSet => setupCardSet.SetupCards);

            var fontName = "KOMIKAX.ttf";
            var fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), fontName);
            //var labelBackground = new CMYKColor(100, 100, 100, 100);
            //var boldFontName = "TrajanPro-Bold.otf";
            //var boldFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), boldFontName);
            var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            //var boldBaseFont = BaseFont.CreateFont(boldFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var drawHeroActions = allHeroes.SelectMany(hero => new List<Action<PdfContentByte, Rectangle>>
            {
                (canvas, rectangle) =>
                {
                    var topCursor = new Cursor();
                    var bottomCursor = new Cursor();
                    const float startOfLabelOffset = 4f;
                    topCursor.AdvanceCursor(rectangle.Top - startOfLabelOffset);
                    bottomCursor.AdvanceCursor(rectangle.Bottom + startOfLabelOffset);
                    TextSharpHelpers.DrawRoundedRectangle(canvas, rectangle, new BaseColor(168,255,247));

                    //DrawBackgroundImage(card.SuperType, rectangle, canvas, topCursor, bottomCursor);
                    DrawFactionsAndTypes(hero, rectangle, canvas, topCursor, bottomCursor);
                    //DrawSetImageAndReturnTop(rectangle, bottomCursor, card.Set.Image, canvas);

                    //var cardName = card.GroupName ?? card.Name;
                    //DrawCardText(rectangle, topCursor, bottomCursor, canvas, cardName, baseFont, card.SuperType);
                    DrawCardText(rectangle, topCursor, bottomCursor, canvas, hero.Name, baseFont);
                    DrawSetText(rectangle, topCursor, bottomCursor, canvas, hero.Set, baseFont);
                }
            }).ToList();
            var drawMastermindActions = allMasterminds.SelectMany(mastermind => new List<Action<PdfContentByte, Rectangle>>
            {
                (canvas, rectangle) =>
                {
                    var topCursor = new Cursor();
                    var bottomCursor = new Cursor();
                    const float startOfLabelOffset = 4f;
                    topCursor.AdvanceCursor(rectangle.Top - startOfLabelOffset);
                    bottomCursor.AdvanceCursor(rectangle.Bottom + startOfLabelOffset);
                    TextSharpHelpers.DrawRoundedRectangle(canvas, rectangle, new BaseColor(255,61,83));

                    //DrawBackgroundImage(card.SuperType, rectangle, canvas, topCursor, bottomCursor);
                    //DrawSetImageAndReturnTop(rectangle, bottomCursor, card.Set.Image, canvas);

                    //var cardName = card.GroupName ?? card.Name;
                    //DrawCardText(rectangle, topCursor, bottomCursor, canvas, cardName, baseFont, card.SuperType);
                    DrawCardText(rectangle, topCursor, bottomCursor, canvas, mastermind.Name, baseFont);
                    DrawSetText(rectangle, topCursor, bottomCursor, canvas, mastermind.Set, baseFont);
                }
            }).ToList();

            var drawVillainActions = allVillains.SelectMany(villain => new List<Action<PdfContentByte, Rectangle>>
            {
                (canvas, rectangle) =>
                {
                    var topCursor = new Cursor();
                    var bottomCursor = new Cursor();
                    const float startOfLabelOffset = 4f;
                    topCursor.AdvanceCursor(rectangle.Top - startOfLabelOffset);
                    bottomCursor.AdvanceCursor(rectangle.Bottom + startOfLabelOffset);
                    TextSharpHelpers.DrawRoundedRectangle(canvas, rectangle, new BaseColor(255,102,119));

                    //DrawBackgroundImage(card.SuperType, rectangle, canvas, topCursor, bottomCursor);
                    //DrawSetImageAndReturnTop(rectangle, bottomCursor, card.Set.Image, canvas);

                    //var cardName = card.GroupName ?? card.Name;
                    //DrawCardText(rectangle, topCursor, bottomCursor, canvas, cardName, baseFont, card.SuperType);
                    DrawCardText(rectangle, topCursor, bottomCursor, canvas, villain.Name, baseFont);
                    DrawSetText(rectangle, topCursor, bottomCursor, canvas, villain.Set, baseFont);
                }
            }).ToList();
            var drawHenchmenActions = allHenchmen.SelectMany(henchmen => new List<Action<PdfContentByte, Rectangle>>
            {
                (canvas, rectangle) =>
                {
                    var topCursor = new Cursor();
                    var bottomCursor = new Cursor();
                    const float startOfLabelOffset = 4f;
                    topCursor.AdvanceCursor(rectangle.Top - startOfLabelOffset);
                    bottomCursor.AdvanceCursor(rectangle.Bottom + startOfLabelOffset);
                    TextSharpHelpers.DrawRoundedRectangle(canvas, rectangle, new BaseColor(255,173,182));

                    //DrawBackgroundImage(card.SuperType, rectangle, canvas, topCursor, bottomCursor);
                    //DrawSetImageAndReturnTop(rectangle, bottomCursor, card.Set.Image, canvas);

                    //var cardName = card.GroupName ?? card.Name;
                    //DrawCardText(rectangle, topCursor, bottomCursor, canvas, cardName, baseFont, card.SuperType);
                    DrawCardText(rectangle, topCursor, bottomCursor, canvas, henchmen.Name, baseFont);
                    DrawSetText(rectangle, topCursor, bottomCursor, canvas, henchmen.Set, baseFont);
                }
            }).ToList();
            var drawStartingCardsActions = allStartingCards.SelectMany(startingCard => new List<Action<PdfContentByte, Rectangle>>
            {
                (canvas, rectangle) =>
                {
                    var topCursor = new Cursor();
                    var bottomCursor = new Cursor();
                    const float startOfLabelOffset = 4f;
                    topCursor.AdvanceCursor(rectangle.Top - startOfLabelOffset);
                    bottomCursor.AdvanceCursor(rectangle.Bottom + startOfLabelOffset);
                    TextSharpHelpers.DrawRoundedRectangle(canvas, rectangle, new BaseColor(200,200,200));

                    //DrawBackgroundImage(card.SuperType, rectangle, canvas, topCursor, bottomCursor);
                    //DrawSetImageAndReturnTop(rectangle, bottomCursor, card.Set.Image, canvas);

                    //var cardName = card.GroupName ?? card.Name;
                    //DrawCardText(rectangle, topCursor, bottomCursor, canvas, cardName, baseFont, card.SuperType);
                    DrawCardText(rectangle, topCursor, bottomCursor, canvas, startingCard.Name, baseFont);
                    DrawSetText(rectangle, topCursor, bottomCursor, canvas, startingCard.Set, baseFont);
                }
            }).ToList();
            var drawStartupCardsActions = allSetupCards.SelectMany(setupCard => new List<Action<PdfContentByte, Rectangle>>
            {
                (canvas, rectangle) =>
                {
                    var topCursor = new Cursor();
                    var bottomCursor = new Cursor();
                    const float startOfLabelOffset = 4f;
                    topCursor.AdvanceCursor(rectangle.Top - startOfLabelOffset);
                    bottomCursor.AdvanceCursor(rectangle.Bottom + startOfLabelOffset);
                    TextSharpHelpers.DrawRoundedRectangle(canvas, rectangle, new BaseColor(35,255,39));

                    //DrawBackgroundImage(card.SuperType, rectangle, canvas, topCursor, bottomCursor);
                    //DrawSetImageAndReturnTop(rectangle, bottomCursor, card.Set.Image, canvas);

                    //var cardName = card.GroupName ?? card.Name;
                    //DrawCardText(rectangle, topCursor, bottomCursor, canvas, cardName, baseFont, card.SuperType);
                    DrawCardText(rectangle, topCursor, bottomCursor, canvas, setupCard.Name, baseFont);
                    DrawSetText(rectangle, topCursor, bottomCursor, canvas, setupCard.Set, baseFont);
                }
            }).ToList();
            var allActions = drawHeroActions
                .Concat(drawMastermindActions)
                .Concat(drawVillainActions)
                .Concat(drawHenchmenActions)
                .Concat(drawStartingCardsActions)
                .Concat(drawStartupCardsActions);
            var drawActionRectangleQueue = new Queue<Action<PdfContentByte, Rectangle>>(allActions);
            PdfGenerator.DrawRectangles(drawActionRectangleQueue, BaseColor.WHITE, "Legendary");
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
            //var textWidthOffset = 8 + (maxFontSize - font.Size) * .35f;
            var textRectangle = new Rectangle(
                rectangle.Left + textWidthOffset,
                bottomCursor.GetCurrent() + textPadding,
                rectangle.Left + textWidthOffset + textRectangleHeight,
                topCursor.GetCurrent() - textPadding);
            //TextSharpHelpers.DrawRectangle(canvas, textRectangle, BaseColor.GREEN);
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
            const float textPadding = 0f;
            const float maxFontSize = 4f;
            var potentialTextRectangleHeight = topCursor.GetCurrent() - bottomCursor.GetCurrent(); // - textPadding * 2
            var textFontSize = TextSharpHelpers.GetFontSize(canvas, heroName, potentialTextRectangleHeight, baseFont, maxFontSize, Element.ALIGN_LEFT, Font.NORMAL);
            var font = new Font(baseFont, textFontSize, Font.NORMAL, BaseColor.BLACK);
            var textRectangleHeight = textFontSize * .9f;
            var textWidthOffset = 2f;
            //var textWidthOffset = 8 + (maxFontSize - font.Size) * .35f;
            var textRectangle = new Rectangle(
                rectangle.Left + textWidthOffset,
                bottomCursor.GetCurrent() + textPadding,
                rectangle.Left + textWidthOffset + textRectangleHeight,
                topCursor.GetCurrent() - textPadding);
            //TextSharpHelpers.DrawRectangle(canvas, textRectangle, BaseColor.GREEN);
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
