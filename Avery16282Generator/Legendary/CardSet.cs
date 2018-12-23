using System.Collections.Generic;

namespace Avery16282Generator.Legendary
{
    public class CardSet
    {
        public IList<Hero> Heroes { get; set; } = new List<Hero>();
        public string SetName { get; set; }
    }
}
