﻿using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Auxiliares.ListarEstado.Queries
{
    public interface IListarEstadoQuery
    {
        Task<IEnumerable<Estado>> Execute(PaginacaoInput input);
    }
}