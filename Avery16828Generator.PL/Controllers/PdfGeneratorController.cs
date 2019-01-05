using System.IO;
using Avery16282Generator.Brewcrafters;
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
        public ActionResult GetFile(string fileName)
        {
            var file = Path.Combine(Directory, fileName);
            if (!System.IO.File.Exists(file))
            {
                return new NotFoundResult();
            }
            return File(new FileStream(file, FileMode.Open), "application/pdf", "Brewcrafters.pdf");
        }
    }
}
