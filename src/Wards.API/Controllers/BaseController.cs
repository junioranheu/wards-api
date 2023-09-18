using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wards.Application.Services.Usuarios.ObterUsuarioCache;
using Wards.Application.UseCases.Usuarios.Shared.Output;

namespace Wards.API.Controllers
{
    public abstract class BaseController<T> : Controller
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        protected BaseController()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

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
            UsuarioOutput? usuario = await service!.Execute(ObterUsuarioEmail());

            return usuario is not null ? usuario.UsuarioId : 0;
        }

        #region cancellationToken
        protected CancellationToken CancellationToken => _cancellationTokenSource.Token;

        protected void CancelarToken()
        {
            _cancellationTokenSource.Cancel();
        }

        protected async Task<TResult> WithCancellationToken<TResult>(Func<CancellationToken, Task<TResult>> operation)
        {
            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken);

            try
            {
                return await operation(linkedTokenSource.Token);
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == CancellationToken)
            {
                throw new TaskCanceledException("Processo cancelado.", ex);
            }
        }
        #endregion
    }
}