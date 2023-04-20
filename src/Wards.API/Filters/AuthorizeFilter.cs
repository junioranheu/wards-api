using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Security.Claims;
using Wards.Application.Services.Usuarios.ListarUsuarioRolesCache;
using Wards.Application.UseCases.UsuariosRoles.Shared.Output;
using Wards.Domain.Entities;
using Wards.Domain.Enums;

namespace Wards.API.Filters
{
    public class AuthAttribute : TypeFilterAttribute
    {
        public AuthAttribute(params UsuarioRoleEnum[] roles) : base(typeof(AuthorizeFilter))
        {
            Arguments = new object[] { roles };
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class AuthorizeFilter : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly int[] _rolesNecessarias;

        public AuthorizeFilter(params UsuarioRoleEnum[] roles)
        {
            _rolesNecessarias = NormalizarRoles(roles);
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (IsUsuarioAutenticado(context))
            {
                IEnumerable<UsuarioRoleOutput>? usuarioRoles = await ListarUsuarioRoles(context);
                IsUsuarioTemAcesso(context, usuarioRoles, _rolesNecessarias);
            }
        }

        private static bool IsUsuarioAutenticado(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity!.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return false;
            }

            return true;
        }

        private static string ObterUsuarioEmail(AuthorizationFilterContext filterContextExecuted)
        {
            if (filterContextExecuted.HttpContext.User.Identity!.IsAuthenticated)
            {
                // Obter o e-mail do usuário pela Azure;
                //var claim = filterContextExecuted.HttpContext.User.Claims.First(c => c.Type == "preferred_username");
                //return claim.Value ?? string.Empty;

                // Obter o e-mail do usuário pela autenticação própria;
                string email = filterContextExecuted.HttpContext.User.FindFirst(ClaimTypes.Email)!.Value;
                return email ?? string.Empty;
            }

            return string.Empty;
        }

        private static async Task<IEnumerable<UsuarioRoleOutput>?> ListarUsuarioRoles(AuthorizationFilterContext context)
        {
            var service = context.HttpContext.RequestServices.GetService<IListarUsuarioRolesCacheService>();
            IEnumerable<UsuarioRoleOutput>? usuarioRoles = await service!.Execute(ObterUsuarioEmail(context));

            return usuarioRoles;
        }

        private static bool IsUsuarioTemAcesso(AuthorizationFilterContext context, IEnumerable<UsuarioRoleOutput>? usuarioRoles, int[] _rolesNecessarias)
        {
            if (_rolesNecessarias.Length == 0)
            {
                return true;
            }

            bool isUsuarioTemAcesso = usuarioRoles!.Any(x => _rolesNecessarias.Any(y => x.RoleId == (UsuarioRoleEnum)y));

            if (!isUsuarioTemAcesso)
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                return false;
            }

            return true;
        }

        private static int[] NormalizarRoles(UsuarioRoleEnum[] roles)
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