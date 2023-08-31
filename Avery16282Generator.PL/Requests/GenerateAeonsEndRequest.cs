using System.Collections.Generic;

namespace Avery16282Generator.PL.Requests
{
    public class GenerateAeonsEndRequest : GenerateLabelsRequest
    {
        public IEnumerable<string> SelectedExpansionNames { get; set; }
    }
}
