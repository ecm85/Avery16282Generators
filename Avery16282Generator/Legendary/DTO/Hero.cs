using System.Collections.Generic;
using Avery16282Generator.Legendary.Enums;

namespace Avery16282Generator.Legendary.DTO
{
    public class Hero
    {
        public IList<HeroCard> Cards { get; } = new List<HeroCard>();
        public IList<HeroFaction> Factions { get; set; }
        public string Name { get; set; }
    }
}
