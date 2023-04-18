using FluentValidation;

namespace Wards.Application.UsesCases.Wards.Shared.Input
{
    public sealed class WardInputValidator : AbstractValidator<WardInput>
    {
        public WardInputValidator()
        {
            RuleFor(x => x.Titulo).NotNull().NotEmpty().MinimumLength(3);
            RuleFor(x => x.Conteudo).NotNull().NotEmpty().MinimumLength(10);
        }
    }
}