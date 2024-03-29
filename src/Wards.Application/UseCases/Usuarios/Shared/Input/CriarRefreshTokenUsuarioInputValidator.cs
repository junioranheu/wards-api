﻿using FluentValidation;

namespace Wards.Application.UseCases.Usuarios.Shared.Input
{
    public sealed class CriarRefreshTokenUsuarioInputValidator : AbstractValidator<CriarRefreshTokenUsuarioInput>
    {
        public CriarRefreshTokenUsuarioInputValidator()
        {
            RuleFor(x => x.Token).NotNull().NotEmpty();
            RuleFor(x => x.RefreshToken).NotNull().NotEmpty();
        }
    }
}