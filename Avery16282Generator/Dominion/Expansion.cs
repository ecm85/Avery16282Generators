using System;

namespace Avery16282Generator.Dominion
{
    public enum Expansion
    {
        Adventures,
        Alchemy,
        Animals,
        Base,
        Cornucopia,
        Dark_Ages,
        Dominion_1st_Edition,
        Dominion_2nd_Edition,
        Dominion_2nd_Edition_Upgrade,
        Empires,
        Guilds,
        Hinterlands,
        Intrigue_1st_Edition,
        Intrigue_2nd_Edition,
        Intrigue_2nd_Edition_Upgrade,
        Nocturne,
        Promo,
        Prosperity,
        Seaside,
        Renaissance
    }

    public static class ExpansionExtensions
    {
        public static string GetExpansionName(this Expansion expansion)
        {
            switch (expansion)
            {
                case Expansion.Adventures:
                    return "Adventures";
                case Expansion.Alchemy:
                    return "Alchemy";
                case Expansion.Animals:
                    return "Animals";
                case Expansion.Base:
                    return "Base";
                case Expansion.Cornucopia:
                    return "Cornucopia";
                case Expansion.Dark_Ages:
                    return "Dark Ages";
                case Expansion.Dominion_1st_Edition:
                    return "Dominion 1st Edition";
                case Expansion.Dominion_2nd_Edition:
                    return "Dominion 2nd Edition";
                case Expansion.Dominion_2nd_Edition_Upgrade:
                    return "Dominion 2nd Edition Upgrade";
                case Expansion.Empires:
                    return "Empires";
                case Expansion.Guilds:
                    return "Guilds";
                case Expansion.Hinterlands:
                    return "Hinterlands";
                case Expansion.Intrigue_1st_Edition:
                    return "Intrigue 1st Edition";
                case Expansion.Intrigue_2nd_Edition:
                    return "Intrigue 2nd Edition";
                case Expansion.Intrigue_2nd_Edition_Upgrade:
                    return "Intrigue 2nd Edition Upgrade";
                case Expansion.Nocturne:
                    return "Nocturne";
                case Expansion.Promo:
                    return "Promo";
                case Expansion.Prosperity:
                    return "Prosperity";
                case Expansion.Seaside:
                    return "Seaside";
                case Expansion.Renaissance:
                    return "Renaissance";
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
