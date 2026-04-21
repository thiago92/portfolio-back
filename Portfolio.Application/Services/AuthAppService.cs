using FluentValidation;
using Portfolio.Application.DTOs.Auth;
using Portfolio.Application.Exceptions;
using Portfolio.Application.Interface;
using Portfolio.Domain.Interface;

namespace Portfolio.Application.Services
{
    public class AuthAppService : IAuthAppService
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEnumerable<IValidator<LoginDto>> _validators;

        public AuthAppService(
            IUserService userService,
            IJwtTokenGenerator tokenGenerator,
            IPasswordHasher passwordHasher,
            IEnumerable<IValidator<LoginDto>> validators)
        {
            _userService = userService;
            _tokenGenerator = tokenGenerator;
            _passwordHasher = passwordHasher;
            _validators = validators;
        }

        public async Task<TokenDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);
            await ValidarAsync(dto, cancellationToken);

            var user = await _userService.GetByEmailAsync(dto.Email, cancellationToken)
                ?? throw new BadCredentialsException();

            if (!_passwordHasher.Verify(dto.Senha, user.PasswordHash))
                throw new BadCredentialsException();

            var token = _tokenGenerator.Generate(user);

            return new TokenDto
            {
                AccessToken = token.Token,
                ExpiresAt = token.ExpiresAt
            };
        }

        private async Task ValidarAsync(LoginDto dto, CancellationToken cancellationToken)
        {
            if (!_validators.Any()) return;

            var failures = (await Task.WhenAll(_validators.Select(v => v.ValidateAsync(dto, cancellationToken))))
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .Select(f => f.ErrorMessage)
                .ToList();

            if (failures.Count > 0)
                throw new EntityValidationException(failures);
        }
    }
}
