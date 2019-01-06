using System.Collections.Generic;
using Avery16282Generator.Legendary.Enums;

namespace Avery16282Generator.Legendary.DTO
{
    public class MastermindCardSet
    {
        public IList<Mastermind> Masterminds { get; } = new List<Mastermind>();
        public Expansion Expansion { get; set; }
    }
}
