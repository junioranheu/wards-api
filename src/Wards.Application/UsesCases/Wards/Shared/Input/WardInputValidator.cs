using FluentValidation;

namespace Wards.Application.UsesCases.Wards.Shared.Input
{
    public sealed class WardInputValidator : AbstractValidator<WardInput>
    {
        public WardInputValidator()
        {
            RuleFor(x => x.Conteudo).NotNull().NotEmpty();
        }
    }
}