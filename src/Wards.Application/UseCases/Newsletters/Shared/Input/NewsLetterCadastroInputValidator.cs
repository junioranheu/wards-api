using FluentValidation;
using static Wards.Utils.Fixtures.Validate;

namespace Wards.Application.UseCases.NewslettersCadastros.Shared.Input
{
    public sealed class NewsletterCadastroInputValidator : AbstractValidator<NewsletterCadastroInput>
    {
        public NewsletterCadastroInputValidator()
        {
            RuleFor(x => x.Email).NotNull().NotEmpty().Must((rootObj, obj) => { return ValidarEmail(email: rootObj.Email); }).WithMessage("O e-mail é inválido");
        }
    }
}