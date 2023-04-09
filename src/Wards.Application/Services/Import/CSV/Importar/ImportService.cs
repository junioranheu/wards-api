using Microsoft.AspNetCore.Http;
using MySqlConnector;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using Wards.Infrastructure.Factory;

namespace Wards.Application.Services.Import.CSV.Importar
{
    public sealed class ImportService : IImportService
    {
        private readonly IConnectionFactory _connectionFactory;

        public ImportService(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<(DataTable? tabelaErros, bool isErroBanco)> InserirCsv(string nomeDaTabela, object objectType, IFormFile formFile, int justificativaId, bool isVerificarData, List<string>? nomesEquipamentos = default)
        {
            DataTable tabelaInsert = new();
            CriarColunas(objectType, tabelaInsert);

            DataTable tabelaErros = new();
            CriarColunas(objectType, tabelaErros);

            string csvAsString = await LerCsv(formFile);
            int numeroLinha = 0, qtdColunas = 0;
            bool isErroBanco = false;

            PopularDataTable(justificativaId, tabelaInsert, csvAsString, ref numeroLinha, ref qtdColunas);

            ValidarColunas(tabelaInsert, tabelaErros, isVerificarData, nomesEquipamentos);

            if (tabelaErros.Rows.Count == 0)
            {
                isErroBanco = await CsvToSql(tabelaInsert, nomeDaTabela, objectType);
            }

            return (tabelaErros, isErroBanco);
        }

        private static void CriarColunas(object objectType, DataTable tabela)
        {
            foreach (var prop in objectType.GetType().GetProperties())
            {
                if (!prop.Name.Contains($"{objectType.GetType().Name}Id") && prop.Name != "Justificativas")
                {
                    tabela.Columns.Add(prop.Name);
                }
            }
        }

        private static async Task<string> LerCsv(IFormFile formFile)
        {
            string csvAsString;
            using (var stream = formFile.OpenReadStream())
            using (var reader = new StreamReader(stream))
            {
                csvAsString = await reader.ReadToEndAsync();
            }

            return csvAsString;
        }

        private static void PopularDataTable(int justificativaId, DataTable tabelaInsert, string csvAsString, ref int numeroLinha, ref int qtdColunas)
        {
            foreach (string linhaCsv in csvAsString.Split('\n'))
            {
                if (!string.IsNullOrEmpty(linhaCsv) && numeroLinha != 0)
                {
                    tabelaInsert.Rows.Add();
                    int numeroColuna = 0;

                    if (qtdColunas == 0)
                    {
                        qtdColunas = linhaCsv.ToCharArray().Count(c => c == ';');
                    }

                    foreach (string fileRec in linhaCsv.Split(';'))
                    {
                        try
                        {
                            string dataToAdd = !string.IsNullOrEmpty(fileRec) ? fileRec : "0";

                            tabelaInsert.Rows[tabelaInsert.Rows.Count - 1][numeroColuna] = string.Join(string.Empty, Regex.Split(dataToAdd, @"(?:\r\n|\n|\r)"));
                            numeroColuna++;

                            qtdColunas = InserirJustificativaIdSeUltimaColuna(justificativaId, tabelaInsert, qtdColunas, numeroColuna);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }

                numeroLinha++;
            }
        }

        private static void ValidarColunas(DataTable tabelaInsert, DataTable tabelaErros, bool isVerificarData, List<string>? nomesEquipamentos)
        {
            foreach (var row in tabelaInsert.Select())
            {
                bool isLinhaValida = false;

                if (isVerificarData)
                {
                    isLinhaValida = ValidarDataHora(data: row["Data"].ToString()!, hora: row["Hora"].ToString()!);
                }

                if (isLinhaValida && nomesEquipamentos?.Count > 0)
                {
                    isLinhaValida = ValidarNome(row["Nome"].ToString()!, nomesEquipamentos);
                }

                // ============>>>>> É NECESSÁRIO REMOVER ESSE "isLinhaValida = true" PARA FUNCIONAR AS VALIDAÇÕES <<<<<============
                isLinhaValida = true;

                if (!isLinhaValida)
                {
                    tabelaErros.Rows.Add(row.ItemArray);
                }
            }

            try
            {
                tabelaErros.Columns.Remove("JustificativaId");
            }
            catch (Exception)
            {

            }
        }

        private static int InserirJustificativaIdSeUltimaColuna(int justificativaId, DataTable tabelaInsert, int qtdColunas, int numeroColuna)
        {
            if (numeroColuna > qtdColunas)
            {
                tabelaInsert.Rows[tabelaInsert.Rows.Count - 1][numeroColuna] = justificativaId;
                qtdColunas = 0;
            }

            return qtdColunas;
        }

        private async Task<bool> CsvToSql(DataTable tabelaInsert, string nomeTabela, object objectType)
        {
            bool isErroBanco = false;

            try
            {
                var connection = _connectionFactory.CreateDbSqlConnection();

                // SqlBulkCopy sqlBulk = new(connection)
                MySqlBulkCopy sqlBulk = new(connection)
                {
                    DestinationTableName = nomeTabela
                };

                MapearTabelaBanco(objectType, sqlBulk);

                connection.Open();
                sqlBulk.BulkCopyTimeout = 180;
                // sqlBulk.BatchSize = 5000;
                await sqlBulk.WriteToServerAsync(tabelaInsert);
                connection.Close();
            }
            catch (Exception)
            {
                isErroBanco = true;
            }

            return isErroBanco;
        }

        // private static void MapearTabelaBanco(object objectType, SqlBulkCopy sqlBulk)
        private static void MapearTabelaBanco(object objectType, MySqlBulkCopy sqlBulk)
        {
            int i = 0;

            foreach (var prop in objectType.GetType().GetProperties())
            {
                if (!prop.Name.Contains($"{objectType.GetType().Name}Id") && prop.Name != "Justificativas")
                {
                    // sqlBulk.ColumnMappings.Add(prop.Name, prop.Name);
                    sqlBulk.ColumnMappings.Add(new MySqlBulkCopyColumnMapping(i, prop.Name));
                    i++;
                }
            }
        }

        private static bool ValidarDataHora(string data, string hora)
        {
            bool isValid = DateTime.TryParseExact(
                           data,
                           "dd/MM/yyyy",
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None,
                           out DateTime d);

            if (isValid)
            {
                int[] listaHoras = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1100, 1200, 1300, 1400, 1500, 1600, 1700, 1800, 1900, 2000, 2100, 2200, 2300 };
                isValid = listaHoras.Any(p => p == int.Parse(hora));
            }

            return isValid;
        }

        private static bool ValidarNome(string nome, List<string> nomesEquipamentos)
        {
            return nomesEquipamentos.Contains(nome);
        }
    }
}