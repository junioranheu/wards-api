using Wards.Application.UseCases.Wards.ObterWard.Queries;
using Wards.Domain.Entities;
using Wards.Domain.Enums;
using Wards.Infrastructure.Data;
using static Wards.Utils.Fixtures.Get;

namespace Wards.Application.UseCases.Wards.AtualizarWard.Commands
{
    public sealed class AtualizarWardCommand : IAtualizarWardCommand
    {
        private readonly WardsContext _context;
        private readonly IObterWardQuery _obterQuery;

        public AtualizarWardCommand(WardsContext context, IObterWardQuery obterQuery)
        {
            _context = context;
            _obterQuery = obterQuery;
        }

        public async Task<int> Execute(Ward input)
        {
            var item = await _obterQuery.Execute(input.WardId);

            if (item is null)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            item.Titulo = !string.IsNullOrEmpty(input.Titulo) ? input.Titulo : item.Titulo;
            item.ImagemPrincipalBlob = input.ImagemPrincipalBlob?.Length > 0 ? input.ImagemPrincipalBlob : item.ImagemPrincipalBlob;
            item.Conteudo = !string.IsNullOrEmpty(input.Conteudo) ? input.Conteudo : item.Conteudo;
            item.UsuarioModId = input.UsuarioModId;
            item.DataMod = GerarHorarioBrasilia();
            item.IsAtivo = input.IsAtivo;

            _context.Update(item);
            await _context.SaveChangesAsync();

            return input.WardId;
        }
    }
}