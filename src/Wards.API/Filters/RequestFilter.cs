using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Wards.API.Filters
{
    public class AuthAttribute : TypeFilterAttribute
    {
        public AuthAttribute(params UsuarioPerfilEnum[] roles) : base(typeof(AuthorizeFilter))
        {
            Arguments = new object[] { roles };
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class AuthorizeFilter : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly int[] _rolesNecessarias;

        public AuthorizeFilter(params UsuarioPerfilEnum[] roles)
        {
            _rolesNecessarias = NormalizarRoles(roles);
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (IsUsuarioAutenticado(context))
            {
                int[] usuarioPerfilLista = await GetListaUsuarioPerfis(context);
                IsUsuarioTemAcesso(context, usuarioPerfilLista, _rolesNecessarias);
            }
        }

        private static bool IsUsuarioAutenticado(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return false;
            }

            return true;
        }

        private static async Task<int[]> GetListaUsuarioPerfis(AuthorizationFilterContext context)
        {
            var obterUsuarioUseCase = context.HttpContext.RequestServices.GetService<IObterUsuarioUseCase>();
            int[] idUsuarioPerfilLista = await obterUsuarioUseCase.GetListaIdUsuarioPerfil(GetUsuarioEmail(context));

            return idUsuarioPerfilLista;
        }

        private static string GetUsuarioEmail(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var claim = context.HttpContext.User.Claims.First(c => c.Type == "preferred_username");
                return claim.Value ?? string.Empty;
            }

            return string.Empty;
        }

        private static bool IsUsuarioTemAcesso(AuthorizationFilterContext context, int[] usuarioPerfilLista, int[] _rolesNecessarias)
        {
            if (_rolesNecessarias.Length == 0)
            {
                return true;
            }

            bool isUsuarioTemAcesso = usuarioPerfilLista.Any(x => _rolesNecessarias.Any(y => x == y));

            if (!isUsuarioTemAcesso)
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                return false;
            }

            return true;
        }

        private static int[] NormalizarRoles(UsuarioPerfilEnum[] roles)
        {
            List<int> r = new();

            foreach (var role in roles)
            {
                r.Add((int)role);
            }

            return r.ToArray();
        }
    }
}
