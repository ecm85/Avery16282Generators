using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Avery16282Generator.Legendary
{
    public class DataAccess
    {
        //TODO: Civil-war double cards
        public static IEnumerable<CardSet> GetCardSets()
        {
            var allLines = File.ReadAllLines(@"Legendary\Data\HeroesAndAllies");
            var allSets = new List<CardSet>();
            CardSet currentSet = null;
            Hero currentHero = null;
            HeroCard currentCard = null;
            HeroCardSection heroCardSection = null;
            var currentLineIndex = 0;
            while(currentLineIndex < allLines.Length)
            {
                if (string.IsNullOrWhiteSpace(allLines[currentLineIndex]))
                {
                    if (currentCard != null)
                    {
                        currentHero.Cards.Add(currentCard);
                        currentCard = null;
                        if (string.IsNullOrWhiteSpace(allLines[currentLineIndex + 1]))
                        {
                            currentSet.Heroes.Add(currentHero);
                            currentHero = null;
                            if (string.IsNullOrWhiteSpace(allLines[currentLineIndex + 2]))
                            {
                                allSets.Add(currentSet);
                                currentSet = null;
                                currentLineIndex++;
                            }
                            currentLineIndex++;

                        }
                    }
                }
                else if (currentSet == null)
                {
                    currentSet = new CardSet
                    {
                        SetName = allLines[currentLineIndex].Replace("==", "")
                    };
                }
                else if (currentHero == null)
                {
                    var factionText = allLines[currentLineIndex + 1];
                    currentHero = new Hero
                    {
                        Factions = ParseHeroFactions(factionText).ToList(),
                        Name = allLines[currentLineIndex]
                    };
                    currentLineIndex++;
                }
                else if (currentCard == null)
                {
                    //TODO: Parse out name, count and card type
                    if (allLines[currentLineIndex + 1].Equals("Divided", StringComparison.OrdinalIgnoreCase))
                    {
                        heroCardSection = new HeroCardSection
                        {

                            Name = allLines[currentLineIndex + 2],
                            HeroCardTypes = ParseHeroCardTypes(allLines[currentLineIndex + 3]).ToList()
                        };
                        currentCard = new HeroCard
                        {
                            HeroCardSection1 = heroCardSection,
                            NameAndCount = allLines[currentLineIndex]
                        };
                        currentLineIndex += 3;
                    }
                    else
                    {
                        heroCardSection = new HeroCardSection
                        {

                            Name = allLines[currentLineIndex].Substring(0, allLines[currentLineIndex].IndexOf("(")),
                            HeroCardTypes = ParseHeroCardTypes(allLines[currentLineIndex + 1]).ToList()
                        };
                        currentCard = new HeroCard
                        {
                            HeroCardSection1 = heroCardSection,
                            NameAndCount = allLines[currentLineIndex]
                        };
                        currentLineIndex++;
                    }
                }
                else
                {
                    if (allLines[currentLineIndex].StartsWith("---"))
                    {
                        heroCardSection = new HeroCardSection
                        {
                            Name = allLines[currentLineIndex + 1],
                            HeroCardTypes = ParseHeroCardTypes(allLines[currentLineIndex + 2]).ToList()
                        };
                        currentCard.HeroCardSection2 = heroCardSection;
                        currentLineIndex += 2;
                    }
                    else
                    {
                        heroCardSection.CardTextAndCost.Add(allLines[currentLineIndex]);
                    }
                    //TODO: Check for cost
                }
                currentLineIndex++;

            }

            return allSets;
        }

        private static IEnumerable<HeroCardType> ParseHeroCardTypes(string heroCardTypeText)
        {
            var heroCardTypes = heroCardTypeText.Split(',');
            foreach (var cardType in heroCardTypes)
            {
                yield return ParseHeroCardType(cardType);
            }
        }

        private static HeroCardType ParseHeroCardType(string heroCardTypeText)
        {
            var successfullyHeroCardType = Enum.TryParse(heroCardTypeText, true, out HeroCardType heroCardType);

            if (!successfullyHeroCardType)
                throw new InvalidOperationException($"Could not parse {heroCardTypeText} into a hero card type");
            return heroCardType;
        }

        private static IEnumerable<HeroFaction> ParseHeroFactions(string factionText)
        {
            var factions = factionText.Split('/');
            foreach (var faction in factions)
            {
                yield return ParseHeroFaction(faction);
            }
        }

        private static HeroFaction ParseHeroFaction(string factionText)
        {
            var factionTextWithoutSpaces = factionText.Replace(" ", "");
            var successfullyParsedFaction = Enum.TryParse(factionTextWithoutSpaces, true, out HeroFaction faction);
            if (!successfullyParsedFaction)
            {
                if (factionTextWithoutSpaces.Equals("X-Men", StringComparison.OrdinalIgnoreCase))
                    return HeroFaction.XMen;
                if (factionTextWithoutSpaces.Equals("(Unaffiliated)", StringComparison.OrdinalIgnoreCase))
                    return HeroFaction.Unaffiliated;
                if (factionTextWithoutSpaces.Equals("S.H.I.E.L.D.", StringComparison.OrdinalIgnoreCase))
                    return HeroFaction.Shield;
                if (factionTextWithoutSpaces.Equals("X-Force", StringComparison.OrdinalIgnoreCase))
                    return HeroFaction.XForce;

                throw new InvalidOperationException($"Could not parse {factionText} into a faction");
            }
            return faction;
        }
    }
}
