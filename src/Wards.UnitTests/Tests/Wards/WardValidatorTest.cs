using FluentValidation.TestHelper;
using Wards.Application.UseCases.Wards.Shared.Input;
using Xunit;

namespace Wards.UnitTests.Tests.Wards
{
    public sealed class WardValidatorTest
    {
        private readonly WardInputAltValidator _validator;

        public WardValidatorTest()
        {
            _validator = new WardInputAltValidator();
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

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("Fazueli", false)]
        [InlineData("Para criar uma API você deverá bla bla bla", true)]
        public void Validar_Conteudo(string conteudo, bool esperado)
        {
            // Arrange;
            var model = new WardInput { Conteudo = conteudo };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            if (esperado)
            {
                result.ShouldNotHaveValidationErrorFor(x => x.Conteudo);
            }
            else
            {
                result.ShouldHaveValidationErrorFor(x => x.Conteudo);
            }
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(0, false)]
        [InlineData(22, true)]
        public void Validar_UsuarioId(int? usuarioId, bool esperado)
        {
            // Arrange;
            var model = new WardInput { UsuarioId = usuarioId };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            if (esperado)
            {
                result.ShouldNotHaveValidationErrorFor(x => x.UsuarioId);
            }
            else
            {
                result.ShouldHaveValidationErrorFor(x => x.UsuarioId);
            }
        }
    }
}