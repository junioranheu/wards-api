﻿using AutoMapper;
using Wards.Application.UseCases.Usuarios.ObterUsuarioCondicaoArbitraria.Queries;
using Wards.Application.UseCases.Usuarios.Shared.Output;

namespace Wards.Application.UseCases.Usuarios.ObterUsuarioCondicaoArbitraria
{
    public sealed class ObterUsuarioCondicaoArbitrariaUseCase : IObterUsuarioCondicaoArbitrariaUseCase
    {
        private readonly IMapper _map;
        private readonly IObterUsuarioCondicaoArbitrariaQuery _obterQuery;

        public ObterUsuarioCondicaoArbitrariaUseCase(IMapper map, IObterUsuarioCondicaoArbitrariaQuery obterQuery)
        {
            _map = map;
            _obterQuery = obterQuery;
        }

        public async Task<(UsuarioOutput? usuario, string senha)> Execute(string login)
        {
            var (usuario, senha) = await _obterQuery.Execute(login);
            return (_map.Map<UsuarioOutput>(usuario), senha);
        }
    }
}