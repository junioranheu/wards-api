using AutoMapper;
using Wards.Application.UseCases.Tokens.CriarRefreshToken;
using Wards.Application.UseCases.Usuarios.ObterUsuarioCondicaoArbitraria;
using Wards.Application.UseCases.Usuarios.Shared.Input;
using Wards.Application.UseCases.Usuarios.Shared.Output;
using Wards.Domain.Enums;
using Wards.Infrastructure.Auth.Token;
using static Wards.Utils.Common;

namespace Wards.Application.UseCases.Usuarios.AutenticarUsuario
{
    public sealed class AutenticarUsuarioUseCase : BaseUsuario, IAutenticarUsuarioUseCase
    {
        private readonly IMapper _map;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IObterUsuarioCondicaoArbitrariaUseCase _obterUsuarioCondicaoArbitrariaUseCase;
        private readonly ICriarRefreshTokenUseCase _criarRefreshTokenUseCase;

        public AutenticarUsuarioUseCase(
            IMapper map,
            IJwtTokenGenerator jwtTokenGenerator,
            IObterUsuarioCondicaoArbitrariaUseCase obterUsuarioCondicaoArbitrariaUseCase,
            ICriarRefreshTokenUseCase criarRefreshTokenUseCase)
        {
            _map = map;
            _jwtTokenGenerator = jwtTokenGenerator;
            _obterUsuarioCondicaoArbitrariaUseCase = obterUsuarioCondicaoArbitrariaUseCase;
            _criarRefreshTokenUseCase = criarRefreshTokenUseCase;
        }

        public async Task<AutenticarUsuarioOutput> Execute(AutenticarUsuarioInput input)
        {
            var (usuario, senha) = await _obterUsuarioCondicaoArbitrariaUseCase.Execute(input?.Login ?? string.Empty);
            AutenticarUsuarioOutput? output = _map.Map<AutenticarUsuarioOutput>(usuario);
            string senhaCriptografada = senha;

            if (output is null)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.UsuarioNaoEncontrado));
            }

            if (!VerificarCriptografia(senha: input?.Senha ?? string.Empty, senhaCriptografada: senhaCriptografada))
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.UsuarioSenhaIncorretos));
            }

            if (!output.IsAtivo)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.ContaDesativada));
            }

            output!.Token = _jwtTokenGenerator.GerarToken(nomeCompleto: output.NomeCompleto!, email: output.Email!, listaClaims: null);
            output = await GerarRefreshToken(_jwtTokenGenerator, _criarRefreshTokenUseCase, output, output.UsuarioId);

            return output;
        }
    }
}