﻿using AutoMapper;
using Wards.Application.UsesCases.Tokens.CriarRefreshToken;
using Wards.Application.UsesCases.Usuarios.ObterUsuarioCondicaoArbitraria;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Output;
using Wards.Domain.Enums;
using Wards.Infrastructure.Auth.Token;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Usuarios.AutenticarUsuario
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
            // #1 - Buscar usuário e sua senha (para não expor no output);
            (UsuarioOutput?, string) resp = await _obterUsuarioCondicaoArbitrariaUseCase.Execute(input?.Login ?? string.Empty);
            AutenticarUsuarioOutput? output = _map.Map<AutenticarUsuarioOutput>(resp.Item1);
            string senhaCriptografada = resp.Item2;

            if (output is null)
                return (new AutenticarUsuarioOutput() { Messages = new string[] { GetDescricaoEnum(CodigoErroEnum.UsuarioNaoEncontrado) } });

            // #2 - Verificar se a senha está correta;
            if (!VerificarCriptografia(senha: input?.Senha ?? string.Empty, senhaCriptografada: senhaCriptografada))
                return (new AutenticarUsuarioOutput() { Messages = new string[] { GetDescricaoEnum(CodigoErroEnum.UsuarioSenhaIncorretos) } });

            // #3 - Verificar se o usuário está ativo;
            if (!output.IsAtivo)
                return (new AutenticarUsuarioOutput() { Messages = new string[] { GetDescricaoEnum(CodigoErroEnum.ContaDesativada) } });

            // #4 - Criar token JWT;
            output!.Token = _jwtTokenGenerator.GerarToken(nomeCompleto: output.NomeCompleto!, email: output.Email!, listaClaims: null);

            // #5 - Gerar refresh token;
            output = await GerarRefreshToken(_jwtTokenGenerator, _criarRefreshTokenUseCase, output, output.UsuarioId);

            return output;
        }
    }
}