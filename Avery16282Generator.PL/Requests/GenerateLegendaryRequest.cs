﻿using System.Collections.Generic;

namespace Avery16282Generator.PL.Requests
{
    public class GenerateLegendaryRequest : GenerateLabelsRequest
    {
        public IEnumerable<string> SelectedExpansionNames { get; set; }
        public bool IncludeSpecialSetupCards { get; set; }
    }
}
