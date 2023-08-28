using System.Collections.Generic;

namespace Avery16282Generator.PL.Requests;

public class GenerateArkhamHorrorLcgRequest
{
    public IEnumerable<string> SelectedCycles { get; set; }
}