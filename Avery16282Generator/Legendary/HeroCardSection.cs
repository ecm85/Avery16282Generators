using System.Collections.Generic;

namespace Avery16282Generator.Legendary
{
    public class HeroCardSection
    {
        public string Name { get; set; }
        public HeroCardType HeroCardType { get; set; }
        public IList<string> CardTextAndCost { get; set; } = new List<string>();
    }
}
