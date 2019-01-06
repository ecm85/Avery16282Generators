using System.Collections.Generic;
using Avery16282Generator.Legendary.Enums;

namespace Avery16282Generator.Legendary.DTO
{
    public class StartingCardSet
    {
        public IList<StartingCard> StartingCards { get; } = new List<StartingCard>();
        public Expansion Expansion { get; set; }
    }
}
