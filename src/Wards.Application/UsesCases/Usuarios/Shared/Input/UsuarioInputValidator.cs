using FluentValidation;

namespace Wards.Application.UsesCases.Usuarios.Shared.Input
{
    public class UsuarioInputValidator : AbstractValidator<UsuarioInput>
    {
        public UsuarioInputValidator()
        {
            RuleFor(x => x.NomeCompleto).NotEmpty();
            RuleFor(x => x.NomeUsuarioSistema).NotEmpty();
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Senha).NotEmpty();
            RuleFor(x => x.Chamado).NotEmpty();
        }
    }
}