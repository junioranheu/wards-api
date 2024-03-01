using System.ComponentModel;

namespace Wards.Domain.Enums
{
    public enum CodigoErroEnum
    {
        #region 900
        [Description("Já existe outro usuário cadastrado com este e-mail ou nome de usuário")]
        UsuarioExistente = 901,

        [Description("Tipo de usuário não permitido")]
        TipoUsuarioNaoPermitido = 902,

        [Description("Tipo de usuário não encontrado")]
        TipoUsuarioNaoEncontrado = 903,

        [Description("Os requisitos de senha não foram cumpridos. A senha deve ser mais segura")]
        RequisitosSenhaNaoCumprido = 904,

        [Description("O nome completo ou nome de usuário não atingem o mínimo de caracteres necessários")]
        RequisitosNome = 905,

        [Description("E-mail inválido")]
        EmailInvalido = 906,

        [Description("Usuário ou senha estão incorretos")]
        UsuarioSenhaIncorretos = 907,

        [Description("Esta conta está desativada")]
        ContaDesativada = 908,

        [Description("Este usuário não foi encontrado")]
        UsuarioNaoEncontrado = 909,

        [Description("Este código de verificação é inválido")]
        CodigoVerificacaoInvalido = 910,

        [Description("Este código está expirado")]
        CodigoExpirado = 911,

        [Description("Esta conta já se encontra verificada portanto não é possível verificá-la novamente")]
        ContaJaVerificada = 912,

        [Description("Esta conta ainda não foi verificada. Um novo e-mail foi enviado para você. Verifique-a e tente novamente mais tarde")]
        ContaNaoVerificadaMasNovoEmailVerificacaoEnviado = 913,

        [Description("Esta conta ainda não foi verificada. Além disso, houve um erro ao enviar um novo e-mail de vericação. Tente novamente mais tarde")]
        ContaNaoVerificadaComFalhaNoEnvioNovoEmailVerificacao = 914,

        [Description("As senhas não se coincidem")]
        SenhasNaoCoincidem = 915,

        [Description("A senha atual não está correta, portanto o processo foi abortado")]
        SenhaIncorretaAoAtualizar = 916,

        [Description("Um e-mail de recuperação de senha já foi enviado para você há pouco tempo. Tente novamente mais tarde")]
        EmailRecuperacaoJaEnviado = 917,

        [Description("Esse código de recuperação não está vinculado a nenhum usuário")]
        HashRecuperacaoNaoExiste = 918,

        [Description("Um e-mail de validação de conta já foi enviado para você há pouco tempo. Tente novamente mais tarde")]
        EmailValidacaoJaEnviado = 919,

        [Description("Você já está autenticado, portanto não pode fazer isso agora")]
        UsuarioJaAutenticado = 920,

        [Description("Permissão não encontrada")]
        TipoRoleNaoEncontrado = 921,

        [Description("Este e-mail já está cadastrado na Newsletter")]
        NewsletterEmailJaCadastrado = 922,

        [Description("Formato de arquivo não permitido. Apenas imagens são permitidas")]
        FormatoDeArquivoNaoPermitido_ApenasImagemPermitidas = 923,

        [Description("Arquivo a ser importado está em um formato inválido")]
        ArquivoImportFormatoInvalido = 924,
        #endregion

        #region 200
        [Description("Processo concluído com sucesso")]
        OK = 200,
        #endregion

        #region 400
        [Description("O servidor não pode ou não irá processar a requisição devido a alguma coisa que foi entendida como um erro do cliente")]
        BadRequest = 400,

        [Description("Não autorizado")]
        NaoAutorizado = 403,

        [Description("Registro não encontrado")]
        NaoEncontrado = 404,

        [Description("Refresh token inválido")]
        RefreshTokenInvalido = 411,
        #endregion

        #region 500
        [Description("Houve um erro interno")]
        ErroInterno = 500,

        [Description("Houve um problema ao resetar a base de dados")]
        ErroDBInitialize = 510,
        #endregion
    }
}
