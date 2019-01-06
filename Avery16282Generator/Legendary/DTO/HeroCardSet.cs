using System.Collections.Generic;
using Avery16282Generator.Legendary.Enums;

namespace Avery16282Generator.Legendary.DTO
{
    public class HeroCardSet
    {
        public IList<Hero> Heroes { get; } = new List<Hero>();
        public Expansion Expansion { get; set; }
    }
}
