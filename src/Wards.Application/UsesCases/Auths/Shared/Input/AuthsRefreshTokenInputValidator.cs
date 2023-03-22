using FluentValidation;

namespace Wards.Application.UsesCases.Auths.Shared.Input
{
    public class AuthsRefreshTokenInputValidator : AbstractValidator<AuthsRefreshTokenInput>
    {
        public AuthsRefreshTokenInputValidator()
        {
            RuleFor(x => x.Token).NotNull().NotEmpty();
            RuleFor(x => x.RefreshToken).NotNull().NotEmpty();
        }
    }
}