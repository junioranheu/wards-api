using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wards.Application.Services.Usuarios.ObterUsuarioCache;
using Wards.Domain.Entities;

namespace Wards.API.Controllers
{
    public abstract class BaseController<T> : Controller
    {
        protected BaseController() { }

        protected string ObterUsuarioEmail()
        {
            if (User.Identity!.IsAuthenticated)
            {
                // Obter o e-mail do usuário pela Azure;
                // var claim = User.Claims.First(c => c.Type == "preferred_username");
                // return claim.Value ?? string.Empty;

                // Obter o e-mail do usuário pela autenticação própria;
                string email = User.FindFirst(ClaimTypes.Email)!.Value;
                return email ?? string.Empty;
            }

            return string.Empty;
        }

        protected async Task<int> ObterUsuarioId()
        {
            var service = HttpContext.RequestServices.GetService<IObterUsuarioCacheService>();
            Usuario? usuario = await service!.Execute(ObterUsuarioEmail());

            return usuario is not null ? usuario.UsuarioId : 0;
        }
    }
}
