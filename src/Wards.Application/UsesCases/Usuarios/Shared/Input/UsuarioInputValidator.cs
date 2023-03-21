using FluentValidation;

namespace Wards.Application.UsesCases.Usuarios.Shared.Input
{
    public class UsuarioInputValidator : AbstractValidator<UsuarioInput>
    {
        public UsuarioInputValidator()
        {
            RuleFor(x => x.NomeCompleto).NotNull().NotEmpty().MinimumLength(3);
            RuleFor(x => x.NomeUsuarioSistema).NotNull().NotEmpty().MinimumLength(3);
            RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();

            RuleFor(x => x.Senha).NotEmpty().WithMessage("Sua senha não pode ser vazia").
                MinimumLength(6).WithMessage("Sua senha não pode ter menos de 6 caracteres").
                MaximumLength(24).WithMessage("Sua senha não pode ultrapassar 24 caracteres").
                Matches(@"[A-Z]+").WithMessage("Sua senha deve ter pelo menos um caracter maiúsculo").
                Matches(@"[a-z]+").WithMessage("Sua senha deve ter pelo menos um caracter minúsculo").
                Matches(@"[0-9]+").WithMessage("Sua senha deve ter pelo menos um número").
                Matches(@"[][""!@$%^&*(){}:;<>,.?/+_=|'~\\-]").WithMessage("Sua senha deve ter pelo menos um caracter especial");

            RuleFor(x => x.Chamado).NotNull().NotEmpty();
            RuleFor(x => x.UsuariosRolesId).NotNull().NotEmpty();
        }
    }
}