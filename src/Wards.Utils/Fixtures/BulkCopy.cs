using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;

namespace Wards.Utils.Fixtures
{
    /// <summary>
    /// Dependência adicional obrigatória: Dependência Microsoft.EntityFrameworkCore.Relational;
    /// </summary>
    public static class BulkCopy
    {
        private const int timeOutSegundosPadrao = 180;

        /// <summary>
        /// Diferentemente dos dois métodos que podem receber diretamente um SqlConnection ou MySqlConnection,
        /// este método recebe um _context e valida se o Bulk será realizado para SQL Server ou MySQL;
        /// </summary>
        public static async Task BulkInsert<T, TContext>(List<T> queryLINQ, TContext context, string nomeTabelaDestino, int? timeOutSegundos = timeOutSegundosPadrao) where TContext : DbContext
        {
            if (context is null)
            {
                throw new Exception("O parâmetro de conexão não deve ser nulo");
            }

            DbConnection con = context.Database.GetDbConnection();

            if (con is SqlConnection)
            {
                await BulkInsert(queryLINQ, con as SqlConnection, nomeTabelaDestino, timeOutSegundos);
            }
            else if (con is MySqlConnection)
            {
                await BulkInsert(queryLINQ, con as MySqlConnection, nomeTabelaDestino, timeOutSegundos);
            }
            else
            {
                throw new Exception($"O parâmetro de conexão deve ser do tipo 'SqlConnection' ou 'MySqlConnection'. Tipo atual: {con.GetType()}");
            }
        }

        /// <summary>
        /// Método para SQL Server;
        /// Recebe um resultado LINQ como parâmetro e converte os dados para DataTable e depois realiza o Bulk Insert;
        /// </summary>
        public static async Task BulkInsert<T>(List<T> queryLINQ, SqlConnection? con, string nomeTabelaDestino, int? timeOutSegundos = timeOutSegundosPadrao)
        {
            if (con is null)
            {
                throw new Exception("O parâmetro de conexão não deve ser nulo");
            }

            SqlBulkCopy sqlBulk = new(con)
            {
                DestinationTableName = nomeTabelaDestino
            };

            DataTable dataTable = ConverterListaParaDataTable(queryLINQ, sqlBulk);

            try
            {
                await con.OpenAsync();
                sqlBulk.BulkCopyTimeout = timeOutSegundos ?? timeOutSegundosPadrao;
                sqlBulk.BatchSize = 5000;
                await sqlBulk.WriteToServerAsync(dataTable);

                await con.CloseAsync();
                dataTable.Clear();
            }
            catch (Exception ex)
            {
                throw new Exception($"Houve uma falha interna ao salvar os dados no banco de dados. Mais informações: {ex.Message}");
            }
        }

        /// <summary>
        /// Método para MySQL;
        /// Recebe um resultado LINQ como parâmetro e converte os dados para DataTable e depois realiza o Bulk Insert;
        /// </summary>
        public static async Task BulkInsert<T>(List<T> queryLINQ, MySqlConnection? con, string nomeTabelaDestino, int? timeOutSegundos = timeOutSegundosPadrao)
        {
            if (con is null)
            {
                throw new Exception("O parâmetro de conexão não deve ser nulo");
            }

            MySqlBulkCopy sqlBulk = new(con)
            {
                DestinationTableName = nomeTabelaDestino
            };

            DataTable dataTable = ConverterListaParaDataTable(queryLINQ, null);

            try
            {
                await con.OpenAsync();
                sqlBulk.BulkCopyTimeout = timeOutSegundos ?? timeOutSegundosPadrao;
                await sqlBulk.WriteToServerAsync(dataTable);

                await con.CloseAsync();
                dataTable.Clear();
            }
            catch (Exception ex)
            {
                throw new Exception($"Houve uma falha interna ao salvar os dados no banco de dados. Mais informações: {ex.Message}");
            }
        }

        #region metodos_extras;
        private static DataTable ConverterListaParaDataTable<T>(List<T> queryLINQ, SqlBulkCopy? sqlBulk)
        {
            try
            {
                DataTable dataTable = new(typeof(T).Name);
                PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                List<PropertyInfo> listaTipos = new();

                MapearColunas(sqlBulk, dataTable, props, listaTipos);
                AdicionarValores(queryLINQ, dataTable, listaTipos);

                return dataTable;
            }
            catch (Exception ex)
            {
                throw new Exception($"Houve uma falha interna ao converter os dados para uma tabela em memória. Mais informações: {ex.Message}");
            }
        }

        private static void MapearColunas(SqlBulkCopy? sqlBulk, DataTable dataTable, PropertyInfo[] props, List<PropertyInfo> listaTipos)
        {
            try
            {
                foreach (PropertyInfo prop in props)
                {
                    Type? type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);

                    if (!IsForeignKey(prop))
                    {
                        sqlBulk?.ColumnMappings.Add(prop.Name, prop.Name); // Apenas para SQL Server;
                        dataTable.Columns.Add(prop.Name, type!);

                        listaTipos.Add(prop);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Houve uma falha interna ao mapear as colunas da tabela em memória. Mais informações: {ex.Message}");
            }
        }

        private static void AdicionarValores<T>(List<T> queryLINQ, DataTable dataTable, List<PropertyInfo> listaTipos)
        {
            try
            {
                foreach (T item in queryLINQ)
                {
                    var values = new object[dataTable.Columns.Count];

                    for (int i = 0; i < values.Length; i++)
                    {
                        if (!IsForeignKey(listaTipos[i]))
                        {
                            values[i] = listaTipos[i].GetValue(item, null)!;
                        }
                    }

                    dataTable.Rows.Add(values);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Houve uma falha interna ao atribuir valores à tabela em memória. Mais informações: {ex.Message}");
            }
        }

        private static bool IsForeignKey(PropertyInfo property)
        {
            try
            {
                var propertyType = property.PropertyType;
                var isCollection = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ICollection<>);
                var isClass = propertyType.IsClass && propertyType != typeof(string);

                return isCollection || isClass;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion;
    }
}