using System.Collections.Generic;
using Avery16282Generator.Legendary.Enums;

namespace Avery16282Generator.Legendary.DTO
{
    public class VillainCardSet
    {
        public IList<Villain> Villains { get; } = new List<Villain>();
        public Expansion Expansion { get; set; }
    }
}
