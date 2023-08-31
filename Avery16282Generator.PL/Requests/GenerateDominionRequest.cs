using System.Collections.Generic;
using Avery16282Generator.Dominion;

namespace Avery16282Generator.PL.Requests
{
    public class GenerateDominionRequest : GenerateLabelsRequest
    {
        public IEnumerable<DominionCardIdentifier> SelectedCardIdentifiers { get; set; }
    }
}
