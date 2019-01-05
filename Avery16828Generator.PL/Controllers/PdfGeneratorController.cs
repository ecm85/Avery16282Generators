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
            return BrewcraftersLabels.CreateLabels();
        }
    }
}
