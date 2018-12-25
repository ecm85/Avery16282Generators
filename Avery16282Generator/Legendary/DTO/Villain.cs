using System.Collections.Generic;

namespace Avery16282Generator.Legendary.DTO
{
    public class Villain
    {
        public IList<VillainCard> Cards { get; } = new List<VillainCard>();
        public string Name { get; set; }
    }
}
