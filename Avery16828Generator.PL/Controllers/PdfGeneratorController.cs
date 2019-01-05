using System.IO;
using Avery16282Generator.Brewcrafters;
using Microsoft.AspNetCore.Mvc;

namespace Avery16828Generator.PL.Controllers
{
    [Route("api/[controller]")]
    public class PdfGeneratorController : Controller
    {
        [HttpGet("[action]")]
        public string GenerateBrewcrafters()
        {
            return Path.GetFileName(BrewcraftersLabels.CreateLabels());
        }

        [HttpGet("[action]")]
        public ActionResult GetFile(string fileName)
        {
            var file = "c:\\Avery\\" + fileName;
            if (System.IO.File.Exists(file))
            {
                return File(new FileStream(file, FileMode.Open), "application/pdf", "Brewcrafters.pdf");
            }
            else
            {
                return new NotFoundResult();
            }
        }
    }
}
