using System.Collections.Generic;

namespace Avery16282Generator.Legendary.DTO
{
    public class Hero
    {
        public IList<HeroCard> Cards { get; set; } = new List<HeroCard>();
        public IList<HeroFaction> Factions { get; set; }
        public string Name { get; set; }
    }
}
