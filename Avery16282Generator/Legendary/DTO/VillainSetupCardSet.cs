using System.Collections.Generic;
using Avery16282Generator.Legendary.Enums;

namespace Avery16282Generator.Legendary.DTO
{
    public class VillainSetupCardSet
    {
        public IList<VillainSetupCard> VillainSetupCards { get; } = new List<VillainSetupCard>();
        public Expansion Expansion { get; set; }
    }
}
