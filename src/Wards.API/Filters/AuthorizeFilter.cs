﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole;
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
                IEnumerable<UsuarioRole> usuarioRoles = await GetUsuarioRoles(context);
                IsUsuarioTemAcesso(context, usuarioRoles, _rolesNecessarias);
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

        private static async Task<IEnumerable<UsuarioRole>> GetUsuarioRoles(AuthorizationFilterContext context)
        {
            var obterUsuarioRoleUseCase = context.HttpContext.RequestServices.GetService<IObterUsuarioRoleUseCase>();
            IEnumerable<UsuarioRole> usuarioRoles = await obterUsuarioRoleUseCase.ObterUsuarioRolesByEmailComCache(GetUsuarioEmail(context));

            return usuarioRoles;
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

        private static bool IsUsuarioTemAcesso(AuthorizationFilterContext context, IEnumerable<UsuarioRole> usuarioRoles, int[] _rolesNecessarias)
        {
            if (_rolesNecessarias.Length == 0)
            {
                return true;
            }

            bool isUsuarioTemAcesso = usuarioRoles.Any(x => _rolesNecessarias.Any(y => x.RoleId == y));

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
