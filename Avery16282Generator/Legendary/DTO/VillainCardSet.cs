using System.Collections.Generic;

namespace Avery16282Generator.Legendary.DTO
{
    public class VillainCardSet
    {
        public IList<Villain> Villains { get; } = new List<Villain>();
        public string SetName { get; set; }
    }
}
