using System;
using System.IO;
using System.Linq;
using Avery16282Generator.AeonsEnd;
using Avery16282Generator.Brewcrafters;
using Avery16282Generator.Legendary;
using Microsoft.AspNetCore.Mvc;

namespace Avery16828Generator.PL.Controllers
{
    [Route("api/[controller]")]
    public class PdfGeneratorController : Controller
    {
        private const string Directory = "c:\\Avery\\";

        [HttpGet("[action]")]
        public string GenerateBrewcrafters()
        {
            return BrewcraftersLabels.CreateLabels(Directory);
        }

        [HttpGet("[action]")]
        public string GenerateLegendary()
        {
            var includedSets = Enum.GetValues(typeof(Avery16282Generator.Legendary.Enums.Expansion))
                .Cast<Avery16282Generator.Legendary.Enums.Expansion>()
                .ToList();
            return LegendaryLabels.CreateLabels(Directory, includedSets, true);
        }

        [HttpGet("[action]")]
        public string GenerateAeonsEnd()
        {
            return AeonsEndLabels.CreateLabels(Directory, Enum.GetValues(typeof(Expansion)).Cast<Expansion>().ToList());
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
