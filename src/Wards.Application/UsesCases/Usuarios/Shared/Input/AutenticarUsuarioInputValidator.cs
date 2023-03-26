using FluentValidation;

namespace Wards.Application.UsesCases.Usuarios.Shared.Input
{
    public sealed class AutenticarUsuarioInputValidator : AbstractValidator<AutenticarUsuarioInput>
    {
        public AutenticarUsuarioInputValidator()
        {
            RuleFor(x => x.Login).NotNull().NotEmpty();
            RuleFor(x => x.Senha).NotNull().NotEmpty();
        }
    }
}