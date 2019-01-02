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
        BuriedSecrets,
        Legacy
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
                case Expansion.BuriedSecrets:
                    return "Buried Secrets";
                case Expansion.Legacy:
                    return "Legacy";
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
                case Expansion.BuriedSecrets:
                    return "BS";
                case Expansion.Legacy:
                    return "L";
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
