﻿using Wards.Application.UsesCases.Tokens.ObterRefreshToken.Queries;

namespace Wards.Application.UsesCases.Tokens.ObterRefreshToken
{
    public sealed class ObterRefreshTokenUseCase : IObterRefreshTokenUseCase
    {
        public readonly IObterRefreshTokenUseCase _obterQuery;

        public ObterRefreshTokenUseCase(IObterRefreshTokenUseCase obterQuery)
        {
            _obterQuery = obterQuery;
        }

        public async Task<string> ObterByUsuarioId(int id)
        {
            return await _obterQuery.ObterByUsuarioId(id);
        }
    }
}
