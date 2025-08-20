using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace Wards.Infrastructure.Interceptors
{
    public class SlowQueryDebugInterceptor : DbCommandInterceptor
    {
        public override async ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            LogIfSlow(command, eventData);
            return await base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            LogIfSlow(command, eventData);
            return base.ReaderExecuted(command, eventData, result);
        }

        #region extras
        private static void LogIfSlow(DbCommand command, CommandExecutedEventData eventData)
        {
#if !DEBUG
        return;
#endif

            int threshold = GetThreshold(command);

            if (eventData.Duration.TotalMilliseconds > threshold)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("DB");
                Console.ResetColor();
                Console.Write($": [{DateTime.Now:HH:mm:ss}]");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($" Query longa detectada, {eventData.Duration.Milliseconds}ms gastos: {command?.CommandText ?? "-"}");
                Console.ResetColor();
            }
        }

        private static int GetThreshold(DbCommand command)
        {
            string sql = command.CommandText.ToUpper();

            if (sql.StartsWith("SELECT"))
            {
                int joinCount = sql.Split("JOIN").Length - 1;

                if (joinCount == 0 && sql.Contains("GROUP BY"))
                {
                    return 125; // Simples com GROUP BY;
                }

                if (joinCount == 1)
                {
                    return 150; // Média, query com apenas 1 JOIN;
                }

                if (joinCount > 1 && joinCount <= 3)
                {
                    return 300; // Pesada (1 a 3 JOIN);
                }

                if (joinCount > 3 || sql.Contains("SUBQUERY"))
                {
                    return 300; // Pesada (mais de 3 JOIN ou com SUBQUERY);
                }

                return 75; // Simples;
            }

            if (sql.StartsWith("UPDATE") || sql.StartsWith("DELETE"))
            {
                if (sql.Contains("WHERE"))
                {
                    return 150;
                }

                return 500; // Sem WHERE ou update/delete grande;
            }

            return 75; // Default;
        }
        #endregion
    }
}