using System.Collections.Generic;

namespace Avery16282Generator.Legendary.DTO
{
    public class SetupCardSet
    {
        public IList<SetupCard> SetupCards { get; } = new List<SetupCard>();
        public string SetName { get; set; }
    }
}
