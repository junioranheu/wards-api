using AutoMapper;
using Wards.Application.UseCases.CriarFeriados.CriarFeriado;
using Wards.Application.UseCases.Feriados.CriarFeriado.Commands;
using Wards.Application.UsesCases.Feriados.Shared.Models.Input;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Feriados.CriarFeriado
{
    public class CriarFeriadoUseCase : ICriarFeriadoUseCase
    {
        private readonly IMapper _map;
        private readonly ICriarFeriadoCommand _criarFeriadoCommand;

        public CriarFeriadoUseCase(IMapper map, ICriarFeriadoCommand criarFeriadoCommand)
        {
            _map = map;
            _criarFeriadoCommand = criarFeriadoCommand;
        }

        public async Task<int> ExecuteAsync(FeriadoInput input)
        {
            return await _criarFeriadoCommand.ExecuteAsync(_map.Map<Feriado>(input));
        }
    }
}