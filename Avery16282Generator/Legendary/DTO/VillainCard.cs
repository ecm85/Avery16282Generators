using System.Collections.Generic;

namespace Avery16282Generator.Legendary.DTO
{
    public class VillainCard
    {
        public string NameAndCount { get; set; }
        public IList<string> CardText { get; } = new List<string>();
    }
}
