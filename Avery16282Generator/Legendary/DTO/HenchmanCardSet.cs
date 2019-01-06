using System.Collections.Generic;
using Avery16282Generator.Legendary.Enums;

namespace Avery16282Generator.Legendary.DTO
{
    public class HenchmenCardSet
    {
        public IList<Henchmen> Henchmen { get; } = new List<Henchmen>();
        public Expansion Expansion { get; set; }
    }
}
