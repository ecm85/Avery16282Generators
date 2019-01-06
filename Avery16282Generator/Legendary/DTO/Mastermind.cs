using System.Collections.Generic;
using Avery16282Generator.Legendary.Enums;

namespace Avery16282Generator.Legendary.DTO
{
    public class Mastermind
    {
        public string Name { get; set; }
        public IList<string> TextLines { get; } = new List<string>();
        public IList<MastermindTactic> Tactics { get; } = new List<MastermindTactic>();
        public Expansion Expansion { get; set; }
    }
}
