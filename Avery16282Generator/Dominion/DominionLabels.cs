using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;

namespace Avery16282Generator.Dominion
{
    public static class DominionLabels
    {
        public static void CreateLabels()
        {
            var cardTypes = GetCardTypes().ToList();
            var cardSets = GetCardSets();
            var cards = GetCards(cardSets, cardTypes);

            var setsToNotPrint = new[] { "Dominion 2nd Edition", "Intrigue 2nd Edition", "Animals" };
            var bannedKeywords = new[] { "Extras" };
            
            var whiteList = new string[] { "Dominion 2nd Edition Upgrade", "Intrigue 2nd Edition Upgrade", "Nocturne" };

            var useWhiteList = false;

            var setsExcludingBlacklist = cardSets.Values
                .Where(cardSet => !setsToNotPrint.Contains(cardSet.Set_name))
                .Where(cardSet => bannedKeywords.All(bannedKeyword => !cardSet.Set_name.Contains(bannedKeyword)))
                .Select(setToNotPrint => setToNotPrint.Set_name);

            var cardFromSetsToPrint = cards
                .Where(card => (useWhiteList ? whiteList : setsExcludingBlacklist ).Contains(card.Set.Set_name))
                .ToList();

            var groupedCards = cardFromSetsToPrint.Where(card => !string.IsNullOrWhiteSpace(card.Group_tag)).ToList();
            var nonGroupedCards = cardFromSetsToPrint.Except(groupedCards);
            var groupedCardsToPrint = groupedCards.GroupBy(card => card.Group_tag)
                .Select(group => group.SingleOrDefault(card => card.Group_top) ?? group.First())
                .ToList();
            var cardsToPrint = nonGroupedCards.Concat(groupedCardsToPrint).ToList();
            var minionProRegular = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "TrajanPro-Regular.otf");
            var minionProBold = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "TrajanPro-Bold.otf");
            var baseFont = BaseFont.CreateFont(minionProRegular, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var boldBaseFont = BaseFont.CreateFont(minionProBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            const float coinCostRectangleHeight = 13f;
            const float potionCostRectangleHeight = 6f;
            const float debtCostRectangleHeight = 13f;
            const float costPadding = 1f;
            const float textPadding = 2f;
            const float firstCostImageHeightOffset = 1.5f;
            const float coinCostImageWidthOffset = 5f;
            const float potionCostImageWidthOffset = 6f;
            const float debtCostImageWidthOffset = 5f;
            const float setImageHeight = 7f;
            const float setImageWidthOffset = 7f;
            const float setImageHeightOffset = 4.5f;
            const float textWidthOffset = 9f;
            const float textHeight = 8f;
            const float maxFontSize = 10f;

            const float costFontSize = 7.5f;
            var costFont = new Font(boldBaseFont, costFontSize, Font.BOLD, BaseColor.BLACK);
            const float costTextWidthOffset = 3.5f;
            const float costTextHeightOffset = 3.5f;

            const float debtCostFontSize = 7.5f;
            var debtCostFont = new Font(boldBaseFont, debtCostFontSize, Font.BOLD, BaseColor.BLACK);
            const float debtCostTextWidthOffset = 3.5f;
            const float debtCostTextHeightOffset = 4f;

            var drawActionRectangles = cardsToPrint.SelectMany(card => new List<Action<PdfContentByte, Rectangle>>
            {
                (contentByte, rectangle) =>
                {
                    const float rotationInRadians = 4.71239f;
                    const int textRotation = 270;

                    var backgroundImage = CreateBackgroundImage(card.SuperType.Card_type_image, rotationInRadians, rectangle);
                    var backgroundImageBottom = rectangle.Bottom + (rectangle.Height - backgroundImage.ScaledHeight) / 2;
                    backgroundImage.SetAbsolutePosition(rectangle.Left, backgroundImageBottom);
                    contentByte.AddImage(backgroundImage);

                    var currentCostBottom = backgroundImageBottom + backgroundImage.ScaledHeight - firstCostImageHeightOffset;
                    Rectangle currentCostRectangle;
                    if (!string.IsNullOrWhiteSpace(card.Cost) && (card.Cost != "0" || (card.Cost == "0" && card.Potcost != 1 && !card.Debtcost.HasValue)))
                    {
                        currentCostBottom = currentCostBottom - (coinCostRectangleHeight + costPadding);
                        currentCostRectangle = new Rectangle(rectangle.Left + coinCostImageWidthOffset, currentCostBottom, rectangle.Right, currentCostBottom + coinCostRectangleHeight);

                        var coinImage = CreateCoinImage(rotationInRadians, currentCostRectangle);
                        coinImage.SetAbsolutePosition(currentCostRectangle.Left, currentCostRectangle.Bottom);
                        contentByte.AddImage(coinImage);

                        var cost = card.Cost.Replace("*", "");
                        ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(cost, costFont), currentCostRectangle.Left + costTextWidthOffset, currentCostRectangle.Top - costTextHeightOffset, textRotation);

                    }
                    if (card.Potcost == 1)
                    {
                        currentCostBottom = currentCostBottom - (potionCostRectangleHeight + costPadding);
                        currentCostRectangle = new Rectangle(rectangle.Left + potionCostImageWidthOffset, currentCostBottom, rectangle.Right, currentCostBottom + potionCostRectangleHeight);

                        var potionImage = CreatePotionImage(rotationInRadians, currentCostRectangle);
                        potionImage.SetAbsolutePosition(currentCostRectangle.Left, currentCostRectangle.Bottom);
                        contentByte.AddImage(potionImage);
                    }
                    if (card.Debtcost.HasValue)
                    {
                        currentCostBottom = currentCostBottom - (debtCostRectangleHeight + costPadding);
                        currentCostRectangle = new Rectangle(rectangle.Left + debtCostImageWidthOffset, currentCostBottom, rectangle.Right, currentCostBottom + debtCostRectangleHeight);

                        var debtImage = CreateDebtImage(rotationInRadians, currentCostRectangle);
                        debtImage.SetAbsolutePosition(currentCostRectangle.Left, currentCostRectangle.Bottom);
                        contentByte.AddImage(debtImage);

                        var debtCost = card.Debtcost.ToString();
                        ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(debtCost, debtCostFont), currentCostRectangle.Left + debtCostTextWidthOffset, currentCostRectangle.Top - debtCostTextHeightOffset, textRotation);
                    }


                    var setImageRectangle = new Rectangle(rectangle.Left + setImageWidthOffset, backgroundImageBottom + setImageHeightOffset, rectangle.Right, backgroundImageBottom + setImageHeightOffset + setImageHeight);
                    if (!string.IsNullOrWhiteSpace(card.Set.Image))
                    {
                        var setImage = CreateSetImage(card.Set.Image, setImageRectangle, rotationInRadians);
                        setImage.SetAbsolutePosition(setImageRectangle.Left, setImageRectangle.Bottom);
                        contentByte.AddImage(setImage);
                    }

                    var cardName = card.GroupName ?? card.Name;
                    var textRectangle = new Rectangle(rectangle.Left + textWidthOffset, setImageRectangle.Top + textPadding, rectangle.Left + textWidthOffset + textHeight, currentCostBottom - textPadding);
                    var font = GetMainTextFont(textRectangle, contentByte, cardName, baseFont, maxFontSize, card);
                    ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(cardName, font), textRectangle.Left, textRectangle.Top, textRotation);

                    //TODO: Experiment with using the rotation on rectangle, in the rectangle generator
                },
                (contentByte, rectangle) =>
                {
                    var rotationInRadians = 1.5708f;
                    const int textRotation = 90;
                    var backgroundImage = CreateBackgroundImage(card.SuperType.Card_type_image, rotationInRadians, rectangle);
                    var backgroundImageBottom = rectangle.Bottom + (rectangle.Height - backgroundImage.ScaledHeight) / 2;
                    backgroundImage.SetAbsolutePosition(rectangle.Left, backgroundImageBottom);
                    contentByte.AddImage(backgroundImage);

                    var currentCostTop = backgroundImageBottom + firstCostImageHeightOffset;
                    Rectangle currentCostRectangle;
                    if (!string.IsNullOrWhiteSpace(card.Cost) && (card.Cost != "0" || (card.Cost == "0" && card.Potcost != 1 && !card.Debtcost.HasValue)))
                    {
                        currentCostTop = currentCostTop + (coinCostRectangleHeight + costPadding);
                        currentCostRectangle = new Rectangle(rectangle.Left, currentCostTop - coinCostRectangleHeight, rectangle.Right - coinCostImageWidthOffset, currentCostTop);
                        var coinImage = CreateCoinImage(rotationInRadians, currentCostRectangle);
                        coinImage.SetAbsolutePosition(currentCostRectangle.Right - coinImage.ScaledWidth, currentCostRectangle.Top - coinImage.ScaledHeight);
                        contentByte.AddImage(coinImage);

                        var cost = card.Cost.Replace("*", "");
                        const float bestGuessAtCostTextWidth = 5.5f;
                        ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(cost, costFont), (currentCostRectangle.Right) - costTextWidthOffset, (currentCostRectangle.Top) - costTextHeightOffset - bestGuessAtCostTextWidth, textRotation);
                    }
                    if (card.Potcost == 1)
                    {
                        currentCostTop = currentCostTop + (potionCostRectangleHeight + costPadding);
                        currentCostRectangle = new Rectangle(rectangle.Left, currentCostTop - potionCostRectangleHeight, rectangle.Right - potionCostImageWidthOffset, currentCostTop);

                        var potionImage = CreatePotionImage(rotationInRadians, currentCostRectangle);
                        potionImage.SetAbsolutePosition(currentCostRectangle.Right - potionImage.ScaledWidth, currentCostRectangle.Top - potionImage.ScaledHeight);
                        contentByte.AddImage(potionImage);
                    }
                    if (card.Debtcost.HasValue)
                    {
                        currentCostTop = currentCostTop + (debtCostRectangleHeight + costPadding);
                        currentCostRectangle = new Rectangle(rectangle.Left, currentCostTop - debtCostRectangleHeight, rectangle.Right - debtCostImageWidthOffset, currentCostTop);

                        var debtImage = CreateDebtImage(rotationInRadians, currentCostRectangle);
                        debtImage.SetAbsolutePosition(currentCostRectangle.Right - debtImage.ScaledWidth, currentCostRectangle.Top - debtImage.ScaledHeight);
                        contentByte.AddImage(debtImage);

                        var debtCost = card.Debtcost.ToString();
                        const float bestGuessAtCostTextWidth = 5.5f;
                        ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(debtCost, debtCostFont), (currentCostRectangle.Right) - debtCostTextWidthOffset, (currentCostRectangle.Top) - debtCostTextHeightOffset - bestGuessAtCostTextWidth, textRotation);
                    }

