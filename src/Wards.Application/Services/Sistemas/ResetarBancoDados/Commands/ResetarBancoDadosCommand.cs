﻿using Microsoft.Extensions.Logging;
using Wards.Infrastructure.Data;
using Wards.Infrastructure.Seed;
using static Wards.Utils.Common;

namespace Wards.Application.Services.Sistemas.ResetarBancoDados.Commands
{
    public sealed class ResetarBancoDadosCommand : IResetarBancoDadosCommand
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public ResetarBancoDadosCommand(WardsContext context, ILogger<ResetarBancoDadosCommand> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Execute()
        {
            try
            {
                await DbInitializer.Initialize(_context);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{detalhes}", DetalhesException(ex.Source));
                throw;
            }
        }
    }
}