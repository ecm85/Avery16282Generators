using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Avery16828Generator.PL
{
    public class ErrorHandlingFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<Startup> _logger;
        public ErrorHandlingFilter(ILogger<Startup> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            _logger.Log(LogLevel.Error, exception, exception.Message);
        }
    }
}