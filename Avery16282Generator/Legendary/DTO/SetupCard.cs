using System.Collections.Generic;
using Avery16282Generator.Legendary.Enums;

namespace Avery16282Generator.Legendary.DTO
{
    public class SetupCard
    {
        public IList<string> CardText { get; } = new List<string>();
        public string Name { get; set; }
        public Expansion Expansion { get; set; }
        public bool IsSpecialCard { get; set; }
    }
}
