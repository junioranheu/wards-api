using AutoMapper;
using Wards.Application.UseCases.CriarFeriados.AtualizarFeriado;
using Wards.Application.UseCases.Feriados.AtualizarFeriado.Commands;
using Wards.Application.UsesCases.Feriados.Shared.Models.Input;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Feriados.AtualizarFeriado
{
    public sealed class AtualizarFeriadoUseCase : IAtualizarFeriadoUseCase
    {
        private readonly IMapper _map;
        private readonly IAtualizarFeriadoCommand _atualizarFeriadoCommand;

        public AtualizarFeriadoUseCase(IMapper map, IAtualizarFeriadoCommand atualizarFeriadoCommand)
        {
            _map = map;
            _atualizarFeriadoCommand = atualizarFeriadoCommand;
        }

        public async Task<int> Execute(FeriadoInput input)
        {
            return await _atualizarFeriadoCommand.Execute(_map.Map<Feriado>(input));
        }
    }
}