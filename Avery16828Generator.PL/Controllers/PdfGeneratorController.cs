using System.Threading;
using Microsoft.AspNetCore.Mvc;

namespace Avery16828Generator.PL.Controllers
{
    [Route("api/[controller]")]
    public class PdfGeneratorController : Controller
    {
        private static int counter = 0;
        [HttpGet("[action]")]
        public string GenerateBrewcrafters()
        {
            Thread.Sleep(3000);
            return "MyDummyFilename" + counter++;
        }
    }
}
