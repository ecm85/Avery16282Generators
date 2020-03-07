using System;

namespace Avery16282Generator.AeonsEnd
{
    public enum Expansion
    {
        AeonsEnd,
        WarEternal,
        TheNameless,
        TheOuterDark,
        TheVoid,
        TheDepths,
        Legacy,
        BuriedSecrets,
        TheNewAge,
        ShatteredDreams,
        TheAncients,
        IntoTheWild
    }

    public static class ExpansionExtensions
    {
        public static string GetFriendlyName(this Expansion expansion)
        {
            switch (expansion)
            {
                case Expansion.AeonsEnd:
                    return "Aeon's End";
                case Expansion.WarEternal:
                    return "War Eternal";
                case Expansion.TheNameless:
                    return "The Nameless";
                case Expansion.TheOuterDark:
                    return "The Outer Dark";
                case Expansion.TheVoid:
                    return "The Void";
                case Expansion.TheDepths:
                    return "The Depths";
                case Expansion.Legacy:
                    return "Legacy";
                case Expansion.BuriedSecrets:
                    return "Buried Secrets";
                case Expansion.TheNewAge:
                    return "The New Age";
                case Expansion.ShatteredDreams:
                    return "Shattered Dreams";
                case Expansion.TheAncients:
                    return "The Ancients";
                case Expansion.IntoTheWild:
                    return "Into The Wild";
                default:
                    throw new InvalidOperationException();
            }
        }

        public static string GetAbbreviation(this Expansion expansion)
        {
            switch (expansion)
            {
                case Expansion.AeonsEnd:
                    return "AE";
                case Expansion.WarEternal:
                    return "WE";
                case Expansion.TheNameless:
                    return "N";
                case Expansion.TheOuterDark:
                    return "OD";
                case Expansion.TheVoid:
                    return "V";
                case Expansion.TheDepths:
                    return "D";
                case Expansion.Legacy:
                    return "L";
                case Expansion.BuriedSecrets:
                    return "BS";
                case Expansion.TheNewAge:
                    return "NA";
                case Expansion.ShatteredDreams:
                    return "SD";
                case Expansion.TheAncients:
                    return "A";
                case Expansion.IntoTheWild:
                    return "IW";
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
