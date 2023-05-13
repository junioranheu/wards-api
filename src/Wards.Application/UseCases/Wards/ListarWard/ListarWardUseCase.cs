﻿using AutoMapper;
using Wards.Application.UseCases.Shared.Models;
using Wards.Application.UseCases.Wards.ListarWard.Queries;
using Wards.Application.UseCases.Wards.Shared.Output;

namespace Wards.Application.UseCases.Wards.ListarWard
{
    public sealed class ListarWardUseCase : IListarWardUseCase
    {
        private readonly IMapper _map;
        private readonly IListarWardQuery _listarQuery;

        public ListarWardUseCase(IMapper map, IListarWardQuery listarQuery)
        {
            _map = map;
            _listarQuery = listarQuery;
        }

        public async Task<IEnumerable<WardOutput>> Execute(PaginacaoInput input)
        {
            return _map.Map<IEnumerable<WardOutput>>(await _listarQuery.Execute(input));
        }
    }
}