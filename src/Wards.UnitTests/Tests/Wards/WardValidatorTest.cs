﻿using FluentValidation.TestHelper;
using Wards.Application.UsesCases.Wards.Shared.Input;
using Xunit;

namespace Wards.UnitTests.Tests.Wards
{
    public sealed class WardValidatorTest
    {
        private readonly WardInputValidator _validator;

        public WardValidatorTest()
        {
            _validator = new WardInputValidator();
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("22", false)]
        [InlineData("Como criar uma API", true)]
        public void Validar_Titulo(string titulo, bool esperado)
        {
            // Arrange;
            var model = new WardInput { Titulo = titulo };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            if (esperado)
            {
                result.ShouldNotHaveValidationErrorFor(x => x.Titulo);
            }
            else
            {
                result.ShouldHaveValidationErrorFor(x => x.Titulo);
            }
        }
    }
}