using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Avery16828Generator.PL.Controllers
{
    public class ErrorTestController : Controller
    {
        [HttpGet("[action]")]
        public string ThrowException()
        {
            throw new InvalidOperationException("Test invalid op from error controller.");
        }

        [HttpGet("[action]")]
        public ActionResult Return500()
        {
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }
}