                    var setImageRectangle = new Rectangle(rectangle.Left, backgroundImageBottom + backgroundImage.ScaledHeight - setImageHeightOffset - setImageHeight, rectangle.Right - setImageWidthOffset, backgroundImageBottom + backgroundImage.ScaledHeight - setImageHeightOffset);
                    if (!string.IsNullOrWhiteSpace(card.Set.Image))
                    {
                        var setImage = CreateSetImage(card.Set.Image, setImageRectangle, rotationInRadians);
                        setImage.SetAbsolutePosition(setImageRectangle.Right - setImage.ScaledWidth, setImageRectangle.Top - setImage.ScaledHeight);
                        contentByte.AddImage(setImage);
                    }
                    var cardName = card.GroupName ?? card.Name;
                    var textRectangle = new Rectangle(rectangle.Right - textWidthOffset - textHeight, currentCostTop + textPadding, rectangle.Right - textWidthOffset, setImageRectangle.Bottom - textPadding);
                    var font = GetMainTextFont(textRectangle, contentByte, cardName, baseFont, maxFontSize, card);
                    ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(cardName, font), textRectangle.Right, textRectangle.Bottom, textRotation);

                },
            }).ToList();
            var drawActionRectangleQueue = new Queue<Action<PdfContentByte, Rectangle>>(drawActionRectangles);
            PdfGenerator.DrawRectangles(drawActionRectangleQueue, BaseColor.WHITE, "Dominion");
        }

        private static Font GetMainTextFont(Rectangle textRectangle, PdfContentByte contentByte, string cardName,
            BaseFont baseFont, float maxFontSize, DominionCard card)
        {
            var rotatedRectangle = new Rectangle(textRectangle.Left, textRectangle.Bottom,
                textRectangle.Left + textRectangle.Height, textRectangle.Bottom + textRectangle.Width);
            var textFontSize = TextSharpHelpers.GetFontSize(contentByte, cardName, rotatedRectangle, baseFont, maxFontSize,
                Element.ALIGN_LEFT, Font.NORMAL);
            var fontColor = card.SuperType.Card_type_image == "night_nc.png"
                ? BaseColor.WHITE
                : BaseColor.BLACK;
            var font = new Font(baseFont, textFontSize, Font.NORMAL, fontColor);
            return font;
        }

        private static Image CreateSetImage(string imageName, Rectangle setImageRectangle, float rotationInRadians)
        {
            var setImage = Image.GetInstance($@"Dominion\{imageName}");
            setImage.ScaleToFit(setImageRectangle);
            setImage.Rotation = rotationInRadians;
            return setImage;
        }

        private static Image CreateDebtImage(float rotationInRadians, Rectangle currentCostRectangle)
        {
            var debtImage = Image.GetInstance(@"Dominion\debt.png");
            debtImage.Rotation = rotationInRadians;
            debtImage.ScaleToFit(currentCostRectangle);
            return debtImage;
        }

        private static Image CreatePotionImage(float rotationInRadians, Rectangle currentCostRectangle)
        {
            var potionImage = Image.GetInstance(@"Dominion\potion.png");
            potionImage.Rotation = rotationInRadians;
            potionImage.ScaleToFit(currentCostRectangle);
            return potionImage;
        }

        private static Image CreateCoinImage(float rotationInRadians, Rectangle currentCostRectangle)
        {
            var coinImage = Image.GetInstance(@"Dominion\coin_small.png");
            coinImage.Rotation = rotationInRadians;
            coinImage.ScaleToFit(currentCostRectangle);
            return coinImage;
        }

        private static Image CreateBackgroundImage(string baseImageName, float rotationInRadians, Rectangle rectangle)
        {
            var imageNameTokens = baseImageName.Split('.');
            var backgroundImage = Image.GetInstance($@"Dominion\{imageNameTokens[0]}_nc.{imageNameTokens[1]}");
            backgroundImage.Rotation = rotationInRadians;
            backgroundImage.ScaleToFit(rectangle);
            return backgroundImage;
        }

        private static IEnumerable<DominionCard> GetCards(IDictionary<string, CardSet> cardSets, IList<CardSuperType> cardSuperTypes)
        {
            IEnumerable<DominionCard> cards;
            using (var fileStream = new FileStream("Dominion\\cards_db.json", FileMode.Open))
            using (var reader = new StreamReader(fileStream))
            using (var jsonTextReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                cards = serializer.Deserialize<IEnumerable<DominionCard>>(jsonTextReader).ToList();
            }

            IDictionary<string, DominionCard> englishCards;
            using (var fileStream = new FileStream("Dominion\\cards_en_us.json", FileMode.Open))
            using (var reader = new StreamReader(fileStream))
            using (var jsonTextReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                englishCards = serializer.Deserialize<IDictionary<string, DominionCard>>(jsonTextReader);
            }

            return cardSets.Keys
                .SelectMany(key => 
                    cards
                        .Where(card => card.Cardset_tags.Contains(key))
                        .Select(card => new DominionCard
                        {
                            Group_tag = card.Group_tag,
                            Types = card.Types,
                            Name = englishCards[card.Card_tag].Name,
                            Card_tag = card.Card_tag,
                            Cardset_tags = card.Cardset_tags,
                            GroupName = string.IsNullOrWhiteSpace(card.Group_tag) ? null : englishCards[card.Group_tag].Name,
                            Group_top = card.Group_top,
                            Cost = card.Cost,
                            Debtcost = card.Debtcost,
                            Potcost = card.Potcost,
                            Set = cardSets[key],
                            SuperType = cardSuperTypes.Single(cardSuperType => cardSuperType.Card_type.OrderBy(i => i).SequenceEqual(card.Types.OrderBy(i => i)))
                        })
                        .ToList());
        }

        private static IDictionary<string, CardSet> GetCardSets()
        {
            IDictionary<string, CardSet> cardSets;
            using (var fileStream = new FileStream("Dominion\\sets_db.json", FileMode.Open))
            using (var reader = new StreamReader(fileStream))
            using (var jsonTextReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                cardSets = serializer.Deserialize<Dictionary<string, CardSet>>(jsonTextReader);
            }

            IDictionary<string, CardSet> englishCardSets;
            using (var fileStream = new FileStream("Dominion\\sets_en_us.json", FileMode.Open))
            using (var reader = new StreamReader(fileStream))
            using (var jsonTextReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                englishCardSets = serializer.Deserialize<Dictionary<string, CardSet>>(jsonTextReader);
            }

            foreach (var cardSetName in cardSets.Keys)
            {
                cardSets[cardSetName].Set_name = englishCardSets[cardSetName].Set_name;
                cardSets[cardSetName].Set_text = englishCardSets[cardSetName].Set_text;
                cardSets[cardSetName].Text_icon = englishCardSets[cardSetName].Text_icon;
            }
            return cardSets;
        }

        private static IEnumerable<CardSuperType> GetCardTypes()
        {
            using (var fileStream = new FileStream("Dominion\\types_db.json", FileMode.Open))
            using (var reader = new StreamReader(fileStream))
            using (var jsonTextReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<IEnumerable<CardSuperType>>(jsonTextReader);
            }
        }
    }
}
