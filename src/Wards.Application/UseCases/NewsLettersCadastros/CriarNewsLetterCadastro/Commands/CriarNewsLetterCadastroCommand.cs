using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.NewsLettersCadastros.CriarNewsLetterCadastro.Commands
{
    public sealed class CriarNewsLetterCadastroCommand : ICriarNewsLetterCadastroCommand
    {
        private readonly WardsContext _context;

        public CriarNewsLetterCadastroCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task<int> Execute(NewsLetterCadastro input)
        {
            await _context.AddAsync(input);
            await _context.SaveChangesAsync();

            return input.NewsLetterCadastroId;
        }
    }
}