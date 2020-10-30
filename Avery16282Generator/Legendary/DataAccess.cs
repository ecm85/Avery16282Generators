using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Avery16282Generator.Legendary.DTO;
using Avery16282Generator.Legendary.Enums;

namespace Avery16282Generator.Legendary
{
    public class DataAccess
    {
        public static string GetCurrentPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";

        private static IDictionary<string, Expansion> CardSetsByName { get; } = Enum.GetValues(typeof(Expansion))
            .Cast<Expansion>()
            .ToDictionary(cardSet => cardSet.GetExpansionName().ToLower());

        public static IEnumerable<HeroCardSet> GetHeroCardSets()
        {
            var allLines = File.ReadAllLines(GetCurrentPath + @"Legendary\Data\2_heroes_and_allies.txt_Sanitized.txt");
            var allSets = new List<HeroCardSet>();
            HeroCardSet currentSet = null;
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
                    currentSet = new HeroCardSet
                    {
                        Expansion = CardSetsByName[allLines[currentLineIndex].Replace("==", "").Trim().ToLower()]
                    };
                }
                else if (currentHero == null)
                {
                    var factionText = allLines[currentLineIndex + 1];
                    currentHero = new Hero
                    {
                        Factions = ParseHeroFactions(factionText).ToList(),
                        Name = allLines[currentLineIndex],
                        Expansion = currentSet.Expansion
                    };
                    currentLineIndex++;
                }
                else if (currentCard == null)
                {
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
                    else if (allLines[currentLineIndex].Equals("Transformed", StringComparison.OrdinalIgnoreCase))
                    {
                        currentCard = currentHero.Cards.Last();
                        currentHero.Cards.Remove(currentCard);
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
                        heroCardSection = new HeroCardSection
                        {

                            Name = allLines[currentLineIndex].Substring(0, allLines[currentLineIndex].IndexOf("(", StringComparison.Ordinal)),
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
                }
                currentLineIndex++;

            }

            return allSets;
        }

