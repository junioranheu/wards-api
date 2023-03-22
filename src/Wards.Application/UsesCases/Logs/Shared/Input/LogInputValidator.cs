using FluentValidation;

namespace Wards.Application.UsesCases.Logs.Shared.Input
{
    public class LogInputValidator : AbstractValidator<LogInput>
    {
        public LogInputValidator()
        {
            RuleFor(x => x.TipoRequisicao).NotNull().NotEmpty();
            RuleFor(x => x.Endpoint).NotNull().NotEmpty();
        }
    }
}