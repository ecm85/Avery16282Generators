using System.Collections.Generic;

namespace Avery16282Generator.PL.Requests
{
    public class GenerateDominionRequest
    {
        public IEnumerable<string> SelectedExpansionNames { get; set; }
    }
}
