using FluentValidation.TestHelper;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Xunit;

namespace Wards.UnitTests.Tests.Usuarios
{
    public sealed class UsuarioValidatorTest
    {
        private readonly CriarUsuarioInputValidator _validator;

        public UsuarioValidatorTest()
        {
            _validator = new CriarUsuarioInputValidator();
        }

        [Fact]
        public void DeveRetornarErro_QuandoNomeInvalido()
        {
            // Arrange;
            var model = new CriarUsuarioInput { NomeCompleto = null };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            result.ShouldHaveValidationErrorFor(x => x.NomeCompleto);
        }

        [Fact]
        public void DeveRetornarOk_QuandoNomeValido()
        {
            // Arrange;
            var model = new CriarUsuarioInput { NomeCompleto = "Junior" };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            result.ShouldNotHaveValidationErrorFor(x => x.NomeCompleto);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("junior")]
        [InlineData("junior@")]
        [InlineData("junior@gmail")]
        public void DeveRetornarErro_QuandoEmailInvalido(string email)
        {
            // Arrange;
            var model = new CriarUsuarioInput { Email = email };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Theory]
        [InlineData("junior@gmail.com")]
        [InlineData("mariana@hotmail.com")]
        [InlineData("otavio@bol.com.br")]
        public void DeveRetornarOk_QuandoEmailValido(string email)
        {
            // Arrange;
            var model = new CriarUsuarioInput { Email = email };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void DeveRetornarErro_QuandoChamadoInvalido()
        {
            // Arrange;
            var model = new CriarUsuarioInput { Chamado = null };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            result.ShouldHaveValidationErrorFor(x => x.Chamado);
        }

        [Fact]
        public void DeveRetornarOk_QuandoChamadoInvalido()
        {
            // Arrange;
            var model = new CriarUsuarioInput { Chamado = "#1" };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            result.ShouldNotHaveValidationErrorFor(x => x.Chamado);
        }
    }
}