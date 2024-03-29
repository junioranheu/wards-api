﻿using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Application.UseCases.Wards.Shared.Output;

namespace Wards.Application.UseCases.Wards.ListarWard
{
    public interface IListarWardUseCase
    {
        Task<IEnumerable<WardOutput>> Execute(PaginacaoInput input, string keyword);
    }
}