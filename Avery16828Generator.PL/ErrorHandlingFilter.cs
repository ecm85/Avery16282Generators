using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Avery16828Generator.PL
{
    public class ErrorHandlingFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            Console.WriteLine(exception.Message);
            //TODO: Log to event viewer
        }
    }
}