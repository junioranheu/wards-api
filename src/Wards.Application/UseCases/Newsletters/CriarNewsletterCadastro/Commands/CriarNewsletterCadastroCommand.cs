using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Domain.Enums;
using Wards.Infrastructure.Data;
using static Wards.Utils.Fixtures.Get;

namespace Wards.Application.UseCases.NewslettersCadastros.CriarNewsletterCadastro.Commands
{
    public sealed class CriarNewsletterCadastroCommand : ICriarNewsletterCadastroCommand
    {
        private readonly WardsContext _context;

        public CriarNewsletterCadastroCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task<int> Execute(NewsletterCadastro input)
        {
            input.Email = input.Email.ToLowerInvariant();
            bool isCadastrado = await _context.NewslettersCadastros.AnyAsync(n => n.Email == input.Email);

            if (isCadastrado)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NewsletterEmailJaCadastrado));
            }

            await _context.AddAsync(input);
            await _context.SaveChangesAsync();

            return input.NewsletterCadastroId;
        }
    }
}