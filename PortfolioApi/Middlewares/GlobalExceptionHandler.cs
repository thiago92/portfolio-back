using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioApi.Middlewares
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger,
            IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Exceção não tratada: {Message}", exception.Message);

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Ocorreu um erro inesperado.",
                Detail = BuildDetail(exception),
                Type = "https://httpstatuses.io/500",
                Instance = httpContext.Request.Path
            };

            if (_environment.IsDevelopment())
            {
                problem.Extensions["exceptionType"] = exception.GetType().FullName;
                problem.Extensions["stackTrace"] = exception.StackTrace;
                if (exception.InnerException is not null)
                {
                    problem.Extensions["innerExceptionType"] = exception.InnerException.GetType().FullName;
                    problem.Extensions["innerMessage"] = exception.InnerException.Message;
                }
            }

            httpContext.Response.StatusCode = problem.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

            return true;
        }

        private static string BuildDetail(Exception exception)
        {
            var messages = new List<string> { exception.Message };
            var inner = exception.InnerException;
            while (inner is not null)
            {
                messages.Add(inner.Message);
                inner = inner.InnerException;
            }
            return string.Join(" -> ", messages);
        }
    }
}
