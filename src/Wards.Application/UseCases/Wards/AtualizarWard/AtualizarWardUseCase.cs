﻿using AutoMapper;
using Wards.Application.UseCases.Wards.AtualizarWard.Commands;
using Wards.Application.UseCases.Wards.Shared.Input;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Wards.AtualizarWard
{
    public sealed class AtualizarWardUseCase : IAtualizarWardUseCase
    {
        private readonly IMapper _map;
        private readonly IAtualizarWardCommand _atualizarCommand;

        public AtualizarWardUseCase(IMapper map, IAtualizarWardCommand atualizarCommand)
        {
            _map = map;
            _atualizarCommand = atualizarCommand;
        }

        public async Task<int> Execute(WardInput input)
        {
            return await _atualizarCommand.Execute(_map.Map<Ward>(input));
        }
    }
}