using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wards.API.Filters;
using Wards.Application.UsesCases.Auths.Shared.Output;
using Wards.Application.UsesCases.Usuarios.AutenticarUsuario;
using Wards.Application.UsesCases.Usuarios.CriarRefreshTokenUsuario;
using Wards.Application.UsesCases.Usuarios.CriarUsuario;
using Wards.Application.UsesCases.Usuarios.ListarUsuario;
using Wards.Application.UsesCases.Usuarios.ObterUsuario;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Output;
using Wards.Application.UsesCases.Usuarios.VerificarContaUsuario;
using Wards.Application.UsesCases.UsuariosRoles.CriarUsuarioRole;
using Wards.Domain.Enums;
using static Wards.Utils.Common;

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
        private readonly IVerificarContaUsuarioUseCase _verificarContaUsuarioUseCase;

        public UsuariosController(
            IAutenticarUsuarioUseCase autenticarUseCase,
            ICriarUsuarioUseCase criarUseCase,
            ICriarUsuarioRoleUseCase criarUsuarioRoleUseCase,
            ICriarRefreshTokenUsuarioUseCase criarRefreshTokenUsuarioUseCase,
            IListarUsuarioUseCase listarUseCase,
            IObterUsuarioUseCase obterUseCase,
            IVerificarContaUsuarioUseCase verificarContaUsuarioUseCase)
        {
            _autenticarUseCase = autenticarUseCase;
            _criarRefreshTokenUsuarioUseCase = criarRefreshTokenUsuarioUseCase;
            _criarUseCase = criarUseCase;
            _criarUsuarioRoleUseCase = criarUsuarioRoleUseCase;
            _listarUseCase = listarUseCase;
            _obterUseCase = obterUseCase;
            _verificarContaUsuarioUseCase = verificarContaUsuarioUseCase;
        }

        [HttpPost("autenticar")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutenticarUsuarioOutput))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(AutenticarUsuarioOutput))]
        public async Task<ActionResult<AutenticarUsuarioOutput>> Autenticar(AutenticarUsuarioInput input)
        {
            var resp = await _autenticarUseCase.Execute(input);

            if (resp.Messages!.Length > 0)
                return StatusCode(StatusCodes.Status403Forbidden, resp);

            return Ok(resp);
        }

        [HttpPost("criarRefreshToken")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CriarRefreshTokenUsuarioOutput))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(UsuarioOutput))]
        public async Task<ActionResult<CriarRefreshTokenUsuarioOutput>> CriarRefreshToken(CriarRefreshTokenUsuarioInput input)
        {
            var resp = await _criarRefreshTokenUsuarioUseCase.Execute(input.Token!, input.RefreshToken!, ObterUsuarioEmail());

            if (resp!.Messages!.Length > 0)
                return StatusCode(StatusCodes.Status403Forbidden, resp);

            return Ok(resp);
        }

        [HttpPost]
        [AuthorizeFilter(UsuarioRoleEnum.Adm, UsuarioRoleEnum.Suporte)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutenticarUsuarioOutput))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(AutenticarUsuarioOutput))]
        public async Task<ActionResult<AutenticarUsuarioOutput>> Criar(CriarUsuarioInput input)
        {
            var resp = await _criarUseCase.Execute(input);

            if (resp!.Messages!.Length > 0)
                return StatusCode(StatusCodes.Status403Forbidden, resp);

            await _criarUsuarioRoleUseCase.Execute(input.UsuariosRolesId!, resp.UsuarioId);

            return Ok(resp);
        }

        [HttpGet("listar")]
        [AuthorizeFilter(UsuarioRoleEnum.Adm)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UsuarioOutput>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(UsuarioOutput))]
        public async Task<ActionResult<IEnumerable<UsuarioOutput>>> Listar()
        {
            var resp = await _listarUseCase.Execute();

            if (resp is null)
                return StatusCode(StatusCodes.Status404NotFound, new UsuarioOutput() { Messages = new string[] { GetDescricaoEnum(CodigosErrosEnum.NaoEncontrado) } });

            return Ok(resp);
        }

        [HttpGet]
        [AuthorizeFilter]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsuarioOutput))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(UsuarioOutput))]
        public async Task<ActionResult<UsuarioOutput>> Obter(int id)
        {
            var resp = await _obterUseCase.Execute(id);

            if (resp is null)
                return StatusCode(StatusCodes.Status404NotFound, new UsuarioOutput() { Messages = new string[] { GetDescricaoEnum(CodigosErrosEnum.NaoEncontrado) } });

            return Ok(resp);
        }

        [HttpPut("verificarConta/{codigoVerificacao}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsuarioOutput))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(UsuarioOutput))]
        public async Task<ActionResult<UsuarioOutput>> VerificarConta(string codigoVerificacao)
        {
            if (string.IsNullOrEmpty(codigoVerificacao))
                return StatusCode(StatusCodes.Status403Forbidden, new UsuarioOutput() { Messages = new string[] { GetDescricaoEnum(CodigosErrosEnum.CodigoVerificacaoInvalido) } });

            var resp = await _verificarContaUsuarioUseCase.Execute(codigoVerificacao);

            if (resp!.Messages!.Length > 0)
                return StatusCode(StatusCodes.Status403Forbidden, resp);

            return resp;
        }
    }
}