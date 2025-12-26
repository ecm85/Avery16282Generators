using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Reflection;
using Newtonsoft.Json;

namespace Avery16282Generator.Dominion
{
    public static class DominionCardDataAccess
    {
        public static string CurrentPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static IEnumerable<DominionCard> GetCardsToPrint(IEnumerable<Expansion> expansionsToPrint = null)
        {
            var expansionNamesToPrint = expansionsToPrint?.Select(expansion => expansion.GetExpansionName()).ToList();
            if (expansionNamesToPrint != null) { Console.WriteLine($"Printing cards from sets {string.Join(",", expansionNamesToPrint)}"); }
            var cardTypes = GetCardTypes().ToList();
            var cardSets = GetCardSets();
            var cards = GetCards(cardSets, cardTypes);

            var setsToPrint = cardSets.Values
                .Select(set => set.Set_name)
                .Where(setToPrint => expansionNamesToPrint == null || expansionNamesToPrint.Contains(setToPrint))
                .ToList();

            var cardFromSetsToPrint = cards
                .Where(card => setsToPrint.Contains(card.Set.Set_name))
                .ToList();

            var groupedCards = cardFromSetsToPrint.Where(card => !string.IsNullOrWhiteSpace(card.Group_tag)).ToList();
            var nonGroupedCards = cardFromSetsToPrint.Except(groupedCards);
            // Console.WriteLine(JsonConvert.SerializeObject(groupedCards));
            var groupedCardsToPrint = groupedCards.GroupBy(card => card.Group_tag)
                .Select(cardGroup => {
                    if (cardGroup.Count(card => card.Group_top) > 1)
                    {
                        Console.WriteLine($"Key: {cardGroup.Key}");
                        Console.WriteLine($"CardGroup: {JsonConvert.SerializeObject(cardGroup)}");
                        // Console.WriteLine($"cardGroup: {string.Join(",", cardGroup.Select(card => JsonConvert.SerializeObject(card)))}");
                    }
                    return cardGroup.SingleOrDefault(card => card.Group_top) ?? cardGroup.First();
                })
                .ToList();
            var cardsToPrint = nonGroupedCards.Concat(groupedCardsToPrint).ToList();
            return cardsToPrint
                .Where(card => !string.IsNullOrWhiteSpace(card.GroupName ?? card.Name))
                .OrderBy(card => card.GroupName ?? card.Name)
                .ToList();
        }

        private static IEnumerable<DominionCard> GetCards(IDictionary<string, CardSet> cardSets, IList<CardSuperType> cardSuperTypes)
        {
            IEnumerable<DominionCard> cards;
            using (var fileStream = new FileStream(Path.Combine(CurrentPath, "Dominion", "cards_db.json"), FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fileStream))
            using (var jsonTextReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                cards = serializer.Deserialize<IEnumerable<DominionCard>>(jsonTextReader).ToList();
            }

            IDictionary<string, DominionCard> englishCards;
            using (var fileStream = new FileStream(Path.Combine(CurrentPath, "Dominion", "cards_en_us.json"), FileMode.Open, FileAccess.Read))
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
                        .Select(card => CreateDominionCard(cardSets[key], cardSuperTypes, card, englishCards))
                        .ToList())
                .ToList();
        }

        private static DominionCard CreateDominionCard(
            CardSet cardSet,
            IList<CardSuperType> cardSuperTypes,
            DominionCard baseCard,
            IDictionary<string, DominionCard> englishCards)
        {
            var superType = cardSuperTypes.First(cardSuperType => cardSuperType.Card_type.OrderBy(i => i).SequenceEqual(baseCard.Types.OrderBy(i => i)));
            var hasGroupTag = !string.IsNullOrWhiteSpace(baseCard.Group_tag);
            var cost = !hasGroupTag || baseCard.Group_top ?
                baseCard.Cost :
                "";
            var englishCard = englishCards.ContainsKey(baseCard.Card_tag) ? englishCards[baseCard.Card_tag] : null;
            var englishGroupCard = hasGroupTag && englishCards.ContainsKey(baseCard.Group_tag) ? englishCards[baseCard.Group_tag] : null;
            return new DominionCard
            {
                Group_tag = baseCard.Group_tag,
                Types = baseCard.Types,
                Name = FormatName(englishCard?.Name ?? ""),
                Card_tag = baseCard.Card_tag,
                Cardset_tags = baseCard.Cardset_tags,
                GroupName = englishGroupCard == null ?
                    null :
                    FormatName(englishGroupCard.Name),
                Group_top = baseCard.Group_top,
                Cost = cost,
                Debtcost = baseCard.Debtcost,
                Potcost = baseCard.Potcost,
                Set = cardSet,
                SuperType = superType
            };
        }

        private static string FormatName(string value)
        {
            const string delimiter = " - ";
            var indexOfDelimiter = value.IndexOf(delimiter);
            return indexOfDelimiter < 0 ? value : value.Substring(0, indexOfDelimiter);
        }

        private static IDictionary<string, CardSet> GetCardSets()
        {
            IDictionary<string, CardSet> cardSets;
            using (var fileStream = new FileStream(Path.Combine(CurrentPath, "Dominion", "sets_db.json"), FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fileStream))
            using (var jsonTextReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                cardSets = serializer.Deserialize<Dictionary<string, CardSet>>(jsonTextReader);
            }

            IDictionary<string, CardSet> englishCardSets;
            using (var fileStream = new FileStream(Path.Combine(CurrentPath, "Dominion", "sets_en_us.json"), FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fileStream))
            using (var jsonTextReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                englishCardSets = serializer.Deserialize<Dictionary<string, CardSet>>(jsonTextReader);
            }

            foreach (var cardSetName in cardSets.Keys)
            {
                cardSets[cardSetName].Set_name = englishCardSets.ContainsKey(cardSetName) ? englishCardSets[cardSetName].Set_name : "";
                cardSets[cardSetName].Set_text = englishCardSets.ContainsKey(cardSetName) ? englishCardSets[cardSetName].Set_text : "";
                cardSets[cardSetName].Text_icon = englishCardSets.ContainsKey(cardSetName) ? englishCardSets[cardSetName].Text_icon : "";
            }
            return cardSets;
        }

        private static IEnumerable<CardSuperType> GetCardTypes()
        {
            using (var fileStream = new FileStream(Path.Combine(CurrentPath, "Dominion", "types_db.json"), FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fileStream))
            using (var jsonTextReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<IEnumerable<CardSuperType>>(jsonTextReader);
            }
        }
    }
}
