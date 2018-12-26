using System.Collections.Generic;

namespace Avery16282Generator.Legendary.DTO
{
    public class StartingCardSet
    {
        public IList<StartingCard> StartingCards { get; } = new List<StartingCard>();
        public string SetName { get; set; }
    }
}
