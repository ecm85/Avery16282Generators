using System.Collections.Generic;
using Avery16282Generator.Legendary.Enums;

namespace Avery16282Generator.Legendary.DTO
{
    public class SetupCardSet
    {
        public IList<SetupCard> SetupCards { get; } = new List<SetupCard>();
        public Expansion Expansion { get; set; }
    }
}
