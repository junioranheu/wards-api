using System.ComponentModel;

namespace Wards.Domain.Enums
{
    public enum CodigosErrosEnum
    {
        // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=- 100 =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        [Description("Já existe outro usuário cadastrado com este e-mail ou nome de usuário")]
        UsuarioExistente = 101,

        [Description("Tipo de usuário não permitido")]
        TipoUsuarioNaoPermitido = 102,

        [Description("Tipo de usuário não encontrado")]
        TipoUsuarioNaoEncontrado = 103,

        [Description("Os requisitos de senha não foram cumpridos. A senha deve ser mais segura")]
        RequisitosSenhaNaoCumprido = 104,

        [Description("O nome completo ou nome de usuário não atingem o mínimo de caracteres necessários")]
        RequisitosNome = 105,

        [Description("E-mail inválido")]
        EmailInvalido = 106,

        [Description("O usuário ou senha estão incorretos")]
        UsuarioSenhaIncorretos = 107,

        [Description("Esta conta está desativada")]
        ContaDesativada = 108,

        [Description("Este usuário não foi encontrado")]
        UsuarioNaoEncontrado = 109,

        [Description("Este código de verificação é inválido")]
        CodigoVerificacaoInvalido = 110,

        [Description("Este código está expirado")]
        CodigoExpirado = 111,

        [Description("Esta conta já se encontra verificada portanto não é possível verificá-la novamente")]
        ContaJaVerificada = 112,

        [Description("Esta conta ainda não foi verificada. Um novo e-mail foi enviado para você. Verifique-a e tente novamente mais tarde")]
        ContaNaoVerificadaMasNovoEmailVerificacaoEnviado = 113,

        [Description("Esta conta ainda não foi verificada. Além disso, houve um erro ao enviar um novo e-mail de vericação. Tente novamente mais tarde")]
        ContaNaoVerificadaComFalhaNoEnvioNovoEmailVerificacao = 114,

        [Description("As senhas não se coincidem")]
        SenhasNaoCoincidem = 115,

        [Description("A senha atual não está correta, portanto o processo foi abortado")]
        SenhaIncorretaAoAtualizar = 116,

        [Description("Um e-mail de recuperação de senha já foi enviado para você há pouco tempo. Tente novamente mais tarde")]
        EmailRecuperacaoJaEnviado = 117,

        [Description("Esse código de recuperação não está vinculado a nenhum usuário")]
        HashRecuperacaoNaoExiste = 118,

        [Description("Um e-mail de validação de conta já foi enviado para você há pouco tempo. Tente novamente mais tarde")]
        EmailValidacaoJaEnviado = 119,

        // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=- 200 =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        [Description("Processo concluído com sucesso")]
        OK = 200,

        // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=- 400 =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        [Description("Não autorizado")]
        NaoAutorizado = 403,

        [Description("Dado não encontrado")]
        NaoEncontrado = 404,

        [Description("Refresh token inválido")]
        RefreshTokenInvalido = 411,
    }
}
