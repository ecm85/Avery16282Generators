using System.Collections.Generic;

namespace Avery16282Generator.Legendary.DTO
{
    public class MastermindCardSet
    {
        public string SetName { get; set; }
        public IList<Mastermind> Masterminds { get; } = new List<Mastermind>();
    }
}
