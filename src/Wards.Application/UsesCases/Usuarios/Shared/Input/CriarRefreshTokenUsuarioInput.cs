﻿namespace Wards.Application.UsesCases.Usuarios.Shared.Input
{
    public sealed class CriarRefreshTokenUsuarioInput
    {
        public string? Token { get; set; } = null;

        public string? RefreshToken { get; set; } = null;
    }
}