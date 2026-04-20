using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Common;

namespace PortfolioApi.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult(this Result result) =>
            result.IsSuccess
                ? new NoContentResult()
                : Problem(result.Error);

        public static IActionResult ToActionResult<T>(this Result<T> result) =>
            result.IsSuccess
                ? new OkObjectResult(result.Value)
                : Problem(result.Error);

        public static IActionResult ToCreatedResult<T>(this Result<T> result, string location) =>
            result.IsSuccess
                ? new CreatedResult(location, result.Value)
                : Problem(result.Error);

        private static ObjectResult Problem(Error error)
        {
            var statusCode = error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = error.Code,
                Detail = error.Message,
                Type = $"https://httpstatuses.io/{statusCode}"
            };

            return new ObjectResult(problem) { StatusCode = statusCode };
        }
    }
}
