using FluentValidation;
using static Wards.Utils.Fixtures.Validate;

namespace Wards.Application.UseCases.Usuarios.Shared.Input
{
    public sealed class CriarUsuarioInputValidator : AbstractValidator<CriarUsuarioInput>
    {
        public CriarUsuarioInputValidator()
        {
            RuleFor(x => x.NomeCompleto).NotNull().NotEmpty().MinimumLength(3);
            RuleFor(x => x.NomeUsuarioSistema).NotNull().NotEmpty().MinimumLength(3);
            RuleFor(x => x.Email).NotNull().NotEmpty().Must((rootObj, obj) => { return ValidarEmail(email: rootObj.Email); }).WithMessage("O e-mail é inválido");

            RuleFor(x => x.Senha).NotNull().NotEmpty().Must((rootObj, obj, context) =>
            {
                var (isValido, mensagemErro) = ValidarSenha(senha: rootObj.Senha!, nomeCompleto: rootObj.NomeCompleto!, nomeUsuario: rootObj.NomeUsuarioSistema!, email: rootObj.Email!);
                context.MessageFormatter.AppendArgument("_avisoValidarSenha", mensagemErro);
                return isValido;
            }).WithMessage("{_avisoValidarSenha}");

            //RuleFor(x => x.Senha).NotNull().NotEmpty().WithMessage("Sua senha não pode ser vazia").
            //    MinimumLength(6).WithMessage("Sua senha não pode ter menos de 6 caracteres").
            //    MaximumLength(24).WithMessage("Sua senha não pode ultrapassar 24 caracteres").
            //    Matches(@"[A-Z]+").WithMessage("Sua senha deve ter pelo menos um caracter maiúsculo").
            //    Matches(@"[a-z]+").WithMessage("Sua senha deve ter pelo menos um caracter minúsculo").
            //    Matches(@"[0-9]+").WithMessage("Sua senha deve ter pelo menos um número").
            //    Matches(@"[][""!@$%^&*(){}:;<>,.?/+_=|'~\\-]").WithMessage("Sua senha deve ter pelo menos um caracter especial");

            RuleFor(x => x.Chamado).NotNull().NotEmpty();
            RuleFor(x => x.UsuariosRolesId).Must((rootObj, obj) =>
            {
                if (rootObj.UsuariosRolesId is not null)
                {
                    if (rootObj.UsuariosRolesId.Contains(0))
                    {
                        return false;
                    }
                }

                return true;
            }).WithMessage("Role inexistente");
        }
    }
}