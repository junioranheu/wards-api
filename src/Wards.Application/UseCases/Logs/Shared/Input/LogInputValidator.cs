using FluentValidation;

namespace Wards.Application.UseCases.Logs.Shared.Input
{
    public sealed class LogInputValidator : AbstractValidator<LogInput>
    {
        public LogInputValidator()
        {
            RuleFor(x => x.TipoRequisicao).NotNull().NotEmpty();
            RuleFor(x => x.Endpoint).NotNull().NotEmpty();
        }
    }
}