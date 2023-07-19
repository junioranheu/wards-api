using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wards.API.Filters;
using Wards.Application.UseCases.Auths.Shared.Output;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Application.UseCases.Usuarios.AutenticarUsuario;
using Wards.Application.UseCases.Usuarios.CriarRefreshTokenUsuario;
using Wards.Application.UseCases.Usuarios.CriarUsuario;
using Wards.Application.UseCases.Usuarios.ListarUsuario;
using Wards.Application.UseCases.Usuarios.ObterUsuario;
using Wards.Application.UseCases.Usuarios.Shared.Input;
using Wards.Application.UseCases.Usuarios.Shared.Output;
using Wards.Application.UseCases.Usuarios.SolicitarVerificacaoContaUsuario;
using Wards.Application.UseCases.Usuarios.VerificarContaUsuario;
using Wards.Application.UseCases.UsuariosRoles.CriarUsuarioRole;
using Wards.Domain.Enums;
using static Wards.Utils.Fixtures.Get;

namespace Wards.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : BaseController<UsuariosController>
    {
        private readonly IAutenticarUsuarioUseCase _autenticarUseCase;
        private readonly ICriarRefreshTokenUsuarioUseCase _criarRefreshTokenUsuarioUseCase;
        private readonly ICriarUsuarioUseCase _criarUseCase;
        private readonly ICriarUsuarioRoleUseCase _criarUsuarioRoleUseCase;
        private readonly IListarUsuarioUseCase _listarUseCase;
        private readonly IObterUsuarioUseCase _obterUseCase;
        private readonly ISolicitarVerificacaoContaUsuarioUseCase _solicitarVerificacaoContaUsuarioUseCase;
        private readonly IVerificarContaUsuarioUseCase _verificarContaUsuarioUseCase;

        public UsuariosController(
            IAutenticarUsuarioUseCase autenticarUseCase,
            ICriarUsuarioUseCase criarUseCase,
            ICriarUsuarioRoleUseCase criarUsuarioRoleUseCase,
            ICriarRefreshTokenUsuarioUseCase criarRefreshTokenUsuarioUseCase,
            IListarUsuarioUseCase listarUseCase,
            IObterUsuarioUseCase obterUseCase,
            ISolicitarVerificacaoContaUsuarioUseCase solicitarVerificacaoContaUsuarioUseCase,
            IVerificarContaUsuarioUseCase verificarContaUsuarioUseCase)
        {
            _autenticarUseCase = autenticarUseCase;
            _criarRefreshTokenUsuarioUseCase = criarRefreshTokenUsuarioUseCase;
            _criarUseCase = criarUseCase;
            _criarUsuarioRoleUseCase = criarUsuarioRoleUseCase;
            _listarUseCase = listarUseCase;
            _obterUseCase = obterUseCase;
            _solicitarVerificacaoContaUsuarioUseCase = solicitarVerificacaoContaUsuarioUseCase;
            _verificarContaUsuarioUseCase = verificarContaUsuarioUseCase;
        }

        [ApiExplorerSettings(IgnoreApi = true)] // ***
        [HttpPost("autenticarLazy")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(AutenticarUsuarioOutput))]
        public async Task<ActionResult<AutenticarUsuarioOutput>> AutenticarLazy()
        {
            if (User.Identity!.IsAuthenticated)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.UsuarioJaAutenticado));
            }

            var resp = await _autenticarUseCase.Execute(new AutenticarUsuarioInput() { Login = "adm", Senha = "123" });

            return Ok(resp.Token);
        }

        [HttpPost("autenticar")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutenticarUsuarioOutput))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(AutenticarUsuarioOutput))]
        public async Task<ActionResult<AutenticarUsuarioOutput>> Autenticar(AutenticarUsuarioInput input)
        {
            if (User.Identity!.IsAuthenticated)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.UsuarioJaAutenticado));
            }

            var resp = await _autenticarUseCase.Execute(input);

            return Ok(resp);
        }

        [HttpPost("criarRefreshToken")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CriarRefreshTokenUsuarioOutput))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(UsuarioOutput))]
        public async Task<ActionResult<CriarRefreshTokenUsuarioOutput>> CriarRefreshToken(CriarRefreshTokenUsuarioInput input)
        {
            var resp = await _criarRefreshTokenUsuarioUseCase.Execute(input.Token!, input.RefreshToken!, ObterUsuarioEmail());

            return Ok(resp);
        }

        [HttpPost]
        // [AuthorizeFilter(UsuarioRoleEnum.Administrador, UsuarioRoleEnum.Suporte)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutenticarUsuarioOutput))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(AutenticarUsuarioOutput))]
        public async Task<ActionResult<AutenticarUsuarioOutput>> Criar(CriarUsuarioInput input)
        {
            var resp = await _criarUseCase.Execute(input);
            await _criarUsuarioRoleUseCase.Execute(input.UsuariosRolesId!, resp!.UsuarioId);

            return Ok(resp);
        }

        [HttpGet("listar")]
        [AuthorizeFilter(UsuarioRoleEnum.Administrador)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UsuarioOutput>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(UsuarioOutput))]
        public async Task<ActionResult<IEnumerable<UsuarioOutput>>> Listar([FromQuery] PaginacaoInput input)
        {
            var lista = await _listarUseCase.Execute(input);

            if (!lista.Any())
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            return Ok(lista);
        }

        [HttpGet]
        [AuthorizeFilter]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsuarioOutput))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(UsuarioOutput))]
        public async Task<ActionResult<UsuarioOutput>> Obter(int id)
        {
            var item = await _obterUseCase.Execute(id);

            if (item is null)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            return Ok(item);
        }

        [HttpPost("solicitarVerificacaoConta")]
        [AuthorizeFilter]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(UsuarioOutput))]
        public async Task<ActionResult<UsuarioOutput>> SolicitarVerificacaoConta()
        {
            int usuarioId = await ObterUsuarioId();
            await _solicitarVerificacaoContaUsuarioUseCase.Execute(usuarioId);

            return Ok(true);
        }

        [HttpPut("verificarConta/{codigoVerificacao}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(UsuarioOutput))]
        public async Task<ActionResult<UsuarioOutput>> VerificarConta(string codigoVerificacao)
        {
            if (string.IsNullOrEmpty(codigoVerificacao))
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.CodigoVerificacaoInvalido));
            }

            await _verificarContaUsuarioUseCase.Execute(codigoVerificacao);

            return Ok(true);
        }
    }
}