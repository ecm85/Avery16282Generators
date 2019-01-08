using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avery16282Generator.AeonsEnd;
using Avery16282Generator.Brewcrafters;
using Avery16282Generator.Dominion;
using Avery16282Generator.Legendary;
using Microsoft.AspNetCore.Mvc;
using Expansion = Avery16282Generator.AeonsEnd.Expansion;

namespace Avery16828Generator.PL.Controllers
{
    [Route("api/[controller]")]
    public class PdfGeneratorController : Controller
    {
        private const string Directory = "c:\\Avery\\";

        [HttpPost("[action]")]
        public FileResult GenerateBrewcrafters()
        {
            var fileName = BrewcraftersLabels.CreateLabels(Directory);
            var filePath = Path.Combine(Directory, fileName);
            return File(new FileStream(filePath, FileMode.Open), "application/document", fileName);
        }

        [HttpGet("[action]")]
        public string GenerateLegendary()
        {
            var includedSets = Enum.GetValues(typeof(Avery16282Generator.Legendary.Enums.Expansion))
                .Cast<Avery16282Generator.Legendary.Enums.Expansion>()
                .ToList();
            return LegendaryLabels.CreateLabels(Directory, includedSets, true);
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
            var fileName = DominionLabels.CreateLabels(Directory, includedSets);
            var filePath = Path.Combine(Directory, fileName);
            return File(new FileStream(filePath, FileMode.Open), "application/document", fileName);
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
            var fileName = AeonsEndLabels.CreateLabels(Directory, includedSets);
            var filePath = Path.Combine(Directory, fileName);
            return File(new FileStream(filePath, FileMode.Open), "application/document", fileName);
        }

        [HttpGet("[action]")]
        public IEnumerable<string> GetAeonsEndExpansions()
        {
            return Enum.GetValues(typeof(Expansion))
                .Cast<Expansion>()
                .Select(expansion => expansion.GetFriendlyName())
                .ToList();
        }

        [HttpGet("[action]")]
        public ActionResult GetFile(string fileName)
        {
            var file = Path.Combine(Directory, fileName);
            if (!System.IO.File.Exists(file))
            {
                return new NotFoundResult();
            }
            return File(new FileStream(file, FileMode.Open), "application/pdf", fileName);
        }
    }
}
