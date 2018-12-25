using System.Collections.Generic;

namespace Avery16282Generator.Legendary.DTO
{
    public class HenchmenCardSet
    {
        public IList<Henchmen> Henchmen { get; } = new List<Henchmen>();
        public string SetName { get; set; }
    }
}