        public static IEnumerable<VillainCardSet> GetVillainCardSets()
        {
            var allLines = File.ReadAllLines(GetCurrentPath + @"Legendary\Data\5_villain_and_adversary_groups.txt_Sanitized.txt");
            var allSets = new List<VillainCardSet>();
            VillainCardSet currentSet = null;
            Villain currentVillain = null;
            VillainCard currentCard = null;
            var currentLineIndex = 0;
            while (currentLineIndex < allLines.Length)
            {
                if (string.IsNullOrWhiteSpace(allLines[currentLineIndex]))
                {
                    if (currentCard != null)
                    {
                        currentVillain.Cards.Add(currentCard);
                        currentCard = null;
                        if (string.IsNullOrWhiteSpace(allLines[currentLineIndex + 1]))
                        {
                            currentSet.Villains.Add(currentVillain);
                            currentVillain = null;
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
                    currentSet = new VillainCardSet
                    {
                        Expansion = CardSetsByName[allLines[currentLineIndex].Replace("==", "").Trim().ToLower()]
                    };
                }
                else if (currentVillain == null)
                {
                    currentVillain = new Villain
                    {
                        Name = allLines[currentLineIndex],
                        Expansion = currentSet.Expansion
                    };
                    currentLineIndex++;
                }
                else if (currentCard == null)
                {
                    currentCard = new VillainCard
                    {
                        NameAndCount = allLines[currentLineIndex]
                    };
                    currentLineIndex++;
                }
                else
                {
                    currentCard.CardText.Add(allLines[currentLineIndex]);
                }
                currentLineIndex++;

            }

            return allSets;
        }

        public static IEnumerable<HenchmenCardSet> GetHenchmenCardSets()
        {
            var allLines = File.ReadAllLines(GetCurrentPath + @"Legendary\Data\6_henchmen_and_backup.txt_Sanitized.txt");
            var allSets = new List<HenchmenCardSet>();
            HenchmenCardSet currentSet = null;
            Henchmen currentHenchmen = null;
            var currentLineIndex = 0;
            while (currentLineIndex < allLines.Length)
            {
                if (string.IsNullOrWhiteSpace(allLines[currentLineIndex]))
                {
                    if (currentHenchmen != null)
                    {
                        currentSet.Henchmen.Add(currentHenchmen);
                        currentHenchmen = null;
                        if (string.IsNullOrWhiteSpace(allLines[currentLineIndex + 1]))
                        {
                            allSets.Add(currentSet);
                            currentSet = null;
                            currentLineIndex++;
                        }
                    }
                }
                else if (currentSet == null)
                {
                    currentSet = new HenchmenCardSet
                    {
                        Expansion = CardSetsByName[allLines[currentLineIndex].Replace("==", "").Trim().ToLower()]
                    };
                }
                else if (currentHenchmen == null)
                {
                    currentHenchmen = new Henchmen
                    {
                        Name = allLines[currentLineIndex],
                        Expansion = currentSet.Expansion
                    };
                }
                else
                {
                    currentHenchmen.CardText.Add(allLines[currentLineIndex]);
                }
                currentLineIndex++;

            }

            return allSets;
        }

        public static IEnumerable<StartingCardSet> GetStartingCardSets()
        {
            var allLines = File.ReadAllLines(GetCurrentPath + @"Legendary\Data\StartingCards.txt");
            var allSets = new List<StartingCardSet>();
            StartingCardSet currentSet = null;
            StartingCard currentStartingCard = null;
            var currentLineIndex = 0;
            while (currentLineIndex < allLines.Length)
            {
                if (string.IsNullOrWhiteSpace(allLines[currentLineIndex]))
                {
                    if (currentStartingCard != null)
                    {
                        currentSet.StartingCards.Add(currentStartingCard);
                        currentStartingCard = null;
                        if (string.IsNullOrWhiteSpace(allLines[currentLineIndex + 1]))
                        {
                            allSets.Add(currentSet);
                            currentSet = null;
                            currentLineIndex++;
                        }
                    }
                }
                else if (currentSet == null)
                {
                    currentSet = new StartingCardSet
                    {
                        Expansion = CardSetsByName[allLines[currentLineIndex].Replace("==", "").Trim().ToLower()]
                    };
                }
                else if (currentStartingCard == null)
                {
                    currentStartingCard = new StartingCard
                    {
                        Name = allLines[currentLineIndex],
                        Expansion = currentSet.Expansion
                    };
                }
                else
                {
                    currentStartingCard.CardText.Add(allLines[currentLineIndex]);
                }
                currentLineIndex++;

            }

            return allSets;
        }

        public static IEnumerable<SetupCardSet> GetSetupCardSets()
        {
            var allLines = File.ReadAllLines(GetCurrentPath + @"Legendary\Data\SetupCards.txt");
            var allSets = new List<SetupCardSet>();
            SetupCardSet currentSet = null;
            SetupCard currentSetupCard = null;
            var currentLineIndex = 0;
            while (currentLineIndex < allLines.Length)
            {
                if (string.IsNullOrWhiteSpace(allLines[currentLineIndex]))
                {
                    if (currentSetupCard != null)
                    {
                        currentSet.SetupCards.Add(currentSetupCard);
                        currentSetupCard = null;
                        if (string.IsNullOrWhiteSpace(allLines[currentLineIndex + 1]))
                        {
                            allSets.Add(currentSet);
                            currentSet = null;
                            currentLineIndex++;
                        }
                    }
                }
                else if (currentSet == null)
                {
                    currentSet = new SetupCardSet
                    {
                        Expansion = CardSetsByName[allLines[currentLineIndex].Replace("==", "").Trim().ToLower()]
                    };
                }
                else if (currentSetupCard == null)
                {
                    if (allLines[currentLineIndex].StartsWith("S: "))
                    {
                        currentSetupCard = new SetupCard
                        {
                            Name = allLines[currentLineIndex].Substring(3, allLines[currentLineIndex].IndexOf("(", StringComparison.Ordinal) - 3),
                            Expansion = currentSet.Expansion,
                            IsSpecialCard = true
                        };
                    }
                    else
                    {
                        currentSetupCard = new SetupCard
                        {
                            Name = allLines[currentLineIndex].Substring(0, allLines[currentLineIndex].IndexOf("(", StringComparison.Ordinal)),
                            Expansion = currentSet.Expansion,
                            IsSpecialCard = false
                        };
                    }
                }
                else
                {
                    currentSetupCard.CardText.Add(allLines[currentLineIndex]);
                }
                currentLineIndex++;

            }

            return allSets;
        }

        public static IEnumerable<VillainSetupCardSet> GetVillainSetupCardSets()
        {
            var allLines = File.ReadAllLines(GetCurrentPath + @"Legendary\Data\VillainSetupCards.txt");
            var allSets = new List<VillainSetupCardSet>();
            VillainSetupCardSet currentSet = null;
            VillainSetupCard currentVillainSetupCard = null;
            var currentLineIndex = 0;
            while (currentLineIndex < allLines.Length)
            {
                if (string.IsNullOrWhiteSpace(allLines[currentLineIndex]))
                {
                    if (currentVillainSetupCard != null)
                    {
                        currentSet.VillainSetupCards.Add(currentVillainSetupCard);
                        currentVillainSetupCard = null;
                        if (string.IsNullOrWhiteSpace(allLines[currentLineIndex + 1]))
                        {
                            allSets.Add(currentSet);
                            currentSet = null;
                            currentLineIndex++;
                        }
                    }
                }
                else if (currentSet == null)
                {
                    currentSet = new VillainSetupCardSet
                    {
                        Expansion = CardSetsByName[allLines[currentLineIndex].Replace("==", "").Trim().ToLower()]
                    };
                }
                else if (currentVillainSetupCard == null)
                {
                    currentVillainSetupCard = new VillainSetupCard
                    {
                        Name = allLines[currentLineIndex],
                        Expansion = currentSet.Expansion
                    };
                }
                else
                {
                    currentVillainSetupCard.CardText.Add(allLines[currentLineIndex]);
                }
                currentLineIndex++;

            }

            return allSets;
        }

        public static IEnumerable<MastermindCardSet> GetMastermindCardSets()
        {
            var allLines = File.ReadAllLines(GetCurrentPath + @"Legendary\Data\4_masterminds_and_commanders.txt_Sanitized.txt");
            var allSets = new List<MastermindCardSet>();
            MastermindCardSet currentSet = null;
            Mastermind currentMastermind = null;
            MastermindTactic currentTactic = null;
            var currentLineIndex = 0;
            while (currentLineIndex < allLines.Length)
            {
                if (string.IsNullOrWhiteSpace(allLines[currentLineIndex]))
                {
                    if (currentTactic != null)
                    {
                        currentMastermind.Tactics.Add(currentTactic);
                        if (string.IsNullOrWhiteSpace(allLines[currentLineIndex + 1]))
                        {
                            currentTactic = null;
                            currentSet.Masterminds.Add(currentMastermind);
                            currentMastermind = null;
                            if (string.IsNullOrWhiteSpace(allLines[currentLineIndex + 2]))
                            {
                                allSets.Add(currentSet);
                                currentSet = null;
                                currentLineIndex++;
                            }
                            currentLineIndex++;
                        }
                        else
                        {
                            currentTactic = new MastermindTactic
                            {
                                Name = allLines[currentLineIndex + 1]
                            };
                            currentLineIndex++;
                        }
                    }
                    else if (currentMastermind != null)
                    {
                        currentTactic = new MastermindTactic
                        {
                            Name = allLines[currentLineIndex + 1]
                        };
                        currentLineIndex++;
                    }
                }
                else if (currentSet == null)
                {
                    currentSet = new MastermindCardSet
                    {
                        Expansion = CardSetsByName[allLines[currentLineIndex].Replace("==", "").Trim().ToLower()]
                    };
                }
                else if (currentMastermind == null)
                {
                    currentMastermind = new Mastermind
                    {
                        Name = allLines[currentLineIndex],
                        Expansion = currentSet.Expansion
                    };
                    currentLineIndex++;
                }
                else if (currentTactic == null)
                {
                    currentMastermind.TextLines.Add(allLines[currentLineIndex]);
                }
                else
                {
                    currentTactic.TextLines.Add(allLines[currentLineIndex]);
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
