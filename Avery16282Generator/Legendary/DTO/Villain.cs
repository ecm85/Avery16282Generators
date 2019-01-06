using System.Collections.Generic;
using Avery16282Generator.Legendary.Enums;

namespace Avery16282Generator.Legendary.DTO
{
    public class Villain
    {
        public IList<VillainCard> Cards { get; } = new List<VillainCard>();
        public string Name { get; set; }
        public Expansion Expansion { get; set; }
    }
}
