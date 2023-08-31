using System;

namespace Avery16282Generator.Dominion;

public class DominionCardIdentifier
{
    public string Text { get; init; }
    public string CardSetName { get; init; }

    protected bool Equals(DominionCardIdentifier other)
    {
        return Text == other.Text && CardSetName == other.CardSetName;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((DominionCardIdentifier) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Text, CardSetName);
    }
}
