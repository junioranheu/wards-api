﻿using AutoMapper;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Shared.Output;

namespace Wards.Application.UsesCases.Auxiliares.ListarEstado
{
    public sealed class ListarEstadoUseCase : IListarEstadoUseCase
    {
        private readonly IMapper _map;
        private readonly IListarEstadoQuery _listarEstadoQuery;

        public ListarEstadoUseCase(IMapper map, IListarEstadoQuery listarEstadoQuery)
        {
            _map = map;
            _listarEstadoQuery = listarEstadoQuery;
        }

        public async Task<IEnumerable<EstadoOutput>> Execute()
        {
            return _map.Map<IEnumerable<EstadoOutput>>(await _listarEstadoQuery.Execute());
        }
    }
}