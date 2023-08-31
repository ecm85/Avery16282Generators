using System.Collections.Generic;

namespace Avery16282Generator.PL.Requests;

public class GenerateArkhamHorrorLcgRequest : GenerateLabelsRequest
{
    public IEnumerable<string> SelectedCycles { get; set; }
}