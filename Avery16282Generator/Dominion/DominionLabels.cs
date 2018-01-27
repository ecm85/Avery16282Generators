using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;

namespace Avery16282Generator.Dominion
{
    public class DominionLabels
    {
        public static void CreateLabels()
        {
            var cardTypes = GetCardTypes().ToList();
            var cardSets = GetCardSets();
            var cards = GetCards(cardSets, cardTypes);
            var setNamesToPrint = new[] { "Intrigue 2nd Edition Upgrade", "Dominion 2nd Edition Upgrade", "Nocturne" };
            var setsToPrint = cardSets.Values.Where(cardSet => setNamesToPrint.Contains(cardSet.Set_name));
            var cardFromSetsToPrint = cards
                .Where(card => card.Sets.Any(cardSet => setsToPrint.Contains(cardSet)))
                .ToList();
            var groupedCards = cardFromSetsToPrint.Where(card => !string.IsNullOrWhiteSpace(card.Group_tag)).ToList();
            var nonGroupedCards = cardFromSetsToPrint.Except(groupedCards);
            var groupedCardsToPrint = groupedCards.GroupBy(card => card.Group_tag)
                .Select(group => group.SingleOrDefault(card => card.Group_top) ?? group.First())
                .ToList();
            var cardsToPrint = nonGroupedCards.Concat(groupedCardsToPrint).ToList();

            //TODO: background image (based on type)
            //TODO: Cost (coin/potion/debt)
            //TODO: Set image
            //TODO: Scale text
            //TODO: Font

            var segoeUi = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "seguisym.ttf");
            var baseFont = BaseFont.CreateFont(segoeUi, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var fontSize = 10;
            var font = new Font(baseFont, fontSize, Font.NORMAL, BaseColor.BLACK);

            var drawActionRectangles = cardsToPrint.SelectMany(card => new List<Action<PdfContentByte, Rectangle>>
            {
                (contentByte, rectangle) =>
                {

                    contentByte.SetColorFill(BaseColor.CYAN);
                    contentByte.SetColorStroke(BaseColor.CYAN);
                    TextSharpHelpers.DrawRectangle(contentByte, rectangle);
                    //if (beer.Points > 0)
                    //{
                    //    var pointsImage = Image.GetInstance("Brewcrafters\\Points.png");
                    //    pointsImage.ScalePercent(7f);
                    //    pointsImage.Rotation = 4.71239f;
                    //    pointsImage.SetAbsolutePosition(rectangle.Left + 12, rectangle.Bottom + 3);
                    //    contentByte.AddImage(pointsImage);
                    //}

                    //var smallFontPadding = string.IsNullOrWhiteSpace(beer.Name3) ? 0 : 6;
                    ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(card.GroupName ?? card.Name, font), rectangle.Left + fontSize * 2 + 2, rectangle.Top - 3, 270);
                    //ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(beer.Name2, font), smallFontPadding + rectangle.Left + fontSize + 1, rectangle.Top - 3, 270);
                    //ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(beer.Name3, font), smallFontPadding + rectangle.Left + 0, rectangle.Top - 3, 270);
                    //if (beer.Points > 0)
                    //    ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(beer.Points.ToString(), font), rectangle.Left + 15, rectangle.Bottom + 12, 270);
                    //if (!string.IsNullOrEmpty(beer.ImageName))
                    //{
                    //    var beerImage = Image.GetInstance("Brewcrafters\\" + beer.ImageName);
                    //    beerImage.SetAbsolutePosition(rectangle.Left + 10, rectangle.Bottom + 16);
                    //    beerImage.Rotation = 4.71239f;
                    //    //beerImage.Rotation = 1.5708f;
                    //    beerImage.ScalePercent(9f);
                    //    contentByte.AddImage(beerImage);
                    //}
                    //if (beer.Barrel)
                    //{
                    //    var barrelImage = Image.GetInstance("Brewcrafters\\Barrel.png");
                    //    barrelImage.Rotation = 4.71239f;
                    //    barrelImage.ScalePercent(20f);
                    //    barrelImage.SetAbsolutePosition(rectangle.Left + 12, rectangle.Bottom + 34);
                    //    contentByte.AddImage(barrelImage);
                    //}
                    //if (beer.Hops)
                    //{
                    //    var hopsImage = Image.GetInstance("Brewcrafters\\Hops.png");
                    //    hopsImage.Rotation = 4.71239f;
                    //    hopsImage.ScalePercent(15f);
                    //    hopsImage.SetAbsolutePosition(rectangle.Left + 12, rectangle.Bottom + 34);
                    //    contentByte.AddImage(hopsImage);
                    //}
                },
                (contentByte, rectangle) =>
                {
                    //var fontSize = string.IsNullOrWhiteSpace(beer.Name2) ? 9 : string.IsNullOrWhiteSpace(beer.Name3) ? 10 : 8;
                    //var font = new Font(baseFont, fontSize, Font.NORMAL, BaseColor.BLACK);
                    contentByte.SetColorFill(BaseColor.CYAN);
                    contentByte.SetColorStroke(BaseColor.CYAN);
                    TextSharpHelpers.DrawRectangle(contentByte, rectangle);
                    //var smallFontPadding = string.IsNullOrWhiteSpace(beer.Name3) ? 0 : 6;
                    //if (beer.Points > 0)
                    //{
                    //    var pointsImage = Image.GetInstance("Brewcrafters\\Points.png");
                    //    pointsImage.ScalePercent(7f);
                    //    pointsImage.Rotation = 1.5708f;
                    //    pointsImage.SetAbsolutePosition(rectangle.Right - (12 + pointsImage.ScaledWidth),
                    //        rectangle.Top - (3 + pointsImage.ScaledHeight));
                    //    contentByte.AddImage(pointsImage);
                    //}
                    ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(card.GroupName ?? card.Name, font), rectangle.Right - (fontSize * 2 + 2), rectangle.Bottom + 3, 90);

                    //ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(beer.Name2, font), rectangle.Right - (smallFontPadding + fontSize + 1), rectangle.Bottom + 3, 90);
                    //ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(beer.Name3, font), rectangle.Right  - smallFontPadding, rectangle.Bottom + 3, 90);
                    //if (beer.Points > 0)
                    //    ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(beer.Points.ToString(), font), rectangle.Right - 15, rectangle.Top - 12, 90);
                    //if (!string.IsNullOrEmpty(beer.ImageName))
                    //{
                    //    var beerImage = Image.GetInstance("Brewcrafters\\" + beer.ImageName);
                    //    beerImage.ScalePercent(9f);
                    //    beerImage.SetAbsolutePosition(rectangle.Right - (beerImage.ScaledWidth + 10), rectangle.Top - (beerImage.ScaledHeight + 16));
                    //    beerImage.Rotation = 1.5708f;
                    //    contentByte.AddImage(beerImage);
                    //}

                    //if (beer.Barrel)
                    //{
                    //    var barrelImage = Image.GetInstance("Brewcrafters\\Barrel.png");
                    //    barrelImage.Rotation = 1.5708f;
                    //    barrelImage.ScalePercent(20f);
                    //    barrelImage.SetAbsolutePosition(rectangle.Right - (barrelImage.ScaledWidth + 12), rectangle.Top - (barrelImage.ScaledHeight + 34));
                    //    contentByte.AddImage(barrelImage);
                    //}
                    //if (beer.Hops)
                    //{
                    //    var hopsImage = Image.GetInstance("Brewcrafters\\Hops.png");
                    //    hopsImage.Rotation = 1.5708f;
                    //    hopsImage.ScalePercent(15f);
                    //    hopsImage.SetAbsolutePosition(rectangle.Right - (hopsImage.ScaledWidth + 12), rectangle.Top - (hopsImage.ScaledHeight + 34));
                    //    contentByte.AddImage(hopsImage);
                    //}

                },
            }).ToList();
            var drawActionRectangleQueue = new Queue<Action<PdfContentByte, Rectangle>>(drawActionRectangles);
            PdfGenerator.DrawRectangles(drawActionRectangleQueue, BaseColor.WHITE, "Dominion");
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

            foreach (var card in cards)
            {
                card.Name = englishCards[card.Card_tag].Name;
                card.Sets = card.Cardset_tags.Select(setTag => cardSets[setTag]);
                card.SuperType = cardSuperTypes.Single(cardSuperType => cardSuperType.Card_type.OrderBy(i => i).SequenceEqual(card.Types.OrderBy(i => i)));
                if (!string.IsNullOrWhiteSpace(card.Group_tag))
                    card.GroupName = englishCards[card.Group_tag].Name;
            }
            return cards;
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
