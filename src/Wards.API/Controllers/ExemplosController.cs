using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Wards.Application.Services.Sistemas.ResetarBancoDados;
using Wards.Application.UseCases.Logs.ListarLog;
using Wards.Application.UseCases.Logs.Shared.Output;
using Wards.Application.UseCases.Shared.Models;
using Wards.Application.UseCases.Usuarios.ListarUsuario;
using Wards.Application.UseCases.Usuarios.Shared.Output;
using Wards.Application.UseCases.Wards.BulkCopyCriarWard;
using Wards.Application.UseCases.Wards.Shared.Input;
using Wards.Domain.Entities;
using Wards.Domain.Enums;
using static Wards.Utils.Fixtures.Get;
using static Wards.Utils.Fixtures.Post;

namespace Wards.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExemplosController : Controller
    {
        #region constructor
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IListarLogUseCase _listarLogUseCase;
        private readonly IListarUsuarioUseCase _listarUsuarioUseCase;
        private readonly IMigrateDatabaseService _migrateDatabaseService;
        private readonly IBulkCopyCriarWardUseCase _bulkCopyCriarWardUseCase;

        /// <summary>
        /// Controller para testes e exemplos aleatórios e possivelmente úteis;
        /// </summary>
        public ExemplosController(
            IWebHostEnvironment webHostEnvironment,
            IListarLogUseCase listarLogUseCase,
            IListarUsuarioUseCase listarUsuarioUseCase,
            IMigrateDatabaseService migrateDatabaseService,
            IBulkCopyCriarWardUseCase bulkCopyCriarWardUseCase)
        {
            _webHostEnvironment = webHostEnvironment;
            _listarLogUseCase = listarLogUseCase;
            _listarUsuarioUseCase = listarUsuarioUseCase;
            _migrateDatabaseService = migrateDatabaseService;
            _bulkCopyCriarWardUseCase = bulkCopyCriarWardUseCase;
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
        public async IAsyncEnumerable<dynamic> ExemploStreamingFileEmChunks([EnumeratorCancellation] CancellationToken cancellationToken, string? nomeArquivo = "background.jpg", long? chunkSizeBytes = 1048576)
        {
            if (nomeArquivo is null || chunkSizeBytes is null || chunkSizeBytes < 1)
            {
                throw new Exception("Os parâmetros 'nomeArquivo' e 'chunkSizeBytes' não devem ser nulos");
            }

            string[] listaArquivos = Directory.GetFiles($"{_webHostEnvironment.ContentRootPath}/Assets/Misc/");
            string? arquivo = listaArquivos.Where(x => x.Contains(nomeArquivo)).FirstOrDefault() ?? throw new Exception($"Nenhum arquivo foi encontrado com o nome '{nomeArquivo}'");

            IAsyncEnumerable<(double porcentagemCompleta, byte[] chunk)> streamFileEmChunks = StreamFileEmChunks(arquivo, chunkSizeBytes.GetValueOrDefault(), cancellationToken);

            await foreach (var (porcentagemCompleta, chunk) in streamFileEmChunks)
            {
                dynamic obj = new
                {
                    porcentagemCompleta,
                    chunk
                };

                yield return obj;
            }
        }

        /// <summary>
        /// Exemplo de (simulação) streaming (com yield) de um texto simples;
        /// </summary>
        [HttpGet("exemploSimularStreamingYieldTextoSimples")]
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