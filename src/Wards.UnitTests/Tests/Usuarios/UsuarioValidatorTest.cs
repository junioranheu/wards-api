using FluentValidation.TestHelper;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Xunit;

namespace Wards.UnitTests.Tests.Usuarios
{
    public sealed class UsuarioValidatorTest
    {
        private readonly UsuarioInputValidator _validator;

        public UsuarioValidatorTest()
        {
            _validator = new UsuarioInputValidator();
        }

        [Fact]
        public void DeveRetornarErro_QuandoNomeNulo()
        {
            // Arrange;
            var model = new UsuarioInput { NomeCompleto = null };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            result.ShouldHaveValidationErrorFor(x => x.NomeCompleto);
        }

        [Fact]
        public void DeveRetornarOk_QuandoNomeValido()
        {
            // Arrange;
            var model = new UsuarioInput { NomeCompleto = "nomeTeste" };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            result.ShouldNotHaveValidationErrorFor(x => x.NomeCompleto);
        }

        [Fact]
        public void DeveRetornarErro_QuandoEmailNulo()
        {
            // Arrange;
            var model = new UsuarioInput { Email = null };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void DeveRetornarErro_QuandoEmailForaPadrao()
        {
            // Arrange;
            var model = new UsuarioInput { Email = "teste@teste" };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void DeveRetornarOk_QuandoEmailPadrao()
        {
            // Arrange;
            var model = new UsuarioInput { Email = "teste@gmail.com" };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void DeveRetornarErro_QuandoChamadoNulo()
        {
            // Arrange;
            var model = new UsuarioInput { Chamado = null };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            result.ShouldHaveValidationErrorFor(x => x.Chamado);
        }

        [Fact]
        public void DeveRetornarOk_QuandoChamadoValido()
        {
            // Arrange;
            var model = new UsuarioInput { Chamado = "chamadoTeste" };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            result.ShouldNotHaveValidationErrorFor(x => x.Chamado);
        }
    }
}