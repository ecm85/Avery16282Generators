using System.Collections.Generic;

namespace Avery16282Generator.Legendary.DTO
{
    public class Henchmen
    {
        public IList<string> CardText { get; } = new List<string>();
        public string Name { get; set; }
    }
}
