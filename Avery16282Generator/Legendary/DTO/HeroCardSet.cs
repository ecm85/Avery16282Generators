using System.Collections.Generic;

namespace Avery16282Generator.Legendary.DTO
{
    public class HeroCardSet
    {
        public IList<Hero> Heroes { get; set; } = new List<Hero>();
        public string SetName { get; set; }
    }
}
