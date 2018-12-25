using System.Collections.Generic;
using Avery16282Generator.Legendary.Enums;

namespace Avery16282Generator.Legendary.DTO
{
    public class HeroCardSection
    {
        public string Name { get; set; }
        public IList<HeroCardType> HeroCardTypes { get; set; }
        public IList<string> CardTextAndCost { get; } = new List<string>();
    }
}
