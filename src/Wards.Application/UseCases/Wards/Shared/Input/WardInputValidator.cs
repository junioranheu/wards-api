﻿using FluentValidation;

namespace Wards.Application.UseCases.Wards.Shared.Input
{
    public sealed class WardInputValidator : AbstractValidator<WardInput>
    {
        public WardInputValidator()
        {
            RuleFor(x => x.Titulo).NotNull().NotEmpty().MinimumLength(3);
            RuleFor(x => x.Conteudo).NotNull().NotEmpty().MinimumLength(10);
            RuleFor(x => x.UsuarioId).NotNull().GreaterThan(0);
        }
    }
}