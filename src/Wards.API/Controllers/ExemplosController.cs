﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Wards.Application.Services.Sistemas.ResetarBancoDados;
using Wards.Application.UseCases.Logs.ListarLog;
using Wards.Application.UseCases.Logs.Shared.Output;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Application.UseCases.Shared.Models.Output;
using Wards.Application.UseCases.Usuarios.ListarUsuario;
using Wards.Application.UseCases.Usuarios.Shared.Output;
using Wards.Application.UseCases.Wards.BulkCopyCriarWard;
using Wards.Application.UseCases.Wards.Shared.Input;
using Wards.Application.UseCases.Wards.Shared.Output;
using Wards.Domain.Consts;
using Wards.Domain.Entities;
using Wards.Domain.Enums;
using Wards.Infrastructure.Factory.ConnectionFactory;
using Wards.Utils.Entities.Output;
using static Wards.Utils.Fixtures.Convert;
using static Wards.Utils.Fixtures.Get;
using static Wards.Utils.Fixtures.Post;

namespace Wards.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExemplosController : BaseController<ExemplosController>
    {
        #region constructor
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IListarLogUseCase _listarLogUseCase;
        private readonly IListarUsuarioUseCase _listarUsuarioUseCase;
        private readonly IMigrateDatabaseService _migrateDatabaseService;
        private readonly IBulkCopyCriarWardUseCase _bulkCopyCriarWardUseCase;

        /// <summary>
        /// Controller para testes e exemplos aleatórios e possivelmente úteis;
        /// </summary>
        public ExemplosController(
            IWebHostEnvironment webHostEnvironment,
            IConnectionFactory connectionFactory,
            IListarLogUseCase listarLogUseCase,
            IListarUsuarioUseCase listarUsuarioUseCase,
            IMigrateDatabaseService migrateDatabaseService,
            IBulkCopyCriarWardUseCase bulkCopyCriarWardUseCase)
        {
            _webHostEnvironment = webHostEnvironment;
            _connectionFactory = connectionFactory;
            _listarLogUseCase = listarLogUseCase;
            _listarUsuarioUseCase = listarUsuarioUseCase;
            _migrateDatabaseService = migrateDatabaseService;
            _bulkCopyCriarWardUseCase = bulkCopyCriarWardUseCase;
        }
        #endregion

        #region cancellationToken
        /// <summary>
        /// Exemplo de requisição com cancellation token;
        /// </summary>
        [HttpGet("exemploCancellationToken")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<ActionResult<string>> ExemploCancellationToken(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(5000, cancellationToken); // Simular requisição longa;
                return "Processo finalizado com sucesso";
            }
            catch (OperationCanceledException)
            {
                return "Processo cancelado pelo usuário (CancellationToken)";
            }
        }
        #endregion

        #region parallel_threads
        /// <summary>
        /// Exemplo de execução de threads paralelas com split de uma lista em chunks;
        /// </summary>
        [HttpGet("exemploParallelThread_E_ListaChunks")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public ActionResult<int> ExemploParallelThread_E_ListaChunks()
        {
            List<string> listaStrings = new() { "Naruto", "Sasuke", "Sakura", "Kakashi", "Rock Lee", "Neji", "Ten Ten", "Guy", "Kiba", "Shino", "Hinata", "Kurenai", "Shikamaru", "Chouji", "Ino", "Asuma" };
            List<int> listaTesteLength = new();

            var chunks = SepararListaEmChunks(listaStrings.ToList(), 3);

            Parallel.ForEach(chunks, chunk =>
            {
                ExemploAdicionarLenghtLista(chunk, listaTesteLength);
            });

            return Ok(listaTesteLength.Sum());

            static void ExemploAdicionarLenghtLista(List<string> listaStrings, List<int> listaTesteLength)
            {
                foreach (var item in listaStrings)
                {
                    listaTesteLength.Add(item.Length);
                }
            }
        }
        #endregion

        #region query
        /// <summary>
        /// Busca ao banco sem Entity Framework; utilizando SqlCommand & SqlDataReader, à moda antiga;
        /// Exemplo para MySQL;
        /// </summary>
        [HttpGet("exemploQuerySqlCommand_MySQL")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<WardOutput>))]
        public async Task<ActionResult<List<WardOutput>>> ExemploQuerySqlCommand_MySQL(string parametroNome)
        {
            var connection = _connectionFactory.ObterMySqlConnection();

            MySqlCommand selectCommand = new(@$"SELECT w.WardId, w.Titulo, w.Conteudo, w.Data, u.NomeCompleto
                                              FROM Wards w 
                                              LEFT JOIN Usuarios u ON u.UsuarioId = w.UsuarioId
                                              WHERE 1 = 1
                                              AND u.IsAtivo = 1
                                              AND u.NomeCompleto LIKE CONCAT('%', @parametro1, '%');", connection);

            selectCommand.Parameters.AddWithValue("@parametro1", parametroNome);

            await connection.OpenAsync();

            MySqlDataReader reader = await selectCommand.ExecuteReaderAsync();

            List<WardOutput> listaOutput = new();

            if (!reader.HasRows)
            {
                return listaOutput;
            }

            while (await reader.ReadAsync())
            {
                WardOutput output = new()
                {
                    WardId = NormalizarSqlDataReader<int>(reader["WardId"]),
                    Titulo = NormalizarSqlDataReader<string>(reader["Titulo"]),
                    Conteudo = NormalizarSqlDataReader<string>(reader["Conteudo"])!,
                    Data = NormalizarSqlDataReader<DateTime>(reader["Data"]),
                    Usuarios = new UsuarioOutput()
                    {
                        NomeCompleto = NormalizarSqlDataReader<string>(reader["NomeCompleto"])
                    }
                };

                listaOutput.Add(output);
            }

            await connection.CloseAsync();

            return listaOutput;
        }

        /// <summary>
        /// Busca ao banco sem Entity Framework; utilizando SqlCommand & SqlDataReader, à moda antiga;
        /// Exemplo para SQL Server;
        /// </summary>
        [HttpGet("exemploQuerySqlCommand_SQLServer")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<WardOutput>))]
        public async Task<ActionResult<List<WardOutput>>> ExemploQuerySqlCommand_SQLServer(string parametroNome)
        {
            var connection = _connectionFactory.ObterSqlServerConnection();

            SqlCommand selectCommand = new(@$"SELECT w.WardId, w.Titulo, w.Conteudo, w.Data, u.NomeCompleto
                                              FROM Wards w 
                                              LEFT JOIN Usuarios u ON u.UsuarioId = w.UsuarioId
                                              WHERE 1 = 1
                                              AND u.IsAtivo = 1
                                              AND u.NomeCompleto LIKE CONCAT('%', @parametro1, '%');", connection);

            selectCommand.Parameters.AddWithValue("@parametro1", parametroNome);

            await connection.OpenAsync();

            SqlDataReader reader = await selectCommand.ExecuteReaderAsync();

            List<WardOutput> listaOutput = new();

            if (!reader.HasRows)
            {
                return listaOutput;
            }

            while (await reader.ReadAsync())
            {
                WardOutput output = new()
                {
                    WardId = NormalizarSqlDataReader<int>(reader["WardId"]),
                    Titulo = NormalizarSqlDataReader<string>(reader["Titulo"]),
                    Conteudo = NormalizarSqlDataReader<string>(reader["Conteudo"])!,
                    Data = NormalizarSqlDataReader<DateTime>(reader["Data"]),
                    Usuarios = new UsuarioOutput()
                    {
                        NomeCompleto = NormalizarSqlDataReader<string>(reader["NomeCompleto"])
                    }
                };

                listaOutput.Add(output);
            }

            await connection.CloseAsync();

            return listaOutput;
        }
        #endregion

        #region streaming
        /// <summary>
        /// Exemplo de streaming de um arquivo em chunks;
        /// Base64 to .mp4: base64.guru/converter/decode/video;
        /// Base64 to .jpg: onlinejpgtools.com/convert-base64-to-jpg;
        /// </summary>
        [HttpGet("exemploStreamingFileEmChunks")]
        [AllowAnonymous]
        public async IAsyncEnumerable<StreamingFileOutput> ExemploStreamingFileEmChunks([EnumeratorCancellation] CancellationToken cancellationToken, string? nomeArquivo = "background.jpg", long? chunkSizeBytes = 1048576)
        {
            if (nomeArquivo is null || chunkSizeBytes is null || chunkSizeBytes < 1)
            {
                throw new Exception("Os parâmetros 'nomeArquivo' e 'chunkSizeBytes' não devem ser nulos");
            }

            string[] listaArquivos = Directory.GetFiles($"{_webHostEnvironment.ContentRootPath}/Assets/Misc/");
            string? arquivo = listaArquivos.Where(x => x.Contains(nomeArquivo)).FirstOrDefault() ?? throw new Exception($"Nenhum arquivo foi encontrado com o nome '{nomeArquivo}'");

            IAsyncEnumerable<StreamingFileOutput> streamFileEmChunks = StreamFileEmChunks(arquivo, chunkSizeBytes.GetValueOrDefault(), cancellationToken);

            await foreach (StreamingFileOutput chunk in streamFileEmChunks)
            {
                yield return chunk;
            }
        }

        /// <summary>
        /// Exemplo de (simulação) streaming (com yield) de um texto simples;
        /// </summary>
        [HttpGet("exemploSimularStreamingYieldTextoSimples")]
        [ResponseCache(Duration = TemposConst.UmMinutoEmSegundos)]
        [AllowAnonymous]
        public async IAsyncEnumerable<Ward> ExemploSimularStreamingYieldTextoSimples([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            int i = 1;

            while (!cancellationToken.IsCancellationRequested)
            {
                yield return new Ward
                {
                    WardId = i++,
                    Titulo = GerarStringAleatoria(GerarNumeroAleatorio(5, 10), true),
                    Conteudo = GerarStringAleatoria(GerarNumeroAleatorio(10, 20), false),
                    UsuarioId = GerarNumeroAleatorio(1, 999)
                };

                await Task.Delay(1000, cancellationToken);
            }
        }
        #endregion

        #region bulk_copy
        /// <summary>
        /// Exemplo de insert com BulkCopy e DataTable;
        /// </summary>
        [HttpGet("exemploBulkCopy")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<ActionResult<List<Ward>>> ExemploBulkCopy()
        {
            List<WardInput> listaWards = new();

            for (int i = 0; i < GerarNumeroAleatorio(1000, 2000); i++)
            {
                WardInput ward = new()
                {
                    Titulo = GerarStringAleatoria(5, true),
                    Conteudo = GerarStringAleatoria(20, false),
                    UsuarioId = 1
                };

                listaWards.Add(ward);
            }

            await _bulkCopyCriarWardUseCase.Execute(listaWards);

            return Ok(true);
        }
        #endregion

        #region linq
        /// <summary>
        /// Exemplo de LINQ avançado, fazendo select com group by e inserir resultado em um Output;
        /// </summary>
        [HttpGet("exemploUnificarListasComSyntaxQuery")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LogAgrupadoOutput>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(LogOutput))]
        public async Task<ActionResult<List<LogAgrupadoOutput>>> ExemploUnificarListasComSyntaxQuery()
        {
            var lista = await _listarLogUseCase.Execute(new PaginacaoInput() { IsSelectAll = true });

            if (!lista.Any())
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            // Simular duas listas distintas (mals pelo exemplo bobo de par e impar);
            var listaPares = lista.Where(x => x.LogId % 2 == 0).ToList();
            var listaImpares = lista.Where(x => x.LogId % 2 != 0).ToList();

            // Unificar listas e criar response;
            var queryUnificado = from lista1 in listaPares
                                 join lista2 in listaImpares

                                 on new
                                 {
                                     Ano = lista1.Data.Year,
                                     Mes = lista1.Data.Month,
                                     Dia = lista1.Data.Day,
                                     Hora = lista1.Data.Hour,
                                     // Distribuidora = listaDistribuidoraId.Any(x => x == medicao.DistribuidoraId) // Exemplo real TRETA;
                                 }
                                 equals new
                                 {
                                     Ano = lista2.Data.Year,
                                     Mes = lista2.Data.Month,
                                     Dia = lista2.Data.Day,
                                     Hora = lista2.Data.Hour,
                                     // Distribuidora = listaDistribuidoraId.Any(y => listaDistribuidoraId.Any(x => x == y)) // Exemplo real TRETA;
                                 }

                                 into lista_join
                                 from lista_join_nullable in lista_join.DefaultIfEmpty()

                                 select new ExemploUnificarListasComSyntaxQuery_ExemploController_Output
                                 {
                                     Log = lista1,
                                     Endpointjoin = lista_join_nullable?.Endpoint ?? string.Empty,
                                     Exemplo1 = 0,
                                     Exemplo2 = $"Fino eh {lista1.TipoRequisicao}",
                                     Exemplo3 = lista1.StatusResposta == 200
                                 };

            List<ExemploUnificarListasComSyntaxQuery_ExemploController_Output> queryUnificado_list = queryUnificado.ToList();

            return Ok(queryUnificado_list);
        }

        /// <summary>
        /// Exemplo de LINQ avançado, fazendo select com group by e inserir resultado em um Output;
        /// </summary>
        [HttpGet("exemploLINQGroupBy")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LogAgrupadoOutput>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(LogOutput))]
        public async Task<ActionResult<List<LogAgrupadoOutput>>> ExemploLINQGroupBy()
        {
            var lista = await _listarLogUseCase.Execute(new PaginacaoInput() { IsSelectAll = true });

            if (!lista.Any())
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            List<LogAgrupadoOutput> linq = lista.
                                           // Include(x => x.XXX).
                                           GroupBy(x => new
                                           {
                                               x.Data.Year,
                                               x.Data.Month,
                                               x.Data.Day,
                                               x.UsuarioId
                                           }).
                                           Select(x => new LogAgrupadoOutput
                                           {
                                               Data = x.Select(x => x.Data).FirstOrDefault(),
                                               DataStr = $"Dia {x.Select(x => x.Data.Day).FirstOrDefault()} de {DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(x.Select(x => x.Data.Month).FirstOrDefault())} de {x.Select(x => x.Data.Year).FirstOrDefault()}",
                                               QtdLogs = x.Select(x => x.LogId).Count(),
                                               UsuarioId = x.Select(x => x.UsuarioId).FirstOrDefault(),
                                               UsuarioNome = x.Select(x => x.Usuarios?.NomeCompleto).FirstOrDefault() ?? "-",
                                               ListaStatusResposta = x.Select(x => x.StatusResposta).ToArray()
                                           }).
                                           // Skip((input.IsSelectAll ? 0 : input.Index * input.Limit)).
                                           // Take((input.IsSelectAll ? int.MaxValue : input.Limit)).
                                           // AsNoTracking().ToListAsync();
                                           ToList();

            return Ok(linq);
        }

        /// <summary>
        /// Exemplo de LINQ avançado, fazendo select em uma propriedade dinâmica;
        /// </summary>
        [HttpGet("exemploLINQComSelectDinamico")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(string))]
        public async Task<ActionResult<string>> ExemploLINQComSelectDinamico(string nomePropriedade)
        {
            // Ward: caso seja necessário passar como parâmetro "nomePropriedade" uma classe...
            // Na chamada do end-point/método, pode ser utilizado o "nameof(xxx)";
            if (string.IsNullOrEmpty(nomePropriedade))
            {
                return StatusCode(StatusCodes.Status403Forbidden, string.Empty);
            }

            IEnumerable<UsuarioOutput>? lista = await _listarUsuarioUseCase.Execute(new PaginacaoInput() { IsSelectAll = true });

            object? objeto;

            try
            {
                objeto = lista!.
                         Where(x => x.NomeCompleto!.Contains("Junior")).
                         Select(x => x.GetType().GetProperty(nomePropriedade)!.GetValue(x)).
                         FirstOrDefault();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"A propriedade {nomePropriedade} não existe");
            }

            if (objeto is null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, string.Empty);
            }

            // double valor = objeto is IConvertible ? ((IConvertible)objeto).ToDouble(null) : 0d; // Caso seja necessário converter resultado para double;
            string valor = objeto is IConvertible convertible ? convertible.ToString(null) : "";

            // >>>>>>>>>>>>>>>>>>>>>>>>>>>>> Segunda forma de obter valor dinamicamente (com reflection);
            UsuarioOutput exemplo = lista!.FirstOrDefault()!;
            PropertyInfo? reflection = exemplo!.GetType().GetProperty(nomePropriedade);
            object? valor_forma_2 = reflection!.GetValue(exemplo, null);

            return valor;
        }

        /// <summary>
        /// Exemplos de LINQs avançados, filtrando dados de sub-listas por uma string;
        /// </summary>
        [HttpGet("exemploLINQFiltrarDadosDeSubListaPorString")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Feriado>))]
        public ActionResult<List<Feriado>> ExemploLINQFiltrarDadosDeSubListaPorString(string estadoExclusao)
        {
            if (string.IsNullOrEmpty(estadoExclusao))
            {
                return new List<Feriado>();
            }

            List<Feriado> listaPrincipal = new()
            {
                new Feriado() {
                    Nome = "Feriado do Junior",

                    FeriadosEstados = new List<FeriadoEstado>()
                    {
                        new FeriadoEstado() { Estados = new Estado() { Nome = "São Paulo" } },
                        new FeriadoEstado() { Estados = new Estado() { Nome = "Rio de Janeiro" } }
                    }
                },

                new Feriado() {
                    Nome = "Feriado da Mariana",

                    FeriadosEstados = new List<FeriadoEstado>()
                    {
                        new FeriadoEstado() { Estados = new Estado() { Nome = "Minas Gerais" } },
                        new FeriadoEstado() { Estados = new Estado() { Nome = "São Paulo" } }
                    }
                },
            };

            listaPrincipal = listaPrincipal.
                             Where(lp => !lp.FeriadosEstados!.Any(x => x.Estados!.Nome == estadoExclusao)).
                             ToList();

            return Ok(listaPrincipal);
        }

        /// <summary>
        /// Exemplos de LINQs avançados, filtrando dados de sub-listas por outras listas de datas;
        /// </summary>
        [HttpGet("exemploLINQFiltrarDadosDeSubListaPorOutrasListasDeDatas")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DateTime>))]
        public ActionResult<List<Usuario>> ExemploLINQFiltrarDadosDeSubListaPorOutrasListasDeDatas()
        {
            #region exemplo_codigo_real
            /// <example>
            /// else if (tipoLinqFeriado == TipoLinqFeriadoEnum.FeriadoMovel)
            /// {
            ///    IEnumerable<FeriadoData> listFd = verificarFeriado!.FeriadosDatas!;

            ///    var listaDatasParaSeremExcluidas = listaDatasExclusao!.
            ///                                       Select(x => new { x.MesAnoTratado.Month, x.MesAnoTratado.Year }).ToList().
            ///                                       Intersect(listFd.Select(y => new { y.Data.Month, y.Data.Year })).ToList();

            ///    linqMedicoesCargas = linqMedicoesCargas.
            ///                         Where(mc => (listaDatasParaSeremExcluidas!.Count > 0 ? !listaDatasParaSeremExcluidas.Any(x => x.Month == mc.DataHoraMedicao.Month && x.Year == mc.DataHoraMedicao.Year) : true)).
            ///                         ToList();
            ///}
            /// </example>
            #endregion

            List<Usuario> listaPrincipal = new()
            {
                new Usuario() { Data = new DateTime(2023, 12, 25) },
                new Usuario() { Data = new DateTime(2022, 12, 23) },
                new Usuario() { Data = new DateTime(1997, 03, 25) },
                new Usuario() { Data = new DateTime(2015, 12, 20) }
            };

            List<FeriadoData> listaDatasQueServiraoDeBaseParaFiltragemPosterior = new()
            {
                new FeriadoData() { Data = new DateTime(2022, 12, 22) },
                new FeriadoData() { Data = new DateTime(1975, 01, 17) },
            };

            List<FeriadoData> listaQueSeraFiltrada = new()
            {
                new FeriadoData() { Data = new DateTime(2023, 12, 25) },
                new FeriadoData() { Data = new DateTime(2022, 12, 23) },
                new FeriadoData() { Data = new DateTime(1997, 03, 25) },
                new FeriadoData() { Data = new DateTime(2015, 12, 20) }
            };

            // Lista de datas que se repetem com base nas duas listas em referência...
            // Que, por sua vez, serão utilizadas para filtrar os dados da lista principal posteriormente;
            var listaDatasQueSeRepetem = listaDatasQueServiraoDeBaseParaFiltragemPosterior!.
                                         Select(x => new { x.Data.Month, x.Data.Year }).ToList().
                                         Intersect(listaQueSeraFiltrada.Select(y => new { y.Data.Month, y.Data.Year })).ToList();

            // Remover da "listaPrincipal" os itens que se repetem;
            IEnumerable<DateTime> listaPrincipalFinal = listaPrincipal.
                                                        Where(lp => (listaDatasQueSeRepetem!.Count <= 0 || !listaDatasQueSeRepetem.Any(x => x.Month == lp.Data.Month && x.Year == lp.Data.Year))).
                                                        ToList().
                                                        Select(lp => lp.Data);

            return Ok(listaPrincipalFinal);
        }
        #endregion

        #region migrate
        [HttpGet("migrateDatabase")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(bool))]
        public async Task<ActionResult<bool>> MigrateDatabase(int minuto)
        {
            if (GerarHorarioBrasilia().Minute != minuto)
                return StatusCode(StatusCodes.Status403Forbidden, false);

            await _migrateDatabaseService.Execute(isAplicarMigrations: true);

            return Ok($"Update-Database finalizado com sucesso às {GerarHorarioBrasilia()}");
        }
        #endregion
    }
}