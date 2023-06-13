using MySqlConnector;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Wards.Utils.Fixtures
{
    public static class BulkCopy
    {
        /// <summary>
        /// Método para SQL Server;
        /// Recebe um resultado de uma query (LINQ) como parâmetro, 
        /// converte os dados para DataTable e depois realiza o Bulkinsert;
        /// </summary>
        public static async Task ConverterLINQQueryParaDataTableParaBulkInsert<T>(List<T> queryLINQ, string nomeTabelaDestino, SqlConnection connection)
        {
            SqlBulkCopy sqlBulk = new(connection)
            {
                DestinationTableName = nomeTabelaDestino
            };

            DataTable dataTable = ConverterListaParaDataTable(queryLINQ, sqlBulk);

            await connection.OpenAsync();
            sqlBulk.BulkCopyTimeout = 180;
            sqlBulk.BatchSize = 5000;
            await sqlBulk.WriteToServerAsync(dataTable);
            await connection.CloseAsync();
            dataTable.Clear();
        }

        /// <summary>
        /// Método para MySQL;
        /// Recebe um resultado de uma query (LINQ) como parâmetro, 
        /// converte os dados para DataTable e depois realiza o Bulkinsert;
        /// </summary>
        public static async Task ConverterLINQQueryParaDataTableParaBulkInsert<T>(List<T> queryLINQ, string nomeTabelaDestino, MySqlConnection connection)
        {
            MySqlBulkCopy sqlBulk = new(connection)
            {
                DestinationTableName = nomeTabelaDestino
            };

            DataTable dataTable = ConverterListaParaDataTable(queryLINQ, null);

            await connection.OpenAsync();
            sqlBulk.BulkCopyTimeout = 180;
            await sqlBulk.WriteToServerAsync(dataTable);
            await connection.CloseAsync();
            dataTable.Clear();
        }

        #region metodos_extras;
        private static DataTable ConverterListaParaDataTable<T>(List<T> queryLINQ, SqlBulkCopy? sqlBulk)
        {
            DataTable dataTable = new(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<PropertyInfo> listaTipos = new();

            MapearColunas(sqlBulk, dataTable, props, listaTipos);
            AdicionarValores(queryLINQ, dataTable, listaTipos);

            return dataTable;
        }

        private static void MapearColunas(SqlBulkCopy? sqlBulk, DataTable dataTable, PropertyInfo[] props, List<PropertyInfo> listaTipos)
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

        private static void AdicionarValores<T>(List<T> queryLINQ, DataTable dataTable, List<PropertyInfo> listaTipos)
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

        private static bool IsForeignKey(PropertyInfo property)
        {
            var propertyType = property.PropertyType;
            var isCollection = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ICollection<>);
            var isClass = propertyType.IsClass && propertyType != typeof(string);

            return isCollection || isClass;
        }
        #endregion;
    }
}