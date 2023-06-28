using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Wards.Application.Services.Usuarios.ObterUsuarioCache;
using Wards.Application.UseCases.Usuarios.Shared.Output;

namespace Wards.API.Filters.Base
{
    public sealed class BaseFilter
    {
        public BaseFilter() { }

        #region usuario_id
        internal async Task<int> BaseObterUsuarioId(ActionExecutedContext context)
        {
            return await BaseObterUsuarioId(context.HttpContext.RequestServices.GetService<IObterUsuarioCacheService>(), BaseObterUsuarioEmail(context));
        }

        internal async Task<int> BaseObterUsuarioId(AuthorizationFilterContext context)
        {
            return await BaseObterUsuarioId(context.HttpContext.RequestServices.GetService<IObterUsuarioCacheService>(), BaseObterUsuarioEmail(context));
        }

        internal async Task<int> BaseObterUsuarioId(ExceptionContext context)
        {
            return await BaseObterUsuarioId(context.HttpContext.RequestServices.GetService<IObterUsuarioCacheService>(), BaseObterUsuarioEmail(context));
        }

        private static async Task<int> BaseObterUsuarioId(IObterUsuarioCacheService? service, string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return 0;
            }

            UsuarioOutput? usuario = await service!.Execute(email);
            int usuarioId = usuario is not null ? usuario.UsuarioId : 0;

            return usuarioId;
        }
        #endregion

        #region usuario_email
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Marcar membros como estáticos", Justification = "<Pendente>")]
        internal string BaseObterUsuarioEmail(dynamic context)
        {
            if (context is ActionExecutedContext actionExecutedContext)
            {
                return ObterUsuarioEmail(actionExecutedContext);
            }
            else if (context is AuthorizationFilterContext authorizationFilterContext)
            {
                return ObterUsuarioEmail(authorizationFilterContext);
            }
            else if (context is ExceptionContext exceptionContext)
            {
                return ObterUsuarioEmail(exceptionContext);
            }

            return string.Empty;

            static string ObterUsuarioEmail(dynamic context)
            {
                if (context.HttpContext.User.Identity!.IsAuthenticated)
                {
                    // Obter o e-mail do usuário pela Azure;
                    // var claim = filterContextExecuted.HttpContext.User.Claims.First(c => c.Type == "preferred_username");
                    // return claim.Value ?? string.Empty;

                    // Obter o e-mail do usuário pela autenticação própria;
                    string email = context.HttpContext.User.FindFirst(ClaimTypes.Email)!.Value;
                    return email ?? string.Empty;
                }

                return string.Empty;
            }
        }
        #endregion
    }
}