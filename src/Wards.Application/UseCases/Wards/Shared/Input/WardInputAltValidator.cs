using FluentValidation;

namespace Wards.Application.UseCases.Wards.Shared.Input
{
    public sealed class WardInputAltValidator : AbstractValidator<WardInputAlt>
    {
        public WardInputAltValidator()
        {
            RuleFor(x => x.Titulo).NotNull().NotEmpty().MinimumLength(3);
            RuleFor(x => x.Conteudo).NotNull().NotEmpty().MinimumLength(10);
        }
    }
}