using System.Collections.Generic;

namespace Avery16282Generator.Legendary
{
    public class HeroCardSection
    {
        public string Name { get; set; }
        public IList<HeroCardType> HeroCardTypes { get; set; }
        public IList<string> CardTextAndCost { get; set; } = new List<string>();
    }
}
