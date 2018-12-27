using System.Collections.Generic;

namespace Avery16282Generator.Legendary.DTO
{
    public class VillainSetupCardSet
    {
        public IList<VillainSetupCard> VillainSetupCards { get; } = new List<VillainSetupCard>();
        public string SetName { get; set; }
    }
}
