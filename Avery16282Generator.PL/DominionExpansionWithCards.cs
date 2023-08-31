using System.Collections.Generic;
using Avery16282Generator.Dominion;

namespace Avery16282Generator.PL;

public class DominionExpansionWithCards
{
    public string Name { get; init; }
    public IEnumerable<DominionCardIdentifier> Cards { get; init; }
}