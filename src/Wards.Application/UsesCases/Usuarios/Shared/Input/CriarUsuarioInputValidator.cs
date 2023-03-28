using FluentValidation;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Usuarios.Shared.Input
{
    public sealed class CriarUsuarioInputValidator : AbstractValidator<CriarUsuarioInput>
    {
        public CriarUsuarioInputValidator()
        {
            RuleFor(x => x.NomeCompleto).NotNull().NotEmpty().MinimumLength(3);
            RuleFor(x => x.NomeUsuarioSistema).NotNull().NotEmpty().MinimumLength(3);
            RuleFor(x => x.Email).NotNull().NotEmpty().Must((x, email) => { return ValidarEmail(email: email); }).WithMessage("O e-mail é inválido");

            RuleFor(x => x.Senha).NotNull().NotEmpty().Must((x, senha, context) =>
            {
                var validarSenha = ValidarSenha(senha: x.Senha!, nomeCompleto: x.NomeCompleto!, nomeUsuario: x.NomeUsuarioSistema!, email: x.Email!);
                context.MessageFormatter.AppendArgument("AvisoValidarSenha", validarSenha.Item2);
                return validarSenha.Item1;
            }).WithMessage("{AvisoValidarSenha}");

            //RuleFor(x => x.Senha).NotNull().NotEmpty().WithMessage("Sua senha não pode ser vazia").
            //    MinimumLength(6).WithMessage("Sua senha não pode ter menos de 6 caracteres").
            //    MaximumLength(24).WithMessage("Sua senha não pode ultrapassar 24 caracteres").
            //    Matches(@"[A-Z]+").WithMessage("Sua senha deve ter pelo menos um caracter maiúsculo").
            //    Matches(@"[a-z]+").WithMessage("Sua senha deve ter pelo menos um caracter minúsculo").
            //    Matches(@"[0-9]+").WithMessage("Sua senha deve ter pelo menos um número").
            //    Matches(@"[][""!@$%^&*(){}:;<>,.?/+_=|'~\\-]").WithMessage("Sua senha deve ter pelo menos um caracter especial");

            RuleFor(x => x.Chamado).NotNull().NotEmpty();
            RuleFor(x => x.UsuariosRolesId).Must((x, usuariosRolesId) =>
            {
                if (usuariosRolesId is not null)
                    if (usuariosRolesId.Contains(0))
                        return false;

                return true;
            }).WithMessage("Role inexistente");
        }
    }
}