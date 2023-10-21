using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wards.Application.UseCases.ChatGPT.EnviarMensagem.Commands;

namespace Wards.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatGPTController : Controller
    {
        private readonly IEnviarMensagemCommand _enviarMensagemCommand;

        public ChatGPTController(IEnviarMensagemCommand enviarMensagemCommand)
        {
            _enviarMensagemCommand = enviarMensagemCommand;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(StatusCodes))]
        public async Task<ActionResult<string>> EnviarMensagem(string input)
        {
            string resp = await _enviarMensagemCommand.Execute(input);
            return Ok(resp);
        }
    }
}