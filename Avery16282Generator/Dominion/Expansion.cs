using System;

namespace Avery16282Generator.Dominion
{
    public enum Expansion
    {
        Adventures,
        Adventures_Extras,
        Alchemy,
        Animals,
        Base,
        Cornucopia,
        Cornucopia_Extras,
        Dark_Ages,
        Dark_Ages_Extras,
        Dominion_1st_Edition,
        Dominion_2nd_Edition,
        Dominion_2nd_Edition_Upgrade,
        Empires,
        Empires_Extras,
        Extras,
        Guilds,
        Hinterlands,
        Intrigue_1st_Edition,
        Intrigue_2nd_Edition,
        Intrigue_2nd_Edition_Upgrade,
        Nocturne,
        Nocturne_Extras,
        Promo,
        Prosperity,
        Seaside
    }

    public static class ExpansionExtensions
    {
        public static string GetExpansionName(this Expansion expansion)
        {
            switch (expansion)
            {
                case Expansion.Adventures:
                    return "Adventures";
                case Expansion.Adventures_Extras:
                    return "Adventures Extras";
                case Expansion.Alchemy:
                    return "Alchemy";
                case Expansion.Animals:
                    return "Animals";
                case Expansion.Base:
                    return "Base";
                case Expansion.Cornucopia:
                    return "Cornucopia";
                case Expansion.Cornucopia_Extras:
                    return "Cornucopia Extras";
                case Expansion.Dark_Ages:
                    return "Dark Ages";
                case Expansion.Dark_Ages_Extras:
                    return "Dark Ages Extras";
                case Expansion.Dominion_1st_Edition:
                    return "Dominion 1st Edition";
                case Expansion.Dominion_2nd_Edition:
                    return "Dominion 2nd Edition";
                case Expansion.Dominion_2nd_Edition_Upgrade:
                    return "Dominion 2nd Edition Upgrade";
                case Expansion.Empires:
                    return "Empires";
                case Expansion.Empires_Extras:
                    return "Empires Extras";
                case Expansion.Extras:
                    return "Extras";
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
                case Expansion.Nocturne_Extras:
                    return "Nocturne Extras";
                case Expansion.Promo:
                    return "Promo";
                case Expansion.Prosperity:
                    return "Prosperity";
                case Expansion.Seaside:
                    return "Seaside";
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
