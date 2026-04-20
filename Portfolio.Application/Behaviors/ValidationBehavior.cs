using FluentValidation;
using MediatR;
using Portfolio.Application.Common;

namespace Portfolio.Application.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);
            var failures = (await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken))))
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if (failures.Count == 0)
                return await next();

            var message = string.Join(" | ", failures.Select(f => f.ErrorMessage));
            var error = Error.Validation("Validation.Failed", message);

            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var valueType = typeof(TResponse).GetGenericArguments()[0];
                var failureMethod = typeof(Result)
                    .GetMethods()
                    .First(m => m.Name == nameof(Result.Failure) && m.IsGenericMethod)
                    .MakeGenericMethod(valueType);

                return (TResponse)failureMethod.Invoke(null, new object[] { error })!;
            }

            if (typeof(TResponse) == typeof(Result))
                return (TResponse)(object)Result.Failure(error);

            throw new ValidationException(failures);
        }
    }
}
