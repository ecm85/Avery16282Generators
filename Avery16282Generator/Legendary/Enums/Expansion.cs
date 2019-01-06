using System;
using Avery16282Generator.Legendary.Enums;

namespace Avery16282Generator.Legendary.Enums
{
    public enum Expansion
    {
        ThreeD,
        Captain__America__75th__Anniversary,
        Champions,
        Civil__War,
        Dark__City,
        Deadpool,
        Fantastic__Four,
        Fear__Itself,
        Guardians__of__the__Galaxy,
        Legendary,
        Noir,
        Paint__the__Town__Red,
        Secret__Wars__Volume__1,
        Secret__Wars__Volume__2,
        Spider_Man__Homecoming,
        Villains,
        X_Men
    }
}

public static class ExpansionExtensions
{
    public static string GetExpansionName(this Expansion expansion)
    {
        switch (expansion)
        {
            case Expansion.ThreeD:
                return "3D";
            case Expansion.Captain__America__75th__Anniversary:
                return "Captain America 75th Anniversary";
            case Expansion.Champions:
                return "Champions";
            case Expansion.Civil__War:
                return "Civil War";
            case Expansion.Dark__City:
                return "Dark City";
            case Expansion.Deadpool:
                return "Deadpool";
            case Expansion.Fantastic__Four:
                return "Fantastic Four";
            case Expansion.Fear__Itself:
                return "Fear Itself";
            case Expansion.Guardians__of__the__Galaxy:
                return "Guardians of the Galaxy";
            case Expansion.Legendary:
                return "Legendary";
            case Expansion.Noir:
                return "Noir";
            case Expansion.Paint__the__Town__Red:
                return "Paint the Town Red";
            case Expansion.Secret__Wars__Volume__1:
                return "Secret Wars Volume 1";
            case Expansion.Secret__Wars__Volume__2:
                return "Secret Wars Volume 2";
            case Expansion.Spider_Man__Homecoming:
                return "Spider-Man Homecoming";
            case Expansion.Villains:
                return "Villains";
            case Expansion.X_Men:
                return "X-Men";
            default:
                throw new InvalidOperationException();
        }
    }
}
