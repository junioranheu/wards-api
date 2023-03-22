using FluentValidation;

namespace Wards.Application.UsesCases.Auths.Shared.Input
{
    public sealed class LogarInputValidator : AbstractValidator<LogarInput>
    {
        public LogarInputValidator()
        {
            RuleFor(x => x.Login).NotNull().NotEmpty();
            RuleFor(x => x.Senha).NotNull().NotEmpty();
        }
    }
}