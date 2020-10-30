using System;
using System.Collections.Generic;
using System.Linq;
using Avery16282Generator.AeonsEnd;
using Avery16282Generator.Brewcrafters;
using Avery16282Generator.Dominion;
using Avery16282Generator.Legendary;
using Avery16828Generator.PL.Requests;
using Microsoft.AspNetCore.Mvc;
using Expansion = Avery16282Generator.AeonsEnd.Expansion;

namespace Avery16828Generator.PL.Controllers
{
    [Route("api/[controller]")]
    public class PdfGeneratorController : Controller
    {
        [HttpPost("[action]")]
        public FileResult GenerateBrewcrafters()
        {
            var bytes = BrewcraftersLabels.CreateLabels();
            return File(bytes, "application/document", "BrewcraftersLabels.pdf");
        }

        [HttpPost("[action]")]
        public ActionResult<string> GenerateLegendary([FromBody]GenerateLegendaryRequest request)
        {
            var expansionsByName = Enum.GetValues(typeof(Avery16282Generator.Legendary.Enums.Expansion))
                .Cast<Avery16282Generator.Legendary.Enums.Expansion>()
                .ToDictionary(expansion => expansion.GetExpansionName());
            var selectedExpansions = request.SelectedExpansionNames
                .Select(expansionName => expansionsByName[expansionName])
                .ToList();
            var bytes = LegendaryLabels.CreateLabels(selectedExpansions, request.IncludeSpecialSetupCards);

            return S3Service.UploadPdfToS3(bytes, "LegendaryLabels");
        }

        [HttpGet("[action]")]
        public IEnumerable<string> GetLegendaryExpansions()
        {
            return Enum.GetValues(typeof(Avery16282Generator.Legendary.Enums.Expansion))
                .Cast<Avery16282Generator.Legendary.Enums.Expansion>()
                .Select(expansion => expansion.GetExpansionName())
                .ToList();
        }

        [HttpPost("[action]")]
        public FileResult GenerateDominion(IEnumerable<string> expansionNames)
        {
            var expansionsByName = Enum.GetValues(typeof(Avery16282Generator.Dominion.Expansion))
                .Cast<Avery16282Generator.Dominion.Expansion>()
                .ToDictionary(expansion => expansion.GetExpansionName());
            var includedSets = expansionNames
                .Select(expansionName => expansionsByName[expansionName])
                .ToList();
            var bytes = DominionLabels.CreateLabels(includedSets);
            return File(bytes, "application/document", "DominionLabels.pdf");
        }

        [HttpGet("[action]")]
        public IEnumerable<string> GetDominionExpansions()
        {
            return Enum.GetValues(typeof(Avery16282Generator.Dominion.Expansion))
                .Cast<Avery16282Generator.Dominion.Expansion>()
                .Select(expansion => expansion.GetExpansionName())
                .ToList();
        }

        [HttpPost("[action]")]
        public FileResult GenerateAeonsEnd(IEnumerable<string> expansionNames)
        {
            var expansionsByName = Enum.GetValues(typeof(Expansion))
                .Cast<Expansion>()
                .ToDictionary(expansion => expansion.GetFriendlyName());
            var includedSets = expansionNames
                .Select(expansionName => expansionsByName[expansionName])
                .ToList();
            var bytes = AeonsEndLabels.CreateLabels(includedSets);
            return File(bytes, "application/document", "AeonsEndLabels.pdf");
        }

        [HttpGet("[action]")]
        public IEnumerable<string> GetAeonsEndExpansions()
        {
            return Enum.GetValues(typeof(Expansion))
                .Cast<Expansion>()
                .Select(expansion => expansion.GetFriendlyName())
                .ToList();
        }
    }
}
