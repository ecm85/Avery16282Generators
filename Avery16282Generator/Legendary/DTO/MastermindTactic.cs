using System.Collections.Generic;

namespace Avery16282Generator.Legendary.DTO
{
    public class MastermindTactic
    {
        public string Name { get; set; }
        public IList<string> TextLines { get; } = new List<string>();
    }
}
