using System.Collections.Generic;

namespace Avery16282Generator.Legendary.DTO
{
    public class Mastermind
    {
        public string Name { get; set; }
        public IList<string> TextLines { get; } = new List<string>();
        public IList<MastermindTactic> Tactics { get; } = new List<MastermindTactic>();
        public string Set { get; set; }
    }
}
