using System.Collections.Generic;

namespace Avery16282Generator.Legendary.DTO
{
    public class HeroCardSet
    {
        public IList<Hero> Heroes { get; } = new List<Hero>();
        public string SetName { get; set; }
    }
}
