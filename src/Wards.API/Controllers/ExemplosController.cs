using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Collections.Concurrent;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using Wards.Application.Services.GenericReadExcel;
using Wards.Application.Services.GenericReadExcel.Models.Input;
using Wards.Application.Services.Sistemas.ResetarBancoDados;
using Wards.Application.UseCases.Logs.ListarLog;
using Wards.Application.UseCases.Logs.Shared.Output;
using Wards.Application.UseCases.NewslettersCadastros.Shared.Output;
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
using Wards.Infrastructure.Data;
using Wards.Infrastructure.UnitOfWork.Generic;
using Wards.Utils.Entities.Output;
using static Wards.Utils.Fixtures.Convert;
using static Wards.Utils.Fixtures.Get;
using static Wards.Utils.Fixtures.Post;

/// <summary>
/// Este Controller está propositalmente uma bagunça!!!;
/// NÃO LEVE EM CONSIDERAÇÃO ESSE CONTROLLER EM GERAL COMO PARÂMETRO DE QUALIDADE: ELE ESTÁ RUIM PROPOSITALMENTE;
/// </summary>
namespace Wards.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExemplosController : BaseController<ExemplosController>
    {
        #region constructor
        private readonly WardsContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly Infrastructure.Factory.ConnectionFactory.IConnectionFactory _connectionFactory;
        private readonly IListarLogUseCase _listarLogUseCase;
        private readonly IListarUsuarioUseCase _listarUsuarioUseCase;
        private readonly IMigrateDatabaseService _migrateDatabaseService;
        private readonly IBulkCopyCriarWardUseCase _bulkCopyCriarWardUseCase;
        private readonly IGenericRepository<Usuario> _genericUsuarioRepository;
        private readonly IConnectionFactory _rabbitMQConectionFactory;
        private readonly IConnection _rabbitMQConnection;
        private readonly IModel _rabbitMQChannel;

        /// <summary>
        /// Controller para testes e exemplos aleatórios e possivelmente úteis;
        /// Sim, este Controller está propositalmente uma bagunça;
        /// </summary>
        public ExemplosController(
            WardsContext context,
            IWebHostEnvironment webHostEnvironment,
            Infrastructure.Factory.ConnectionFactory.IConnectionFactory connectionFactory,
            IListarLogUseCase listarLogUseCase,
            IListarUsuarioUseCase listarUsuarioUseCase,
            IMigrateDatabaseService migrateDatabaseService,
            IBulkCopyCriarWardUseCase bulkCopyCriarWardUseCase,
            IGenericRepository<Usuario> genericUsuarioRepository)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _connectionFactory = connectionFactory;
            _listarLogUseCase = listarLogUseCase;
            _listarUsuarioUseCase = listarUsuarioUseCase;
            _migrateDatabaseService = migrateDatabaseService;
            _bulkCopyCriarWardUseCase = bulkCopyCriarWardUseCase;
            _genericUsuarioRepository = genericUsuarioRepository;

            try
            {
                if (IsServicoInstaladoNaMaquina("RabbitMQ"))
                {
                    _rabbitMQConectionFactory = new ConnectionFactory() { HostName = "localhost" };
                    _rabbitMQConnection = _rabbitMQConectionFactory.CreateConnection();
                    _rabbitMQChannel = _rabbitMQConnection.CreateModel();
                    _rabbitMQChannel.QueueDeclare(queue: ObterDescricaoEnum(RabbitMQChannelEnum.TESTE), durable: false, exclusive: false, autoDelete: false, arguments: null);
                }
            }
            catch (Exception)
            { }
        }

        ~ExemplosController()
        {
            _rabbitMQChannel?.Close();
            _rabbitMQConnection?.Close();
        }
        #endregion

        #region linq_com_EF.Functions
        [HttpGet("linq-EF-functions-manipular-data-diretamente-LINQ")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Usuario))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusCodes))]
        public async Task<ActionResult<List<Usuario>>> ExemploEFFunctions_ManipularDataDiretamenteLINQ()
        {
            DateTime dataTeste = DateTime.Now;

            List<Usuario>? lista = await _context.Usuarios.
                                   Where(x => EF.Property<DateTime>(x, "Data").Year <= dataTeste.Year).
                                   AsNoTracking().ToListAsync();

            return Ok(lista);
        }

        [HttpGet("linq-EF-functions-datadiff")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Usuario))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusCodes))]
        public async Task<ActionResult<List<Usuario>>> ExemploEFFunctions_DataDiff()
        {
            const int offsetEmHoras = 24;

            List<Usuario>? lista = await _context.Usuarios.
                                   Include(ur => ur.UsuarioRoles)!.ThenInclude(r => r.Roles).
                                   Where(u => EF.Functions.DateDiffHour(u.Data, GerarHorarioBrasilia()) > offsetEmHoras).
                                   AsNoTracking().ToListAsync();

            return Ok(lista);
        }
        #endregion

        #region IFormFile
        [HttpGet("iFormFile")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IFormFile))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusCodes))]
        public ActionResult<IFormFile> ExemploIFormFile()
        {
            string arquivo = $"{_webHostEnvironment.ContentRootPath}/Uploads/Usuarios/Perfil/Imagem/1AAAAA.jpg";
            string nome = Path.GetFileName(arquivo);
            FileStream fileStream = new(arquivo, FileMode.Open, FileAccess.Read);

            FileExtensionContentTypeProvider p = new();
            if (!p.TryGetContentType(nome, out string? tipo))
            {
                tipo = "application/octet-stream";
            }

            return File(fileStream, tipo, nome);
        }

        [HttpPost("lerExcelNewsletterCadastroOutput")]
        [AllowAnonymous]
        [RequestSizeLimit(SistemaConst.QtdLimiteMBsImport)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NewsletterCadastroOutput>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusCodes))]
        public ActionResult<List<NewsletterCadastroOutput>> ExemploLerExcelNewsletterCadastroOutput([FromForm] FileInput input)
        {
            if (input.FormFile is null || (!input.FormFile!.FileName.EndsWith(".xlsx") && !input.FormFile!.FileName.EndsWith(".xls")))
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.ArquivoImportFormatoInvalido));
            }

            List<NewsletterCadastroOutput>? excel = GenericReadExcel.ReadExcel<NewsletterCadastroOutput>(file: input.FormFile, skipRow: 1);

            // Depois de ler o excel, é possível fazer um Bulk Insert;
            // var result = _mapper.Map<List<NewsletterCadastro>>(response);
            // await BulkCopy.BulkInsert(result, _context, nomeTabelaAlvo);

            return Ok(excel);
        }
        #endregion

        #region channels
        [HttpGet("channel-classeReal")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusCodes))]
        public async Task<ActionResult<StringBuilder>> ExemploChannel_ClasseReal()
        {
            Channel<LogOutput> channel = Channel.CreateUnbounded<LogOutput>();
            List<dynamic> listaDinamica = new();

            // Producer;
            Task? producerTask = Task.Run(async () =>
            {
                PaginacaoInput input = new() { IsSelectAll = true };
                var lista = await _listarLogUseCase.Execute(input);

                foreach (var item in lista)
                {
                    await channel.Writer.WriteAsync(item);
                }

                channel.Writer.Complete();
            });

            // Consumer;
            Task? consumerTask = Task.Run(async () =>
            {
                await foreach (var item in channel.Reader.ReadAllAsync())
                {
                    // Console.WriteLine($"Consumindo: {item}");
                    dynamic objetoDinamico = new ExpandoObject();
                    objetoDinamico.logId = item.LogId;
                    objetoDinamico.endpoint = item.Endpoint;
                    listaDinamica.Add(objetoDinamico);
                }
            });

            // Aguardar Producer e Consumer finalizarem;
            await Task.WhenAll(producerTask, consumerTask);

            string jsonArray = JsonConvert.SerializeObject(listaDinamica, Formatting.Indented);
            return Ok(jsonArray);
        }

        [HttpGet("channel-int")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StringBuilder))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusCodes))]
        public async Task<ActionResult<StringBuilder>> ExemploChannel_Int()
        {
            Channel<int> channel = Channel.CreateUnbounded<int>();
            StringBuilder sb = new();

            // Producer;
            Task? producerTask = Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    // Console.WriteLine($"Produzindo: {i}");
                    await channel.Writer.WriteAsync(i);
                    await Task.Delay(100); // Simular processamento;
                }

                channel.Writer.Complete();
            });

            // Consumer;
            Task? consumerTask = Task.Run(async () =>
            {
                await foreach (var item in channel.Reader.ReadAllAsync())
                {
                    // Console.WriteLine($"Consumindo: {item}");
                    sb.Append($" • {item}");
                }
            });

            // Aguardar Producer e Consumer finalizarem;
            await Task.WhenAll(producerTask, consumerTask);

            return Ok($"{ObterNomeDoMetodo()} finalizado com sucesso: {sb}");
        }
        #endregion

        #region rabbitMQ
        /// <wards>
        /// tutorial: https://www.youtube.com/watch?v=V9DWKbalbWQ&ab_channel=TechnicalBabaji
        /// installer do rabbitMQ: https://www.rabbitmq.com/install-windows.html#installer
        /// installer do Erlang: https://www.erlang.org/downloads
        /// url padrão: http://localhost:15672
        /// parar serviço: rabbitmq-service stop
        /// iniciar serviço: rabbitmq-service start
        /// </wards>

        /// <summary>
        /// Exemplo básico de envio utilizando RabbitMQ;
        /// </summary>
        [HttpPost("senderRabbitMQ")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(void))]
        public async Task<ActionResult<string>> ExemploSenderRabbitMQ()
        {
            Usuario? linq = await _genericUsuarioRepository.Obter(1);
            int x = GerarNumeroAleatorio(1, 99);
            DateTime hora = GerarHorarioBrasilia();

            for (int i = 0; i < x; i++)
            {
                string msg = $"{linq?.NomeUsuarioSistema}_{Guid.NewGuid()}_{hora.AddMinutes(i)}";
                byte[]? body = Encoding.UTF8.GetBytes(msg);

                try
                {
                    _rabbitMQChannel.BasicPublish(exchange: string.Empty, routingKey: ObterDescricaoEnum(RabbitMQChannelEnum.TESTE), basicProperties: null, body);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Houve uma falha ao publicar mensagem: {ex.Message}");
                }
            }

            return Ok($"{x} {(x == 1 ? "nova mensagem foi enviada" : "novas mensagens foram enviadas")}");
        }

        /// <summary>
        /// Exemplo básico de consumo utilizando RabbitMQ;
        /// </summary>
        [HttpGet("receiverRabbitMQ")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public ActionResult<List<string>> ExemploReceiverRabbitMQ(bool isConsumirFilaInteira)
        {
            List<string> listaQueues = new();

            if (isConsumirFilaInteira)
            {
                while (true)
                {
                    BasicGetResult? resp = _rabbitMQChannel.BasicGet(queue: ObterDescricaoEnum(RabbitMQChannelEnum.TESTE), autoAck: true);

                    if (resp is null)
                    {
                        break;
                    }

                    string msg = Encoding.UTF8.GetString(resp.Body.ToArray());
                    listaQueues.Add(msg);
                }
            }
            else
            {
                BasicGetResult? resp = _rabbitMQChannel.BasicGet(queue: ObterDescricaoEnum(RabbitMQChannelEnum.TESTE), autoAck: true);

                if (resp is not null)
                {
                    string msg = Encoding.UTF8.GetString(resp.Body.ToArray());
                    listaQueues.Add(msg);
                }
            }

            return Ok(listaQueues);
        }
        #endregion

        #region genericRepository
        /// <summary>
        /// Exemplos de utilização do genericRepository do Chalecão;
        /// </summary>
        [HttpGet("genericRepositoryObter-id")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof((Usuario, UsuarioOutput)))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusCodes))]
        public async Task<ActionResult<(Usuario, UsuarioOutput)>> ExemploGenericRepositoryObter_Id(int id)
        {
            Usuario? linq = await _genericUsuarioRepository.Obter(id);
            UsuarioOutput? linqAutoMapper = await _genericUsuarioRepository.Obter<UsuarioOutput>(id);

            if (linq is null)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            return Ok(new { linq, linqAutoMapper });
        }

        [HttpGet("genericRepositoryObter-where-include")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof((Usuario, UsuarioOutput)))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusCodes))]
        public async Task<ActionResult<(Usuario, UsuarioOutput)>> ExemploGenericRepositoryObter_Where_Include(string nome)
        {
            Usuario? linq = await _genericUsuarioRepository.Obter(where: x => x.NomeCompleto == nome,
                                                                  include: new List<Expression<Func<Usuario, object>>> { x => x.UsuarioRoles! });

            UsuarioOutput? linqAutoMapper = await _genericUsuarioRepository.Obter<UsuarioOutput>(where: x => x.NomeCompleto == nome,
                                                                            include: new List<Expression<Func<Usuario, object>>> { x => x.UsuarioRoles! });

            if (linq is null)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            return Ok(new { linq, linqAutoMapper });
        }

        [HttpGet("genericRepositoryListar-tudo")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof((List<Usuario>, List<UsuarioOutput>)))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusCodes))]
        public async Task<ActionResult<(List<Usuario>, List<UsuarioOutput>)>> ExemploGenericRepositoryListar_Tudo()
        {
            List<Usuario> linq = await _genericUsuarioRepository.Listar(where: null, // Listar tudo;
                                                                        orderBy: x => x.OrderByDescending(y => y.UsuarioId),
                                                                        include: new List<Expression<Func<Usuario, object>>> { x => x.UsuarioRoles! });

            List<UsuarioOutput> linqAutoMapper = await _genericUsuarioRepository.Listar<UsuarioOutput>(where: null, // Listar tudo;
                                                                                 orderBy: x => x.OrderByDescending(y => y.UsuarioId),
                                                                                 include: new List<Expression<Func<Usuario, object>>> { x => x.UsuarioRoles! });

            if (!linq.Any())
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            return Ok(new { linq, linqAutoMapper });
        }

        [HttpGet("genericRepositoryListar-where-orderby-include")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof((List<Usuario>, List<UsuarioOutput>)))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusCodes))]
        public async Task<ActionResult<(List<Usuario>, List<UsuarioOutput>)>> ExemploGenericRepositoryListar_Where_Orderby_Include(string nome)
        {
            List<Usuario> linq = await _genericUsuarioRepository.Listar(where: x => x.NomeCompleto.Contains(nome),
                                                                        orderBy: x => x.OrderByDescending(y => y.UsuarioId),
                                                                        include: new List<Expression<Func<Usuario, object>>> { x => x.UsuarioRoles! });

            List<UsuarioOutput> linqAutoMapper = await _genericUsuarioRepository.Listar<UsuarioOutput>(where: x => x.NomeCompleto.Contains(nome),
                                                                                 orderBy: x => x.OrderByDescending(y => y.UsuarioId),
                                                                                 include: new List<Expression<Func<Usuario, object>>> { x => x.UsuarioRoles! });

            if (!linq.Any())
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            return Ok(new { linq, linqAutoMapper });
        }

        [HttpGet("genericRepositoryAtualizar")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusCodes))]
        public async Task<ActionResult<string>> ExemploGenericRepositoryAtualizar(int id, string novoChamado)
        {
            Usuario? linq = await _genericUsuarioRepository.Obter(id);

            if (linq is null)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            linq.Chamado = novoChamado;
            await _genericUsuarioRepository.Atualizar(linq);
            Usuario? novoLinq = await _genericUsuarioRepository.Obter(id);

            return Ok($"Novo chamado: {novoLinq!.Chamado}");
        }
        #endregion

        #region cancellationToken
        /// <summary>
        /// Exemplo de requisição com cancellation token;
        /// </summary>
        [HttpGet("cancellationToken")]
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
        [HttpGet("parallelThread-e-listaChunks")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult ExemploParallelThread_E_ListaChunks()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            List<string> listaStrings = new();
            const int numLoops = 2_000_000;

            for (int i = 0; i < numLoops; i++)
            {
                listaStrings.Add(GerarStringAleatoria(100, true));
            }

            // ConcurrentBag é uma coleção thread-safe (alternativa de List) que permite adicionar e remover elementos de várias threads sem a necessidade de sincronização explícita;
            ConcurrentBag<int> listaTesteLength = new();
            ConcurrentBag<string> listaTesteReverse = new();

            var chunks = SepararListaEmChunks(listaStrings.ToList(), ObterNumeroDeThreadsSafeMode(3));

            Parallel.ForEach(chunks, chunk =>
            {
                ExemploAdicionarLenght_E_Reverse_EmListas(chunk, listaTesteLength, listaTesteReverse);
            });

            stopwatch.Stop();
            double tempoDecorridoSegundos = stopwatch.Elapsed.TotalSeconds;

            return Ok(new { Qtd = listaTesteReverse.ToList().Count(), TempoDecorrido = tempoDecorridoSegundos });

            static void ExemploAdicionarLenght_E_Reverse_EmListas(List<string> chunks, ConcurrentBag<int> listaTesteLength, ConcurrentBag<string> listaTesteReverse)
            {
                foreach (var chunk in chunks)
                {
                    listaTesteLength.Add(chunk.Length);
                    listaTesteReverse.Add(new string(chunk.Reverse().ToArray()));
                }
            }
        }
        #endregion

        #region query
        /// <summary>
        /// Busca ao banco sem Entity Framework; utilizando SqlCommand & SqlDataReader, à moda antiga;
        /// Exemplo para MySQL;
        /// </summary>
        [HttpGet("querySqlCommand-MySQL")]
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
        [HttpGet("querySqlCommand-SQLServer")]
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
        [HttpGet("streamingFileEmChunks")]
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
        [HttpGet("simularStreamingYieldTextoSimples")]
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
        [HttpPost("bulkCopy")]
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
        [HttpGet("unificarListasComSyntaxQuery")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LogAgrupadoOutput>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusCodes))]
        public async Task<ActionResult<List<LogAgrupadoOutput>>> ExemploUnificarListasComSyntaxQuery()
        {
            var lista = await _listarLogUseCase.Execute(new PaginacaoInput() { IsSelectAll = true });

            if (!lista.Any())
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            // Simular duas listas distintas (mals pelo exemplo bobo de par e impar);
            var listaPares = lista.Where(x => x.LogId % 2 == 0).ToList().Take(2);
            var listaImpares = lista.Where(x => x.LogId % 2 != 0).ToList().Take(2);

            // Unificar listas e criar response;
            var queryUnificado = (from lista1 in listaPares
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
                                      LogId = lista_join_nullable?.LogId ?? null,
                                      Data = lista_join_nullable?.Data ?? null,
                                      Endpointjoin = lista_join_nullable?.Endpoint ?? string.Empty,
                                      Exemplo1 = GerarNumeroAleatorio(1, 1000),
                                      Exemplo2 = $"Fino eh {lista1.TipoRequisicao}",
                                      Exemplo3 = lista1.StatusResposta == 200
                                  }).Distinct();

            List<ExemploUnificarListasComSyntaxQuery_ExemploController_Output> queryUnificado_list = queryUnificado.ToList();

            return Ok(queryUnificado_list);
        }

        /// <summary>
        /// Exemplo de LINQ avançado, fazendo select com group by e inserir resultado em um Output;
        /// </summary>
        [HttpGet("exemploLINQGroupBy")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LogAgrupadoOutput>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusCodes))]
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
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusCodes))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusCodes))]
        public async Task<ActionResult<string>> ExemploLINQComSelectDinamico(string nomePropriedade)
        {
            // Ward: caso seja necessário passar como parâmetro "nomePropriedade" uma classe...
            // Na chamada do end-point/método, pode ser utilizado o "nameof(xxx)";
            if (string.IsNullOrEmpty(nomePropriedade))
            {
                return StatusCode(StatusCodes.Status400BadRequest, string.Empty);
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

            return Ok(valor);
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
                        new() { Estados = new Estado() { Nome = "São Paulo" } },
                        new() { Estados = new Estado() { Nome = "Rio de Janeiro" } }
                    }
                },

                new Feriado() {
                    Nome = "Feriado da Mariana",

                    FeriadosEstados = new List<FeriadoEstado>()
                    {
                        new() { Estados = new Estado() { Nome = "Minas Gerais" } },
                        new() { Estados = new Estado() { Nome = "São Paulo" } }
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
        [HttpPost("migrateDatabase")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(bool))]
        public async Task<ActionResult<bool>> MigrateDatabase(int minuto)
        {
            if (GerarHorarioBrasilia().Minute != minuto)
            {
                return StatusCode(StatusCodes.Status403Forbidden, false);
            }

            await _migrateDatabaseService.Execute(isAplicarMigrations: true);

            return Ok($"Update-Database finalizado com sucesso às {GerarHorarioBrasilia()}");
        }
        #endregion

        #region random
        [HttpGet("listarTodosMetodosAPI")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StringBuilder))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusCodes))]
        public ActionResult<StringBuilder> ExemploListarTodosMetodosAPI()
        {
            List<dynamic> listaDinamica = new();
            Assembly? assembly = Assembly.GetExecutingAssembly();
            List<Type>? controllers = assembly.GetTypes().Where(x => typeof(ControllerBase).IsAssignableFrom(x)).ToList();

            foreach (Type? controller in controllers)
            {
                List<MethodInfo>? metodos = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(x => !x.IsSpecialName).ToList();

                if (!metodos.Any())
                {
                    continue;
                }

                dynamic objetoDinamico = new ExpandoObject();
                objetoDinamico.controller = ObterStringAposDelimitador(str: controller.FullName, delimitador: '.', isEstourarException: false);
                objetoDinamico.metodos = new List<string>();

                foreach (var metodo in metodos)
                {
                    objetoDinamico.metodos.Add(metodo.Name);
                }

                listaDinamica.Add(objetoDinamico);
            }

            string jsonArray = JsonConvert.SerializeObject(listaDinamica, Formatting.Indented);
            return Ok(jsonArray);
        }

        [HttpGet("validarMetodosMemoryStream")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusCodes))]
        public async Task<ActionResult<string>> ExemploValidarMetodosMemoryStream()
        {
            string base64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAbwAAAG+CAYAAADoeLHUAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAP+lSURBVHhe7P1Xkx1Jui2IrR1yixTQWhZQECVQqsXpPqfvvTOXl5wxzgtpNBpppM3D8IFP/A/8RxwzztD4Qs4YeTlXHdGqNLRIILXaKuTmWp/vyEygqrqrUN3IypO+Eo6IHcLDw8P9W75ctr78r//lBB4eHh4eHv/MEUy3Hh4eHh4e/6zhCc/Dw8PD41DAE56Hh4eHx6GAJzwPDw8Pj0MBT3geHh4eHocCnvA8PDw8PA4FPOF5eHh4eBwKeMLz8PDw8DgU8ITn4eHh4XEo4AnPw8PDw+NQwBOeh4eHh8ehgCc8Dw8PD49DAU94Hh4eHh6HAp7wPDw8PDwOBTzheXh4eHgcCnjC8/Dw8PA4FPCE5+Hh4eFxKOAJz8PDw8PjUMATnoeHh4fHoYAnPA8PDw+PQwFPeB4eHh4ehwKe8Dw8PDw8DgU84Xl4eHh4HAp4wvPw8PDwOBTwhOfh4eHhcSjgCc/Dw8PD41DAE56Hh4eHx6GAJzwPDw8Pj0MBT3geHh4eHocCnvA8PDw8PA4FPOF5HHJMps7B/Wp9L1dP3d5jMGeevOym2Ot/zUsb5y7RTgutb3E6vnPf1O3x1sPD43vAE57HIcYuXWl/l4xEQiIkueAV1xxvzoWopsebe420uNnrdGKv/xUPlkELBXOg9t39zJC8NyADhnVAF3I/JOEFvEtZ1T2n4n2OJM1nDw+P7wlPeB6HFA1dOBpymO5PeaQ5s9c5uD1HXrpY7KOsNN03NPuvOsHtT5o/O7xLX6JLp+yaK/n/HoVn95L4PDw8fhg84XkcejRUM6USHnDUEpirX3Itngt03rZSZLzHFF1DUm7/VZjfRmDuKfo/pB8RnbbNc3XRhK5u1XTTfZ6jt+5Oe8b0ufzt4eHx/eEJz+OQwiiGbi9tuF8605qI3L7pjPR2CHCX9IwEnSeE8+VVZ397rg9rR3ZBTb8asuOfiK7mRXXQkJ5CxvO6jz5ZtacOenh4/CB4wvM4tJB+ayhuFzryTber8iqSjSMot3VOik9EaD6RmBxLvexeJjuYuotIdk7h8QDdhMpOzkiPrjLC401Tj7Wre+WP9j08PL4/POF5HEqIK3bpTDBGMewQ3KuEtkNsciIqOhGgrqXTOfNZXu3y3F6vp/c6qqWAM+Iy2p3eu1udWaPiSf02wrP7eD+vU6bVoT3eenh4fA94wvM4pJjSxd7elfrZkI8RmxTbXrcLI62J1J5z7nzjkyogX/4zhtKWrnmGIy6n/AxGblN1RzZ0+9aCZ3/2THeZ3efh4fHD4AnP49BCPR1FIyK9Bg1huTO7Ttg5s3OfjlCmiZREUjxkww3kgpY5DSGwLY/VzG3N8ANTf5b9XNeYvcRrqm7650jQPdmcO6Sne3h4/EB4wvM41BCNvAQjE0c9QRCgrmsUZYWq5okgJGEFKMlsBX+3Iv4OJih5rTnmpoLkNuJ+vyqRhyHyKOKxABmPZ/SiEPHxviBJUSj7xSkQJSiqCf2sEfD6kPfVE7XpKXRTt8Nw+r136+Hh8X3R+vK//pc+53gcOojSRF5K/K79zVVJWnvdtHpSEM+ZAiNpTVokIjsj9pmAvGUo+bvgdSUJEVFsZBaK0EiULSo3dVKZlCXqPEeVZbbfqipE9DeJIyRJTP9JmDXP1zxHCReGASpe43p2ajB6wEvk3G+pP5GtU38eHh7fB57wPA4lvg/hiexaQYQJiayk4CqNaEh+YURH1VZmJDZHcAXPbWUFtkloI14sqipFalRrKUkwJTt2qN5S/u4mCdq6f3ObnpbiUsSRZlMpSXI5t7qPzzLCYxg004oRna7hEe4LauvjQdv38PD48/CE53Eo8W2E1/S0FOEJqsYM44T0E5LcahQ6HCYI4hQ1yWswKZDx+mFeYmMwwiIJbHlzC2uDIUZ5gXGWkxep4khyPZLi0ZkeTh49glNHj+J4r4tTdK08Q5GNSW452lHLnPbLfESSjBgWhtAITo7MqP1pO6CjPZ99PTy+LzzheRxKfDfhSd05GinVThfG1F0ByU4dT2KEaZeKL8KI965UBZ5ubODh0wU8WVzG2miMEe8pW5FVc/JmTGqyJJVaSCdVN9tuG/EdSSL8/PpbOHdkBjPtBOVowGtyzKUxIhJpPthCm9fY7CsKj5EcQzjt3CK6U1WpIz0PD4/vA094HocSjvAcXbghAiK8yvYFnc8rkpUIbxIaiQVpD0HSQX+cY3E4xj88e4anW9tYWF6lqhshj2Ig7WASURWS7NSOpxpHm01FVZdVacQX0feZusDlTogPr1/Brbcuo0PxBpJem2HotGqEPB+oPY/hkqgTDVcMb03FqK3z15r+PDw8vifC/8sHV/6v030Pj0MEKiUjPJGd6I1ur2LSOSk0OtIUJqrKJNllFfDsxTL+cP8x/uHxAp4NM2zXJKH2DCYzR1F2ZjEMUwxIaxu8eDQJkJEwC6rCnP7IWe9MPmu8uYZi2FcQMNvrYiblM0iIIjq19aljy254SHqBSNoNfVD4RHg75z08PP4sVK708DiUaDqCSCTJ2ZI7UwaRgmpT3cWtmKQSqRsJhiS/5aLCZyur+I+PHmGLSm6cdJGnXWQRSY4eDMoamWoxeW17ZhZxpwskCQmPZEdXUAVWVIFFp4d89hg+W9rEv//yIR6u9Xl8Bq02/eP9BclO4bGAEE0YHSUr2zqa9vDw+P7wCs/jUML1eNS4AhJHQ3Qt9Upxc1yGPDAhcyVRG+MSGFPhbcQp/qeHD/E/PLiHtXYH/SBBpva6YNq2pqpG0lBAkopVRUqlZlWTEzkqN/nPcxPSZ87nlO05bFQh+qWOBOikKXrtFGlE8i3HDBTvi+lnqOrVChXDHDAcLZJwXU1IwfTOaNDDw+P7wCs8j8OLqZzb1UnW6d/+l/pLSXDjcY6ICk7K697iMu6urmE9irAyqTHmVmTXLADr9Jfm2KzMRRMSnjnRmaonRXhu6jANRl8c5WgdO416/jjurqzjH+4+wIvBEEWS2qD1CZWh/CW3IQwiG+JgqtTxpoeHxw+EJzyPQ4vdSZml6EgqJBJbdodKymZUiWNs1zVyEs9GWeLLh4/w9MUi76EqFAv9CNIRPaqXpYYegApuZXsLnz9+hE8fP8bicIi60yOZagYWUmXBS/gXM0wizxZJNOY9Xt15ePwweMLzOJSw+SopleSslyZdRLILNeElyaVqRdhWO1qvh5WywOfPnuHR8jKGBQlH7XNJh4Szqw1/MPi8+ZkeRsM+htkI0WwPfT76t48e4w+Pn2HQipEHKbKaYVEvlYokSZJtVSXDKsIzT8wrDw+P7wdPeB6HFiI7dfhX9w+pu7DW2Db7hUKERwKs5+fxcGsTf3/vLtaKAq12R5OjIIlS58lrQuosYe7LsyHGlHDBTBdFr4vH/QH+sLCEzxZWkEVdIJpBFPWk7xDUJOZaw+A1MlBVpJ7wPDx+CDzheRxSOIWnBjGn8JgZpO4mWuFOPSojjOIUS0WJzxcXcX9tFWNNKRa3SXi8mOSo9rTXhchqNNiyKcRalGuabHqrFWDc6WJhXOA/fHkfq9kEBUkvas8gCEmwqnK1eytUZcY9T3geHj8EnvA8DiVc+52r0hRxqHqS+o571E9BbL0yB3GCf3j4EJ++eI6hhhbw99gm2IwwqeTLj6jSJCZ1iTgOdgiv32oh781ii8++v76Nz58tY2VQIENiYwGl6bSCA/+RdD3heXj8UHjC8zjEEGE40rCZTER2rRjZlPD6VHm/ffAID9c2EMwdcaqvqJAkbWvH+3GYIE21SoImpa5RaXUEkuo4jDHgs0dJF18sLOHp1gCbVJRD8nJORTmhymwFJD9NWebh4fGD4AnP41BC2iyKQuQ5lVIYImp3MabC0pAA9OawVtb4v/+P/xZbdYBo/hg2s5JKK0Dc6VEVRraSwo9VeBVVXV1X9I8+kchqEqqeMZwEGJF07y6t4u7yKp4PRxgzXCXdqKqRlaTmKKEPP+75Hh6HDZ7wPA4nSFhVWVKtkUi4v5VlyOMEedrGs/4QXz1fwmpeYrsCxmrXIwFpXk23Jt7EFJbThq8PzaRis6kYcdGR6JoeolKTw4ikt7KKT58/xwpJDrNzJL02MpJwiyrQz7Ti4fHD4AnP49CiIqElVEol9zUEoeh0sB1F+Jok87uHj7E+LtHPaxIiCSZIEJDsWq0A1YSEN3Ftf68LqbqKuU+EJ9pSD9GITlvNAlPyWUWS4N76Gv7pyWPc39rEJo+NoxS55nFRO6InPA+PHwRPeB6HEqIK66QpAtNsKSnVXYfqbjjEl4uLeLK5ZRM/q7NIS0MCRHrMLhORHTWeZkv5EZ00jSo1vM4Ijx5FDEtCERfTqceopinLkwiDJMSTUR+/X3hmRNzXhGLtWRS1CM9nXw+PHwKfYzwOJYxkVH1YUMGR9NDtYZ2q7YvlJTzY2MAo1FI/XYRhm9fxSjFTRaKbtrlpdpQfA1N4JDVNHSaCE+GllJpy8bQ/Sl+rnx+dxbCb4A/Pn+Ef7j/A8jgn4c3xvpR++Ozr4fFD4HOMx6FFHMRGeBOSWxGFeLy+hq9ePCepjKn42mShBAGvETE2hKcZUmxIAwnv9Ss0ncJzy/1oOMSU8Kju5DQIXleMqgJFGmHUjrEwHuCrxRd4sLSKzVGJVtzxCs/D4wfC5xiPQwkn0kLyF/fCEBujEb569gSP11Yw5KEx6aSoSW7iNmaTiE4rKKh7ppuSjCdsLN/rwQhP3tHJGxFeXE2o7ibc1yAJ0lkaY7vMMVD16UwPG0WBT+8/xL3HCyjconjml4eHx/eDJzyPgwmRj3o10k3kmJQ1SZioonE7dKSd6Y9mV9WJZRCiDBMUYYp1qqYnK5vWUaWgshuUlVNYdUG/NH8mn6Dqx1ZAgtL6Pco6ewlH+zrWuOZc80TXxcTxpKoxAwQkOU1aLYhA1S642zbYQjftIRvlKPIJerPHUEZt6z36Kd0a7x8x7DkVqDq46H3djDGMBduap4wbnXOu6eTSxJALl4fH4YFypofHAQRJo6b2EvlQf8mga6ke51RVSDKJIlQkrroqEYVUaZEUnViG+70eNnlnOXsETzYz/Ps/3sW9hU0S4Cxa6QyqOEYy30Wd1sgwRN7KUE4qlAXJpErRTeath6fG8VVVjZhkBCT0PyY5JtAogpDKsSwLpImW9mkhYtisV2hW8coISV5jNuA2jjCuNdNKibIdo2K4xyMS7aDEkdYMZqoOss0K4zJBNXsC98sa/90fP0O/ewTrjIO+VGGaop3w/bIR6tEQCYk5INm5lflSUra2+i3a0wTUBbee8DwOFzzheRxImFrS/1J5ZrqdcwuxmrhBXVPt8HdIotH1+q0hBTkJcKRhCO0Onmxt44unz7GwMUBGsqqCFAX9VHXmYDwkNdCPNEQ4XYi1Rf/qcoLxMCO5kfxIrtbTk6RXkuWsinSarSo+QypKpJhnY0z4/LIg+VKVnT99HgHJsxhSwWVa/4fqlG5E4tPMKwkJLKFyS0mMKQk0oQqdMGwDkthiVuLRYIjPF5ao8njF7FESZs3wjo08u+0UtSa6nqpJhnIaJoVN/zf61xOex+GCJzyPAw5nuJ1ZV1UejTmJSI4MY4RErWMrhNfkHyNF3pGRjLTe3aOVVXz5+DHWSCBh2kYUJ2SEkIqMSqywikJ3v7XnUSOR9DSHdE2FpLkw6T0Jz5FbWZZ2jZ4paN7LJNGMKA6R1r6jP71OF+/cfhfHjp6kz1ShVHpRK6JK1ErmJf0qEFOtqSqVoaZHDDXJMOD9E/o5yHKsbGzj91/dw8ogQ510bAjFoOAL8nkRXalF9IzYKt6uBWgdyVnIjAS158Lp4XFY4AnP4wBDZnuPE9lx61r0RHjcko1MlZFodDaIErQiqrggxJP1TdxdWsbTzU2Mea6VqOqPao0KLuZ1coGqSqncsoyKjgQXUenFSYuEqCEKjuCkpCws5A+RnLb6rcOq1tTzpQIjPnPCcCQk1csXr+C92x/i5LFzmJSko0JVnjGfpzeQ34VNEF3WOcmLqo8KbiLS472agmxEHry/uIKvqPIWNvvI4zaC3gwy3j/W+kV6Jv3h2/ANRHrct4KAAjdVfBZuD4/DA094HgcUstbSas65ajoZdWfYbbkfOREIt1qgvCbhBEkbE5LGoG7hnx48wN3VNWzxnOapLEhKg1GG8biwakupLq1sXpeOSNVxBVR2VTVAUQ5JXBqUzkN8ZkRiU3Wi1J2qOkVyRVnQSbGRrBjMin4VGcmIpNPrzOC9Wx/i/OkrSKNZlDkJUgKNqlND/GoqtJaUHR3pDwXJVaslaKhEi+RWhW0MWolVa372bNE6sMRHjlPptbCdZ4g6UqqO8LRgrKm8hvAmmvhaTqH38Dg88ITncXBhjVSO6Bzp0aDTOcMuhhPxSOu1UKoaL1SHkISEUOIZ1d2XL5awpqrK3ixa3Z71dtRgbq2EoOm9REAiPqnEdkJCCSeoij7dNuKgwPGjs1SBqi4loTAnWaeYuuJjGRaRqzqtVFRYRoYkIAYz4PFuu0c1l+DY/GlcOPMWTh8/hyQgieVSYwH9jEhKIlFVn6oqUxWTE+T0u+B7VQyj5tqsSJrPtkf48vkSt0NbJV2rLRS8P2B43dJHrjrTVWk2FEeym7Z9engcJnjC8zigIKFNXUN0rlt+85tnRHr6RZKpw8jWuRuQxBbWt/D546dYotoaUfFVWimBWUEzriTc76QdU3Zu+ICqGklYJJ5WPcak7OPoTIAbb53F+TMnSYRqd3M9HtV0J0UnZWckJ9JhGEI+O1L7HMm1zefNz87T3xitKsFFEt7tt9/HyaNnJB5RW3seSY5+qS3PSJ25VMKs5DNKkq96oqpKdkgFuk33ZHOAPzx6hq8XVzDUBNedHoYkRw29cPHzSpWmyJyk52LJw+PwwBOexwGGCE2rFuyqO3M8bKae25rkUVENiewyEs+ABn+xP8L95VVsM/mPtCoBr97OMoyLwjqW2HCCLKeK03BzkR791Hi8vI9oMsLZYynev3kJJ47Nop2S1KwtTz001VNTVZ6V9egU8lxtcRoaQYU2zulfgDRKSZwitxZOHT+P61du4dSxMwjVS7RQoB3PFXluHWEEjdOztzW+4jsxjJtFaW1367znsyfP8dWLZVN5NQlvI+NzyZq6XvHk4kbxov+V7b3C8zh88ITncSAh428kMGU30YGMukNDDi1kajdTVWWna4royeY27i6v4fHGAPVU2UmfhWmKcLo+XkXia2tYAFVZyd91MUY7nKAXT3CsA7x79RT+zd99jI3VRSM7jbNTFaTm2UzSmMqOxEXSK0h0vd4MQhJnTgJVVWaZV5ifO2IdVFokuPGgwLnTF/HLn/8tLpy7TGIlJU5UpRpitjfHfSo6Elscx4ipGKUgMxKhSC/qdpFR6aka80V/jP/0xT387sETI8CKKrVi+CuSnqpDXZywaEDy1gxpFJseHocOnvA8DiycGXdO2LXhTs+ESYqcBwu13ZEAlscZPn26gPvrmzYGz9rBpu12zV2q7LP2Lqm1IiMxtdBrx5gUAwRFjneunMbHN84hKjZRZyRN6/6vJ6u9jE+dKipXnepg+yQubbTEUMzwiNDacZeERuU5LHB8/iQ+vvMz2/Y3hugmM3wmPdMaQhX/FaStWsMiSIihCKtyVZwMX6HFY0neWknh3tI6Hq5soCK5ZnxWzndTu5/G9ons9JbqSaq2RA+PwwZPeB4HE6bqSCKOYdwBoyxV45Gy6CZUURXJQMS2RaX3YGUdf3jyDI+3SVQz8yS7iMQT0LXomBmsnUt9IVVFqmpIKr0kcJ1Vxts4kgK/fO8tfPL2OaT5BorhJqoyZ0B0j8jEOXpn5NL01hTPqPOMnhXymVJ6euaEUqsdp9y2eEy9Nu/g1vV30Ql7ZGmGX0MVWrFTg+RTdYgh39FJpeV8RoFAnWYiEl7SpoKN8fXzFXz6+AW2aik/Ej7fXVOoqaaUvMk4YeDoFFIPj8MGT3geBxYiOyO9b3EivIyKCEnHqi3vLy7j86fPsDjOMaIayqn4aioiVRmqjS4k2TmnDh7q2VghogiSy0hs1Hh499oJvHP5JI5FGWZaY2SDTZR5Tv7gE8WYvKaByM6UnYiXm5Cko60UnoYkiPCGGuxOQm4zjDbdWNjG+7c/xDs37qBQ75oiRNJqox11XHsiPWhNSr5dyfBR35H0olghnWBUT5CRGNfoz32qvHuLaxgFCcqILE1SRaxB6yQ7izMSMsnTw+OwwROex4GESK3ZGvFNXaPuVFWZi8zUgSMv8OmDR/j6xRLKLsnm6HFsaFYSKi6tUhCRLLRCgQhP9CEykcITsdR1hlF/gFNHU/zqo3dxdp6qafUpjkQlylHf2vCMRxyNuBAxHIJ+SdmpdyZDYgdEfLO9We63bPiBphzT1Gf6Pe5nOHfqIj56/xc4Pn8G9ZiHM2bSKmTYQqq8GpNSU4YVVJ1SozkV44T+1xjzfTQRNjpzWM8m+P3dJ1gdFhjyvgnVXytO0ApVfSsiVvWmQufhcbjgCc/jwMKRnqimcWqBcxWS1m8zjK335dPlFTx8sYjVcWZDENR+t0USVI/FhvBCqien7pq7K6s2rMoxjszFePfWW7h17SJmQpLg9jJSslFNP2ymFf5ZeLjv2u4YClUbSpGJiLlVm5lORVRqPY3741UzMz2MRiMUeWlDIcJWzDDEJL0LuPPOR5jtHCHJhSjGfE4pguPbqvqUJKt+MppBZVLl9jx10ClbVIHdOSq9BPefr+D+whKWtwZUurxC7XwKiwLKhyusHh6HDZ7wPA40dqs1Zfanv+mMAPj70bMFfP3gIbZIfC0S3ZDHtrSUQUrVQwWoxVa1RI+pOzlTdiIDEhnVUBwHeP+92/j5xx9gJqFOK4bohRW2l59bo5gtGSSFpjtUharQ8Bj/TQnPVWlqK6bRtt3Wygp8SlUhpfJSZxTVMKZxx1ZjCFsJ3r19B29fu4UjM0dJdvSgmiCN3CwsGtMg0ksjvmM+sipVTTatdrqcqrUKUwxIkJ/ffYBni8vYGpJU9QwLiD2aYdaOh8fhgic8j58QGvKSUnvVvUpsUjVuX8pHi7NGdUACc2u/qbPGJs9/uvgcny6/QD+NEc/PIy8q1MMCR9IZW8Vcf7wVWk+10jRe9FekF09yzAVjnIgz/PzmKXx47QQm41VyzRhpZw4Ly1tGi1DHl5Z6PNIfEYr26EejEo0VpQIVViOcAJE6qvDs1nYfs3NziKhEh4ORdWixJX2KFk7On8bNq7dwYv6UFhLi+7nenW4+zgolCU/dNfNCs7wE6JLAJyRyDX9QZ5w66eHLF6t4sDnCYhlgA3wGj0vXiiBjq4fVGEb9TWE7TTy7N1AcN85dyTMicr3Lzo0eHgcDnvA89hFmUmk8uUvWcYZW5La7aKlzrl1u1/hSrdANST5xW9WAQFIAx+JZtMbA5naGenYe/+O9r/D78RYet1t4EZNgaKRJeziah+huFpiNuxiRIDIS3aSXok9yyloVtK5cr6aS217D/+l//gH+zc0ugpXfYqa1gdF4iFF0DMv5LEa12sW6DF/k1r8jyep9pLrqSkMaSEz5EO00xHDYt0Ht80eO0Q8tKksCo9LrD4ckwhav6aIYk0Jzklc4g3oEvP/2B7h24TqO9Y6TnLWqgoYmSJ2RYMMIQ0ZG2HXtgcWA4WUcdXlOc3bm6uhy7hr+p6fr+G9/9zW+ypjVT56zwfcjhiWhtJ3wfUXwu8xlrGa/FM/kSRQkxoLbSuRIJxIPqTBV/auv5eFxkOAJz2PfQJv6skqQ1JooSerMq8pual53DPQEKVVboYmSVV1IItjeHpBEekjnjuL+0goebW3jeZ5jMwyQJTEmkevi36EK7FAxlRnVkQaX07dhQZIkj7REpsUQUTnAv/jwAm6cauNYtE21t41eXFmvyNEkwRoJqWolRlwiaIXPwYXNem7yrHV+4SmFXmdiVWHqvRjm5p3knNbU0IXQFrYVwak97+qFt/D21Zsk4R7Gw5zvSYWo4QZ5bR1zVBgQrJhgvUv1nBbJKsI6Zes2iW8hq/HpwjIerm4CjJ/OzAyKgn4pyE18SrXp59TZUe64Lf3U/vRaPcOF3sPjYMETnsf+wcitMZ3O5OtP1lW2tTGsdhUNsl1pW/0Guhp/RkITo2h+yc2yRNntIItTfHr/EZbWtqlmChJIhITkpGm9bPovklrBrdbEi9SDkffXZYGYx8NJgQQFTs538be/+AjnTh0jnUnNVCSrkNeTIHmfqiNfpgiSFzfqrNL00tyL5lin07F2P6sSpA926zfg/NMqCxfOX8K7776P0yfPkmE1A0tE0m5jUmnSM1039YfUZ8TErVSbFJoWum0xjrZHY3x5/yG+evTE5hJtdeds/TzFpI1BnD7RDXvgPl0Tx1bUkPcGd6Xe2727h8fBgic8j33DruHcdS+ZVDPAclJKbt+MsAwy90MSglrPZNiLOALmZ7FF4ru3uoZ7i6voZ3zGJEUadhEhtua0kqSY1wWySUm+FYGl1gYW0VNpvaAY4Fg3xCfvXceVs8fQiUmQWR9VmTmFQ7JSy9xwPJYomjoSjXamod8LO9yA+zNUV9ruHH/pAmE3BnIq0Chu48zpC3jr6g2cOnmBREdVO6ZSDF2nG11pbXFTV6t6ltuKYW1FfL9229bPW9rcxr1nL/BoZR0bmscz7ZlC1HP0vCbkO4UMhmtnwmk9Ywr3q3EeHgcLnvA89gk0mCQPmVeXDHeTovGKDK22U9PaGF9z/K1hBPU4QzuKqdRq5FQywfHjeNAf4D/de4iVfIKCSigIeiSzDsLaEZ6IqdLMKeRHrY5Qqk2Mz5S6i+sM3aDA1dNz+LuPb6Mb5miRAMl4DANJRKuR81q1Jw7GuVVYyug7NyU6hVkKTu+mXi38LS0WqAqznmBudo7n3Ls2JGN3Sh7KaZdHnAsxHOQktw5uXHsXN99+D53kCPIR/SOR7xCe3SNV59o2rfONOYZaPUg1yJ7E+WJ7hM+fvMDjzQEmM/P2/i7c7tmuQLHH2dlpYYPOhY9HdB/fbxpyD48Dg10r4+HxBtGQhDOrSoaN4W2OKnHuNbx7nK6R/dXYNKoUVWcOaNwXqd5+v/Acf3y+hAEVUA6qG1DB1YkN3m7VJIowRJhEaKURSvpTajUCKr6QZNduZbh0sov3r5/F9fNHSYADRJMxUnKl1qYra3XycAZ/QLJVtw/BGX/3PuqFKTSk1rxX89fr9qxK0251tzu4y8wP84dvHZOksnFFddnC8WNncP3qbVw6dw0z7WNoUemprU+kp7u0cT1XG+LjuzGEmm2mDnltZxbbfNW7S6u4t7KJdfpZBLG9i9oB9xLYS6Q3jWs5oQmf3eMOeXgcGHjC89gnOKOuKkJnRAkZVjujX41ppTOjq/29aFmvSA0z0Ji6Laqnf3zwAL978gxrkwBDdexopagnCYlOa8+pBc9Nmqymw1Lr89CKB1J2QYWgHOBou8bP37mMT25dRFJtGeHFpE0N8lYbWUEl2SJ5gM8djEYMmchiN6QG2/A/eyF3zLXf6diEhNe1Y+78LtwbC+5a+d1qaamixAivLgOcPnEBH7z3M1y/fJuKtEuS1gwsUmai0ldJTw7WTqkJtEsNfaB/z/sZvlxcw+fPFm2AugarN51fROYNduJ9uu/APX0vKTyd2b3cw+NAwBOex77BGfm9VnP31w7J8aLmmM6aM6Or1RDaGFLlVdwOaIT/8Pgp7q2todACqDTkJQ16K0jdDCb8a4ihpKIrSk3pVZPMJmiHJcmtxrHOBHeo7t6+eBRlf4mEMuRdmhy6RDWpeR/pLdQUJxEyjeejX/YODKcCufM+pu523sScqjR1gRaCFfG6o9/Ejh9kZQ0vSKjyJiQ2VW120jncuv4e3rp8C+2gZz05pfICMbh1QlF1K3fptG0xnCInDUgf8+CQz91k+eDx1gCfPn2OMck0IxE2E0xrJXVVYDaw2G6+gTydhtrCt+M8PA4OPOF57CtkpM2iuj3bOpM6sbXptAyOVCClmc0UomrIisZXqmQrK9A5dhIbeYn/8OkXuL+8grLdQ93poiJR1KqyI2VpnJva0/QYjbeOooBEQv+KAY50SSbry+iFwH/1r3+B21dOYO3ZV+hGOdpxjboaIy8zzMzO8vktEuUESyvr3IrunMFv2uyaakzxHwWnnY/CEOPhGJ12xyaK7pGMtdW1300Z7kxAIpqQyETYadwj75IE8wBnqPR+9fPfIKgiRFSvaagV2lsYa4FZPi9tt62qttL0LYo3+iXSU1teqzeLtbIywvv7r+7ZOnpaSqhU4YEKNmPA1ZnHzR6jAkITmilEfCJYHWk+mYfHAYEnPI99hqzm1LVUQdi4CaI4NJKraIRbNMJa3y7QODYShsaZoTeHlXFpy/682B5gSENcqhMLjfKocF3yZZ8rKjPNTqKJnrXkT13mmBRjzHeoeJYe4lgX+NVHJ3DpRAfpZBtz7QoRRswcGvIgpnQTjk34zErPIOmJ/GT0d+nZ/XbHHIwu7DpueFDqTnCKSQd3z7v/iZ0dhruqbGiChKUmVtG8mjEV61z3CE4fP4sLpy7bigqDzRGVahvdzgwLCSVGo7ENcreHWmhcu6M6qeSMOy0YO4wS/O7hEzwj4W1TDfdZMMi4DdpdBEmCca51/lxwLKy2b79ecR4eBwee8Dz2EY1BFqb7jdrjVit8i0gKTaXFbU3SK42GWshIPkV3Dk9osL94togXW0NUal+jE0Gqg4lSN+mC5KSlUt3MINYRgwwS8Zh6Yeab27h2NsW//vX7uHSKKqzcpLrjUyYZg0CS5PVSPEWtNi4SKIlhPMpt7stdohP2Ep/7LWd/Un70R6uo28oICoMUof25K3mjwd6e794MgXAHuTXSU/gjzPC9Tx07g9tvv4+5znFUWYsqMLW5OCWIK6o7rcSgSk4N45Cv2ojwCj4/5/uMSXz3WFD4dGERD9e3MAoTtGY08XSAIRWg9cTUrVM0n8VhJ9QeHgcKnvA89gm0nmbUZUWlokQVUnbOUUO51ElHG25VmTnVzqhUtVuLSiXBSlHTWG/bCt9bOa8KUyobGm4ShVYGV1tdqYVSWxV50FVjRrTTMZ+U6hnDVZIc8Mk7F3Hj4jHMpyUJb5uqqU+ioKTiNXq2Vl0o1EbW0qp4VFSDMUlF4aRn/Gdtivy1iykZTA/qlyaW7nW63Kc/utXUXbN11++SpTsVahwdicvIqxXSE3VeUSWt5s6cwc2r7+LaxZs4OndaIyeQjzU4XsSnRWW18gOjj/HW+KrHVAyrSE9TjKnA8NnCMn774Ak26LcIr0//N6kQQ5KzW11hN0zas0/mfnp4HDh4wvPYR8jyk1JISI7WHNnJqso4F2VBdUVDTMMvwhtXpEGqEy1oqjanL1+s4OvVDSwPMxSiAVVz8pqA8ovcRmWniZRzEhbVXSQ/SXJVgVZRIiEZpnWG/+wXb+MXd95CR5V65RaV3whVPkDMx2hhVbUXiuhKKjz10KxJfP3tIRWeKMkRXeNeZgKG2xSaVBzVIYmk2+1R4YV2pPlzNzXOofHLLTdUm9ATgeuMVliY6B0nIU4ckcr7ANcu30SrThiuMcOdoqsFbklapmZ5jyM981WciYpe5fRvMjuPJRYaPn++iK/oVqRckw6CNpUi43yXyF1I3R6x88IeHgcLnvA89gnOamruSpGc0xKNYXYuK3KSTICACkc9CGn1EdKYT6jitvMSf3z8FE/WtzBUdWNA9UUyqklmk7IkZ5ZURSS6gP6T7GpN5EwZVORj8muFDon0xuUT+NXHt3D17DxKqr0WlZ1mVglIn0GodrppRSiJrhTxURvqGYMBSVEC8FshSmjowUG/tAisOqxYVxURkBHGXtLbAzvJZ1sbnqtWVVWonF0pImM4NJj+/OnLuHntXZw+cRZaVsgYjberWlPeuHF0TbzaKRLZxMYu9nm07MxgNSvxT1/dwxcPH2OSpOgdOYqxqjVfCZsLszuye9TD4+DAE57HPoJkJAXjzPCOod/dZwIlyWmlbik8qbuABrlP9XLvyQKerK5hncSnMWYtKhvJwoBOs7BI5RmZkugmdGoFrDTDP72doYI5deQI/u4XH+Ls8R5a5YAkOOS9Y1vhIBTZ8X4pHCk8em6dVWwZIG6zUWYdSXbxpylASk+klaZuGjPNuKLrm67+7s7mfvfeQrNmnapDbeiDnPVgEeGRrLIa7aiLyxeu4s57H+L82QsosgIjErJUXkN2UnkuOvkfdxR0VW0OqBQnJGFVDz9bXce9p8+wtLFpVcdqB3XDP3SfC50558VOaD08DhI84XnsC2QwTaXQyXjrt1ruSiqyIqxtG0QtJGGEWNWJGg43SZBRxTwdF/jd8+dYrUuMwpZVw9U02gHPhSQlLZJq6o6ER5ttT9NfxCd04gonZoELJ0N8/M4pxOUqso0lpFMS0RAGjbMbUVk1QxpEHCGfpb6aOj2iitK6ctJNIhG1PTak7Y66M/ZWRjTyt+mlKZUmpysE16HEXUhnzCRKqkmQfB8qXPcOPE5PpBRFmOq9qQ4qGo5w4ugp3H77XVw+dxV13kI+LDHbmbP4VTuehc16m8pfFz7Bqor5SzOuZEGKJd539/kaFjaGqGPGKd/fFSIUN+5evX9FIhZhOl909NvdS+CBHbL81gs8PP768ITnsS+QutFMIZHGklUaPE0TT5LLowrjuEQWlSi0ZM9gjO4IOBbMIW7N4Hm/xn9a28a/X6O6S4AxnebRzCd0NP5GUKrGbFHRVQXaVHMtEmU5rvmckmy1iZMz6/gv/sV5zEdPMR+u4Cgf3kOKVpGgKFMqng5KrT5OiuymVErZEO1qjJgKkNIRTzZGfO6sGf3AKEMVn1KPjqgak2+dZzSWjgSpJX2OzJ/gdTG6JCPNaOYUlEjEuYYJRCVyeTZGrTDLXzEmj0ohtswxzkhYIsDtzQGO9k7g7372r/DetQ8R5omtp1dkqg7lXZbLta9qXnW70WuwQFFq2jIej9qo20fwbDDBPz1ewYP1DNH8Wb4Z44MKGiXjMQoQswAy4jfpa3iHJqXeQ3rfhp1zCgM3ej3Xkca5P3mzh8dfAZ7wPPYJUg403ObcEWkdGX5Ni6VDbRrVSjOa0OamSQ/DAvjy6Qt8ubyGQdLGmOpHC5Sqfa+ZJaShC0FL8fS31OMSOD43Q/Od41gPuHXlCK5fFMWtI5n0qSBJbVKRtQZ6u6m23EoCdOSaiOQQG6Gp1yfJmMdz9dikwRfBOYXniM4MuxyfqRlVdL+qQtWhRs6tjs5nmf+NzW/2vsvtRfMEB/Xe1MD0Mp/YAPS3LlzHhTOXMR5kVMdUveokw3BaUOlXrchkoJTxIx5kSBjfep+QRBZhNZvgKRXe3efLqKIOAhK+hmVkeW6D2eMkRhhrSrfpeno/AK9e/gNv9/D40fCE57FPEKnVJA05Z9RlfKOKSkKOBBQGCcZUMJoYuk+F8Wx7E589fIDnz5dsIVetDQeRkgiyperGetpnQ1WRMdVRC/l4TDWTUe5so1Vt4/qleXx4+wbmSabusX/a7FrbGa/RWDrtF0VBR9L4nuba3a/qTCpZOsH18Pyx0PNJWAyX/lQwiElwN2/cxK2bt3g8YvwpjpjF+TiF301aLdITQTNM6hBkh2qUdBndJuPr4dIy/nD/Idap/goWLCYpCxf8DjkJLwlDdHgf5SP9booW346dc3oGN/Zt9jp31sPjjcETnsf+wYhKJOUUnao5I1rFpHRVnUVBedTuYtBOcL9PsnvxDM82NqiyJra+XYukhomUkiM89cQ0hWiVdlQh4wJHqew0a8pg4wnOHAvxt5/cws2Lp5FtbTDx7zW5r5pfGnMShAhLToShjhxZXpjTTCt79Nwe18D9VpWjEMcxXWKW3vm399rXBP2S/6Ycp2r5+LFTePutm7hy6S0qv8gGq5cao8ioVPumdZTh880Z8Sl8fEcyXx2FGPLXi/4AX71YMrdKIp10Z0l6HY34sOsTxrGG4Fv86dhex/sb14CH7YC20sGmhV+9yMPjDcATnse+wJGUU3i2fhutpTqNRHWIlIQXlxGqkipvdh4bcYR/fPEUv33+DAMa5Q4NcKUlALTGHVWekRzJTgPMtfCpzLGOq8p0jmQZFBs40p7gX/zsOn75/hXMhxWiwo1TM8O7A2e1HZXJPNPf6WwrZCgjvHFW2LRbGrLgLHbjwXS7Y8mlCnfJTYSXJG5QvCPB5r7Xh0hYM76o56hIr2ZhQTWWJ0h6H9/5BHO9I4haVLosODCajfCkotXLU++nMYkiPeNAKreJVnNnGLcYxhd5ib9/8AT31rf5O8ak3cOEhF2q8bHM0WaZolF4e51F2x40P7W159BJ6Rnp2RkPjzcHT3ge+wMZP5q9HYVHIyv1EdSBqTutBNCKuxjSkD8c9vH75Rd4ONhCZm1IbVQZjbgU3p4qzToozT95LrIjvaAYbKCNDB/eOorfkPBOdCvkG0uYjTUA3BGPM9tTGJHtmmlTeNwzcuHOKM+Rq7OH3dPc9+p2F6o+FJzCU3h3SfDHwRGnwqV9VWFK4WXD3ObbNJV38RoV7gnGEJ+rKmL+qXeonhzq+abw6LgrhTdhwaJupxgnKbYZ71+trOOzhSU8WN3EQIWIOHXDFVS1yetd/L0Me6tvHrZDDdk1xOfh8abhCc9jnyCScsquqdI0paIlb6TatI5dOoPH65v47cJTPBoPsZVEGGpMnqo9I7XBScmp+nJX4Yn46B39mSCkEmmNt3DzUg//8ufXceE4DXp/EUG2jYTKTQpvL2SDnR0282ykZISnn6rS5FbLAmVUTK5K809DZKR7tY3C2IYlOD+nF/xIaFC6VZMyG4tXFSeoQhYGJugms7hz+wNcOHOJ+zOMDxYMqAa1CK71kGWYlPltTk++NGMPBbdZGGBMYu7T360wwReLq/j9o2d4MRjZ8A/wHfRcY39C/zeugcXhq+/Ig3Ydtzuk5854eLwxeMLz2D8Y0Yn4ZABFIGqP02g5LUyaoKABv7+yhs9fLGJd18zOYsRtQWPbTrs84NSN7qcZlTXnLxKZBhRMCpJahuOdFj65fQkf3TyPtN6kRV/HkTaVTlnw8TK5dvMUzgS/dESEpyMaMM5tSdYreZnWmXNXNlfv2VqAmt8Ef6u3pK2Jx+Py88fDzcQSkZysmpSBEuFFAeNtXNswjGuXb+DM8XPoxD0rRLSo8lA59av2PevwIqXGoFJrM15L67iS8fiICq/qztpqCprCbXGQo9DafEnbxiDaRADTV91x05AJ/GkHbDuFzu91Hh5vGp7wPPYNms1Ea7bJ/mueyopGeEjxMKJBrkhoz6kqHlLhLfQHGIVUfZ0OVUiIYV6goLHXunLjcWZ+qbpQ04ZN6gLtmCSVbyLfXMSvP3wbf0vXqQeoB2voRRUSDWonKTrCfRXuiFSijLVNB0bV40gvwvPFJcQ0+hX5Vd027Koda687GgeUpQujtseOHbcqSEFK7y/RjhdR8WY2Vq+G5uhUJx1qScx05tCqIgw2x/jFx7/C33zytwipmDUgvR13rIrTOrLwaoWjZFyqp6Z7HZIZSbBkuDWZdNmewcL2GH94+AwPV9ZRph0EvRlsZRm/F8PPuOHdKMrSSFMrXKit0qptFUj3yi+jee0f9/oeHj8YnvA89g1SJjGNv5EC7a/mMkG7h5xkt05r+PvHT/BkcxvbVC8ULFQgvEmDriN1VKGtpbFtt1NeSUKqa3TSNnppRH04Rqc1xAc3TuPyqQ7m45LqboSoymj4NaRAnU60fdUeOyPdzD1pJGBX6JeUJPUjH6wqVZtyTFebB9N9cw129/Wecs2iqga7/8eCDyczt8TOtqsqYSpJWwk9prKbMaJ769Lb+Oj9TzDTnbfxeu2kyxjTEAlH5C7kahN1bXqCBpVvZyxYRCkLG2083Rri7uIanvcz/k5Z+Oiy8MGreXMQRyRfKk2+X1mV1rHFvat7W3tThU9bD499hCc8j32DlJ2t/k3NoZXEC1Wj0ZCu01je3dzEb0l4i8MRapsnM0RdSFO5GUbKSUWVl0Ezh5RFiSIrEbdIhsUYGK/iaJrhN5+8hdsX53EsrZHUOXWN7hdxqVpSrVaCfps9nhpkR3p2hATgqh9FKjTu3JWyVJXqtFP+d0A+7DrdqzF4qtYUTNHy2I+Dnq43kFJzYVbbnA1PUIcfumaasfOnL5LwfobTx8+5cxOpTnUZUhWyMwE2fRrfN7S2TREf7y24T3Isow6eUy1+9nQR91Y2sK5vMTMPaeusJsHpen4TxaOUnhRjqDlQFSY5vW+zT7cTcd8dgR4efxV4wvPYN9gAbFpJzQdZSTWFCYYkwEeDPv7+8SM8GQ6pJmK0uzMkswQBjXQoIxm0UFChVSSxiYirrJ2yoX/Z1jpiqrlbV+bx4Y2TuHCcSgc5ojKneZfKorGnpaXGmxLdjhm2rdtzlti132mfR0mmNg6PJFDwOVogdfe+vZj617AoIVJXNaZIQOeMQ7/13h8CetIi2Vmv1EaJ8qcIj4SkNju1400KFRESI7tbb7+LM6cu8PIQVclw8IyUnCCSixiX5qiW1elHBK2VIiqqvD79fLI5wBcvVvB4e2grplck8YL35VR1WqRXIXDDNfSJ3PvvkF2z3/y2qzw83iw84XnsE1xHDtpXGkEaZRrVikZ0Ocvx+eoS/mnhCTZIbFXaRhJ3kNKAhzkNMclmEkxIjlQfcUWlVCLhvd2kZ4PWaYFx9liIX310FWfma8xGQ4TFkGRJg07Dr7Xt1BuR9p77jdnVttkXaJWnBCIDbr/4n9ripHoKNeC1HHl9u9uFemiqek9tjM1MK9a+9RdVeK4q0j3ddUoR6UUsJPTSWYz6BSZliPduf4B3brznqjoDLfBKwmsUHv2LSFqJOaplfhhN3F0y3nL6V6c9bNUBvlpaw2fPl/Fkq49WV1OPUTdP2/HE8VrOSQEpqfSaN3xJ4XFfU701xzw83iQ84XnsGxISnBqBND4sidsYklAeLC/h69VlvNAkxVGAEQ2jeiAGRY2Y20j1irSUtOeotMBrlTMR848HJiSjY70Qt66ewTvXT6ET9hFVW0BOwuN9LfUApfG2mf7pN30ivt3smnGekpLIrqKSEdHleYHSCM+1f+3ez63zcA/cMRG7U3i7hPejYY9WODQi0Ck8a3uk1zaeke8pwovDNsYDLRmU4ejccVy5dM0GpqdJl3epGnJKeLxX6i4m4clJ5amyuchJ8vRevTOzMMUi/fnyxTI+ffQUw4rPjiPEndQ6r6i8YRNbM95EeILC02wbkmv2PTzeNDzheewPaPDUs7CigjBLyaS4srGJzx8+wIO1VVSzPeRJjIyGuKLRjUh2Ka+JeV+tv6BGWVO50Ehbe9Motxq+61cu4YN3ruH4HEm0RXVXj0h2JbQMkRZvLfgsKTyyrXss3a7t1Z6jMWGX8CauNyONeEFSFl81nTL+NNwDdK2qM60Nj2Tk1sP7sXBhNbeHPVyI9VJ6DpUW2SpmwUKKbzwqcOzoCbx9/RaOHT/FO0lSdE4VTtvwRHjTak31JLJFZ4OY8U0ll3RQsWCy1B/ji8dP8fDZM/RHQ9d+RzWuqk0+1kivCZbCI0Wn2LLYmB5rnIfHm4QnPI/Xhyw/LZv73/GW2oRUuWadIUgwcq5dqfmtc3RqE+M16n1J/YUtuqfbQ3y1uILlYYZoZtbUg83EQmJTTVnckEZN8qo0mLuNlMa8E9FYlxuYS/p499oRumOYiTPErQKBxuYpTHoen612OCmgONAAd5p6C4/a5ORc2AQRgOhAb6PFX8tJTKWTUOXRuIscGB6HxmzzTiMevhD3HYxK+BfZuDeby3J6PfnhR0IeTB3fQWgG8jdTtulwURaYnZnDTHeOhYIS891jeOftO7h46hILAiTiaRyw+MFQK75Vzdm0NU4QS8HFOsb34n7Q7WHMuHuyvo1Pn61goV9hWKd8Towqoy8sTaT0IyHpKQxVUNn6hkXIgosFV8tC8bkaEzgNt4fHm8JOtvXw+EGYEoJbGJTmkLarpDGTc0v1UI1pZg4aNvIVr3NGX/M9Wo9MGru1/gjRkeMY0iD//cIS/t3jBayTxJL5kyjUI9Oq1dRWRzUXalmewnpItqoYcd3jA7uYnzmKvL+AdLKO/9mvjuM///kRzLZeoB4uoi4zhitESVIsSZZSIVKVMQlM7YGx1uFjeGpKkJKukOK0Kbp4Hd+rVY+NbDUIvo6PYFj08GRhEzO9Dorx9pS6GqOteJDadFWMok4R3ITvr2rFE0fP2LRffCLJlv8znn6UuTeiUlhd31Nxh8iuCkpUYWGujkqAhYGRxupJIbd6yDZrdCdH8G9+/V/gxvnLLCS0UY7VNqnxdx1krQ5yhleTdlu81BmF3gj1hLFf59YrMyOZ5d2j+A9Pt/DbpRKLgzaC8ATmo+OYnfD7qRSjZwYFxnGBfkqXVMgixZDITmsgugKHh8ebhCc8j9eCTJUzV1Qz3HHzWbpCu+sMQtNfVLLCiEgiUUjCozKwhUu534pJIjS2fd6gDhD319bxnMpuHKY0lAlydVDhOflE3UJvqBbor6hEDXhqs6uyAsONVRzp1Pj49gw+ePsoTs7mSNFHSCPttBsJV+pOJGzBkr8iU6oROlXo6SoKxh3/3Z/udB1CjDS1iGxNcqCxVpWkEZtJW3nawA5MnSB9J3KL3cwmepqLLl7Cneay14LiQSpJWdi1J7pvIJXn4supvOYhejbDUjMsVKoRtx+/+xHOHj9tbX0hSS6Mu1SxLYxy9bqc3ks/tJguY8h+K45KxkcWJugHXXy5uIUvnqygnwdI0jmUOQs0WYk0Te35qnqupk6ELIXnVL99DBc0D483BE94Hj8SMmD8n05E4jpOyPySKqqShp7H9YPHKhlRbmXrVH0Y9npYzzI8fP4Cj+i2ByN6RGKQElSfByoY651i1W1yUjIy7Dxk04cNgHwNF0/N4lefvIcbVy+hRyJVZ4uIRtn1VhQp2A3cyPjr+TLiork/D/WyFG0x2KgqGu6S72An/nzW0buqOlXtdzYkQcZ+enzfwXi5ef1duvdw5uQF8lrkxjKGVM8Ma57xWxjhyzG80/hy8a+CAeOEBZeni0u2RuHC1iYGYYAhP/aQ6ljnrKcoSxJJ1TKnAoYGtzsilH8/gXjwOFTwhOfxmqDxJpHs/tkRS1DSG6rqFFXpt+x8pbFaRY68qqiSJtDqPlWng2db27j74gXWhiMayZgXU0HRGEeaCFlkR9KztiXzVbpLj6W5beXoRiOcPRLgvetncPvKWczGVHEjdVQBEvql6kopCRc6oSE7kRa38o3/mrMG/mjMsLYiLPGl2v7yojRn7YAkPEfr3w0RmzqsNIu/at8If+r2E4qXuNXBjbfexbs379h8m6PtscXbbK/LwopmzHRtmHKC4kGhFtnZOMQoxpjvsbC5gS9fLODx9hbGbSr0ThejirEjwqtDJCVJz2pX3Rc01Wffwbz18Hhj8ITn8SPA5GNWcEosIg8aNasyFOHxgGZCUXdBTTklmlGPPkoIlHGExdEYXy8v49HqOsYBjWO3h5KX5wUpLkzplxZ5dSrPFJ49SqRl3V2QTNZx43wXH904izNzJMvRFibjEY9LTajnodTZrlV199LJgNPgfpu9bWjIbZ2aEbk1vTQ1ebTNgykW/zMQqYkwm6WBvl/PzjeFENmgwrGZ07j11nu4dult9JIeimGGMi/Q1tp9DL+LCbmG9ERZjA+6jPEQz81hRDL/7Plz/OH5M2xpirGjR20+VFIcFZ7WNwyQ2kr2Ln2I7FTF6RS3h8ebgyc8jx8BGX2RioP9og1zU1Nxx6owa9R0qtds0eiHaUr5lWBEw/nps2f4amkFy+MMOZVFHakXJFCS8DQbSKPwZJzlu/xsQQOaCx4Z4XgXeO/qPG5cmEM3GCMuNYema5ub5AyDkbBL4k11pgy3qTzbd4qjQbO3s6VfjZITx9m0YlQqNpuIFE7z4t+BVxWeqUUe2291J9hg/0kbkyzCkd5JfPDOx3j7yg2UWYXBxjZ67Q6/rL05r3bh1f8uxpzCGzIyJmnblhN6tLWJz5YXbZYcraVXJh3Gk1R2ZJ2DknJapcm7XbueCh77Hw8ehwue8DxeCw1VNEZQtt+13TWOxFJP239IdraiNg2jZuHfyku82O7jtw8e4QXJLktSZFR46tzXIumFdBSEvHVKdiQ+VzUpXVFKmyDGAO9fn8P7147hRIckOFoj2VWYIZmGmo2FUtGRXcNKu6E1JwL8VoVhb2JOb+GILbD2Rxt4LkIWYX0PtdYovKYNr1F4PwXCUxsetRcVnao2u7h45gpuvHULp46pEwu/FcnMvqeiyeLMQXFibXiMk5rENmScDPh9t6naHw228U9PHuPu6hpy9f5sSSWS7Fl4iZohEPTBOtWYwpt66uHxhuAJz+NHoCEHB/u1h/TsjwetZx+NfU5S61MqLQ9HeLq+hQcra+jTcE66PVKYBi5XiEh+KVUDb+URKTRVTSqh1jSYBaKJqjKHSLn96NY5XDs3w/0+iu1VxHVh479afIZTdy5sLhzycOpewu7v3Tdxhl3qzsEZeU2wIqUnhdO85Z+DCE9E99OqztQb8X1yhk29NicJ462DE0dO4cr5KzgycxTjQWZVws1YRf25GNJ7u/iJ25odp8SA37xWByRuP114jq+WVzAMU35vFlw0ZbdVTbtCi6BvYb1IzScPjzcHT3gerw3Zr6kNIxozSDdVMCUVnqowpewGZYURCaRgyf/Jxhb+h3/4JxRpF2MaRVt7jURHGYSiKKytTAuTZlmGNKFBZipNyHq9hOQ5XMV8XOBf/+o2fnHnLQTFJsrhOo7ypGb6L4ZDKheqKpKrhU8BMZJrquYUYHq4G3AXZnflLnha1ZAbm1u2xhsFHh4/fkL1OX3LVy7/NmhduH6/j+PHj9tirY3iq+mZ9drcT/D9u4z/gIRUjKSaY1w4cxnv3foAF05fslXTNfl0J+kibMXWOzXhN1L4Na5Ps6moA4+Wago7HVQ8p8KLFLsmD/jHuw9Q0v9RK8KYqlhqUJGnadnKmvfRHw+PNw1PeB6vjSm9OQ4xiAXoSDD6i9spRiSwMWXRJO1gm6Tx9eIy7i6v2UTEeRBT+dEIttys/cYhO+RUo9dto7+9iUmVoRtPkG8vYZbE9+H10/jFO5dM6UUTrXGnhU81Lk6hUWB26Uvtdc5N9YQFT1c1pKeju87+52FtBSkz11GDKlVtkjwmheeGJey8+HeiUXc/RQOvYRYahqDxkWWm6uIYJ4+exsWzV3Du5CVEIMFVESISHhkL42FmHXbaJLcw4tUsyCheMvpDfsSY31HjKp9t9fHl80U8WNvAgISH2TlkfMaY14UsBCRU8D+Jal2PQwdPeB6vByMFRwSOBBysuopbEZhK/2MqG5sAut3FyrjEHx49owJYwyhuo9AcjdQWjR8N2amtzs1YQlVUZ2jH/F0NgFGFt8+18a8+uo5bF+YRk+yM6PRE54H5ZeTJ8L08+LpxDNeEBCRnv74bqr7UIHlttebbONeUMfJbVZp/PuvIqEvJiRhEeI3Ca/b3GxoqIiWtP60EoSnQjh89g+tXbuPW9fcxkx6h0uNZkl476vAGvjWDncSMExZkQsUFf4s4a0V4lNqahiujDHeXlvHbR4/xgtdlVIDqyTnUxZFWjSBRSjJ7eLxheMLzeC3IXJsSeslRIU33NavJuCxRBSFAddfngWcb27i/vI5FEl/VmaWyc+PrjITMVxlB9V0v6Sr0BxuYm20jpbobb63i7BHgX3x8A3cuH0OSbZiycxAJTYmIToSn56tjRE1/3CwhzsCaebf2pKZ36bcQjx2iH9wGNNQadyc3HI95yh13qu9PQ6SmatG9hCf8JAiP36plPSVZYCBxqQpY8RIiwfEjZ0h47+H8qStAESMblEjCNpVdByHjt1VXNjB9wsJMi8RlipnqTp2NWrxGU7n1+fuPzxbw9eoqlpgOxmmCMk6RKy4L3sPP0Xx1D483BU94Hq8NmUubdNn29zjaMRFCRmMY9WYxobpb6g9xf2kVy1mBEY1iHnd4n5tP0VFezX2pDHXPFOGRzEh8adrCkGSnLu0/e/csfnbzIo60BohzEZ4IzY0Jk1KUkw8ivErhMgJ2Kk9kKooy4zxVeBbQb4WOM0QKv9QIX0pVeeNxxsOuw4rDnzbYIjWRnabZUrWm/BNEeM3+/oFFjbhl82MqblK1odYBBttjklGM86ev4P13PsGxuVM2KXSdk7xJYkbzWk2Br17borosz4jsLIoZt2qrYwGn7vbwggWEL1eWcW9jHX2qwlavZ0sN2dRjoXpwKhweHm8OnvA8XhuO2F5xOk4nhWXTS6kzQ17gwfNFPFlds04MVbuHAYWc1J2MnnOmnXgnVYPW+eF2ZqaN8Wgb2WiIqxe7+OUHN3FqhsZ1uIKTPfrN6+WHLWRqTvuaN9M938hYKsbkhCMYR3hOzfw5hSGiC8LQiEuENxyNeZRP5e9d0vtufJvC0/YnofD0BhoawMKFlHmoJX4YpGxE9a0VD6KeqTxNP3Zs7iRJqqIrGI2Mcb56O+U7kbRFeBo80tKA/KKy4SQlv0nOb5+R6DU+78vF51jiNyz4DH0ffYp2lH6PGPTw+MvCE57Ha6Ex1zL9zZ/9phWTupOeCliq3x6P8Pj5C9x/+gwr2wNM0i5dDyNNqcJrZPREeI3KaxkxqVqTKiIGxlkfZ87M4hcfv48r50+i3crQwRht0PjyPqlEN/WYFJ5Td84xRPRrx03DZ880dffnk75ISW1wWvxV4/ByzYemEOsfn/Hn8G2E12C/CU8kp/UEbRZtxlxZqQTSsoV4tRp6Npbq6+H9dz/EO7fft+pMrbhgVZoKOuNE86SGeo+KfpT8zVKGrXHICNLisHW3jeXxEF+9WMA9kt76cGjfRqtI6Ho9z8PjTcITnsdrQypHf0pEzcg0oaRRVAl/FLXxYpjj66V1PFrro1/RWKqbe5ToZusAIacivzqYNCsWSLVperJqsIXuZIg7b53AJ7dOYy4eIg1GmOnE6G9v7dzvhkGI4KYkZ/sWFDPOrlem/hzcdaqKdT0wRa5WjSrC5f80ySQkneMPyhkjUv4udYnN9anniSgE99aNn3J2lDe79eZIePwLrfrWxdT0IbpIl+4bSr6QLdbKdyw0hQwD1e3OIEmorMc5hv0xLp6/jBvXb+LI7FHEgSaWTlWjSeWdW2wp5lQ9q++vZY9SllKs1yejpw4TbPOCp5tqu13Di8EIQ/qRxylGJElVZ4skFQ2KzZJxUjA4paUDSyJ2TnOiNvOiKox6rojz+6hsD4+9UA708HgNtNBpd60DQkXjmJDk0oikUJcY0QCqnW4zncU/vtjAf3y6ivVoBuH8KYypkvI+SYuEqNXLJ7w+ageoqOY2i8zafzrdeeRbI3SyAT44N4vf3JzDjZNjzEbLtIzryEVoYYdkAiS83w1LyKmi1B6lMV6VEaYIJrRZPtTGJGUiA0ly1ZpxVDYVjb1VfwZ8bjCmH6URqJvhhQQlharJqKlKNwcFVKNZM6BtHo8C6hg+QzQqb6361IjbzDTP0fgHXfXVx5n5c2hlojy1k2k9QIY3aooH+wWSSBibOJPTsk0qaOR5Ycs6JZrxhsfyLMepE2fwwfsf4dTxs8hHFSZkpTa/7SRJUVABB0mMtJ2qmIB8MESp9MDCziibYPbEeZTtI/gPXz3E//CHL7BAlZgdOYr1iupXcUSetZZBpp08bmHEjzpm9JexK3CIFCOGL56SngoiJcNVML3Z5/Tw+AHwhOfxWpCtKWgMExJXGlH1VKUZS62YPaFCyKIUv3/4FPdWt7DKonu/xZI9lY5WDxepiOwiGS2SRqY5GUOSU5JgMBwiGw5wYq6LI7zo1vljuHaqQ6W3hVa5STHCZ6g7vMa3GamZ/mJ4pBFkubV1VOLa60R0zglOhblqTneXCItWl85+yYryWldF6pSEquiygvep6lRtf3p5PtfZ26nVNcKT/81PqhcZ9LDNu6Tw6KgOTQ+bqpxeuM8QYTfKaW+I9Csm6eW2ZFCCS+ev4PLFt/itWchhuUKL2loc8aaiKvjtMyu8xBHflCRofXJIqCPGmyaCQ2cOq1mFz54u4NHGFqL5o2BZifcw3vi5zEm1qRDC77rbqcfpOJUjXAh5jZyutfMeHt8fnvA8XgtSNxWNXEKi0risoixtADJY6i9pIDeGOT79+j6er6zb8VYUm3FUhaOMmGxsTpJU9aaqCksqv07cRkfKJ99CUG7h0rl5vH/7Ki6cPUMeI/nwuoCl+1oqTh0uTE39NcDANSRgpAwjYplgqzCdGtvvU6XW6XSsh6awd/t92gD/quALGInI8aerGt51Os8iBfJxYUStqs2b12/h5LHTvJbfUt+CfxHJTe9S8YCKEFHM30wTUvrq8DPKcyswxJ0etiiRP/36Hu49WaAyjDFmwSXjtdaRRQWEikRKl/BbB6WqPF1A9J012bRrlzW6s3P7HIMeBxCe8DxeGwGNmuhHRkltOUHSQSudwfq4xNdPnuP5+jaGGqyctnkuMQ7RygnNCgrjIjdVR3OIgve0gwjzaYT2ZGBThn34zgVcv3QS3UQDnSuEgZbYCakoqCYCaz1SMP4CaEznyyZU6tMZ8wn6/QGP0NTKBosE7Yo/DakUEZ78aBTLq9v9gt5UTgbASE4HtW/O/dX8dk4dh4iCFKdPnLMJpk8ePYNxnzJP1ZKMi5gqvyFzFQhETBqoL8WmtQ+thY8ENywnWFjbwv3ny7i/uIJRHKPU1HMaEsFrjPBIpLZ2ntLNNJI1xKQZZqK0pkVkrd32+3wED4898ITn8VpQ9R8L87RwmvuyoIVKEHRmMESExytb+P39J9ZhQYOQWyQ8M34iO5EUU52qFBFrnhT+bCVohx0gyzHeWMZcPMKdt+fxs3fP42ivRj7YMsJL4y7vp+oox/RP1ad/SYvXUMCuk01V25aGJGxt96dVbzqnTiza/mmI1Lrdru1rLs292/0mPMGpOqeY9LL2RopSOy5FzfCnPRZsIvQ3hkjDHt67+SFuvfUuiyhU5iOqN1Vj27s49SVlV/Ebt9SeO6G2U3W32gJJnGVI9R+18XxriL//6i7Wec+QiaiOEz6QpMlkFJNHU0ZRSpVnY/sI1QzsVXiuwnkabg+PHwBPeB6vjSjUlFQ5Chm8OMWAlunJ6hbuPl/B49VNDGnkshaNHa8dkxTV1mOzeqg/O41Vp9exiYhVNTbXnsFkMEC50cdbp7v4r/7zj3DldMpS/xaK4ZYb9Ez/zLbK+EGzrPx4g6eQuHY799sdcZBJVZWmU3hDIzkjOh6b3mnXfRdEak2VZl3T+E+V3l7Ft19woXckp3LD7tb9SY4rvm12FRZiskFhxHfu1EXcuvYulfdNpFR9dVYhH1FxU5EpCkVyIrxAi/xKiZHwahJexvM1yS7ozWOTCu6LhUXcXVvD8/GYhSTGh75vFSBiGkrqAAmf7RSeIzo3nlIHXFWnr9L0eB14wvN4TTjjoxURVF1Vx20sbY/x2aMFPFjeQE41Vth8mSrdT6y9TtWY6p2o2q9qojYfmkYaSJo6hGWJDvdvX5zF3314De+/ddxWNEe2hg4Jsk0VUGhgMxVSlKibCq2mheH1sddgvmw8zfzvVF8a4Q016FzteaIJKTxd9+fxbW14Ijy5/Yb4Q6SiUGnr3trO7LhCCwBOQut1qTk1Nbfmifkz+OXHv8Llc1cx257FRAv28tpAg9d5W8XvqK3Fkap/g8iW7NU8mwX9GdNt8vc/PnxM0lvHOr9r0dJYRao8kl1MslP3HlNwU7Iz0muRQBX7U8L7kZ/f4xDCE57Ha0OVc6IdVUkVYYKlQYa7L1axOMjRYkm+TtrWiWVCwrLOBqFWAApM5Wni4vF4YNWiMX0q+uuYj2v83Ue38Iv3ryAqVlENlhBUfRt3104S5OPcJiqOY/WWdNVoPxaNad+75+B+a45IqbqchGxVmjzmKtWUdfZe/02I1LREUNNJZS/x/RQIz6Ao3InGvfEpFTrBaDS2trwuFXirDrG11kfcauM2Vd61C9dxbPY4f6dU4DFizZ7CAo4KQU3VreMlxtm0WnNEl2ssH9PHF4tLeLy1jQ36r9UUqiBm5NBZb9bdSks5VWu66HZHRdY/kRj0OEDwhOfxndAsITJcJY29DLRmHVFHDquOk9GmMddKCCOSwvIoxxfPlvB0c8ASfGrVVBlL99lU2cVkNs2uXxa59ehU9/VOGiENa9TjTXSDHO++dRof3bqAM3O8ttigsstBrkM27mM8GqDdbiOimizywnoH/jVMnsyp/NVWBJdTfZw8eRoPHvT5zhH5u2PH3Li1Pw3F0+nTpzEYDGxR2+FwaAT4k4AI4xXS0Pu4ikkHDSCPWZgJRPq2gjy/WdJDHKQoRjX+5uNf4/rFG5hJ5zAp3TUiSQ29UMHGrabA78t4sEICv52WgxLpbZPBsrSL3z18gs+fvbAB6RnP5zzfYkFJk0w3k4H/uXj28Pi+8ITn8Z3I89yRG9EoEm3l1AllU+PwjhxHFrdxd3EFj9e3kNOITTo9DGmlKho8VzKXLnLayMH5VWUjBCVJYDLAueMJPrx5FuePxmhl62i3CoQT9QLV+DiqBZsNZdruRYPZjKv7UTALv9fkW2B52DllD5Fepo4Z/CWl1ywNZApvGiffBSm5xgkqLLhZWnTrn773zWBvGBguMqCUuJEeT7mo0X9yLs6bXpvBJOI36uHquRu4euEGNKh+RGWfsLDTaXeQURm6u0SUzgfFmSqwrZpbM/EECdVdC58+fY6vl1ZQzcwx/aRYHY8Rz86isrhW1SYp1AVvGpbm+3h4/DB4wvP4TojsGmWnbaPu5ERD2yWN2OwRLGc1fnf/KR6tbRv5lUlqC4JaN3KR3URrGFAVqRODGUBnwKK6RETCm00z3Lg0hzs3TuHkLG/J+khpz2yeRhk5kt1EhDdtw3FG17Xy/Bg4U+oMqHMNpr9pcLUdjEZGfC44U7KTU9j+BBRvUsmuClP3/4QIT+GRs/ds9qeO8cwvTdccsUvclj/lAs1eU7Vx7cItvPP2B5jvHbexlCFJLI1TKj7NrcnfLKPoOzZthIoIFZY0UwppDWU6Y0tG/cev7+HpaIhRp4NtqsMh48xmU2E824oaLDlp5hx9e/sGSo8WIg+P7w9PeB7fiWbiY20Fq5qaGuyKRqimktPadp8+eYF7y2u2BloeJRipOso6MMicau58TfVlC/dwq/tJM9yoM0pcjXHpVAcf3DqNE7O8ptwg2ZEI6YcZOBk3GV8ek5MBDqgwRHhSen8RvGo5Zf31ZBpdEd329nRIAo+p04oaI/VbRvdPQcsCNYQn7CW8nwZc+BUifSkjPX6Y3V6RindtRX3uKsFih9+gHgeYiY7i4umruHzhOuZmjtmqCppurqOxl3xXLeHkCM+o1fyR6pd6y+lH2DuCLGnj04Xn+Pd372KZaWVydB4r+Rgl4013uSni5JRurMhjzkoRHh4/AJ7wPL4TUiGNslNbnghPxltGXEv/VO0ZfPZkEf949wE2yEXx/HEbhtDPM0SJFJhTdqH10dOcl1J7Mpau1N6iUevSZr3/9ml8ePs0kskaysESehqbpXmnaq1F54jTKbzp9F+1evP9eIX3DUyJzoFbGmVVom5sbtsRq9Lkcacu/vyz1ebYrJQgNPEp7D/x6R30ys07OTozwpv2inTkJ9JzbnqFOTczSoJy2MJMesxWSL9y8TrqsoXh9hAdqnwRnSM8FXT07Z1C1/16bpT2sJ0xbXVm0Gc8/cPjh/jdi2fYTkIMWRgqrKDAtEKi03AFSzN0UogiTfnk4fFD4AnP4zuxU31ZlkZ4MtYy4LZkDk3Xi80BPnv0DI9WNkh+XbS6MxjTChUy5jbWTkZOEzI7hecMniDdRoKsS1w+08WdWxdw7iTVELYRtUZIeK9UAjUmjRqdjNuU8GTmTOGR8P5iCs/gQua2bl+zuuhVGoWnfSMHc/zxZyDC2+3ow7sYf031sOJ1/9G8y5SGjOD0js2+Iz171z1biwV+lCTooBjqK7Vx6dxV3Lh+G8eOnLBzWjfPEZ5zNpTAig+OOPXMmIS3tjXEUH4dP46lMsc/3L+Hr5cXkSfq4NJUaTp1ZwpvGl6RnkLi4fFD4AnP4zvxahWca4tqWWeWzf4Af/z6Hh4tr6GK2zbLynZWWO+6KE1t9pUdhUeys1K+mSoH2c2zJ0/gX/zNz3D1ApUhlV0SUPGlmpR6bNOITSYiPKo9M7xUGVNjqeo0q9Lc8e0vCyNY+m3zaPKR6l2paGgWftVxu+jPPL8hvKbg8FMivCltmdM//d5LdvZn8c4j0987W3MstFCBhxMpuQS9zhwuX7iK926/hzOnzqBkGlG7nVVrTsnPvOKd7n71gGUcRAkLSS2MGS9lp42H6yv47b27eLG1udNpxZYRojMinca7hdvD4wfCE96hhoyuiEQmSEaEBkaqiU6GReubRSQ5a4OiQSrCCH0S0HIBPB0U+MOTZazmvG7uGA1XjMFgm94V6LB0XpHw5Kdm0KjU5iWS4LOCSY54MkR7solb57v4zYeXcXq2hc2lx3x6SQUZYUjCi9upIx7ZNRm6nT+FlX+yntz+GDQawa2CwPDRz5ZVv2YMi+aHIWVP+F55iJyntSyNnmttkqZxnflvYKaYB8ww8y+2teFCF8ypwmva834aCs+heQcX7j1b2+d/doF+OeibyBUTFmo0xpLnuYsTsyfxztV3cfn0FbJZaIQoJW7pas/9zt8Wsv4Ix+dP8pt3sDnIbFB6n9/i7tIK7i2tkQQTxjuVXiuaph/dqcC4DkwKg+oMXEWpc3ZNE8BX3RQ7r+Rx6OByn8chhMzEtMs/s78zSlJNMUlJ68fROJclytEQcchkkiRY57Gt3jweh138P798ghetGQzbRzGsSQ8s0c8mAWYDktp4iIT+5QUNZzKPEdoY0+/uTAetagtpuYgPryT4P/+vPsTx+iEmaw9xqqdODiHGWlWbJf2MYauC0gxbU4UpVadwaj27OiAhmTV+PTiyE+loLzEnQxq0+L7YpBtaB4zO7Bn809dLKNqzKNIuabBCNyb/qy6vLmnsp8RFg2pEwTiioLUR+edOncXm6gZmujN8is6bOeY9jOmYnuwzdhQc0YSvUVOuk4irTnS9YqekInVKp/UERxgg7DEumGyy7SGiLMJbJ67jo7d+jncufYQxyz+zMycRJF0Mc5Kj3pn3ZlmONpVdL0hRbWdosTQRtRi3VYw6nccGt//2j1/j7so2tnlNa/aIDUwfaL3EVsWCWMWo13yqWkVBYaEzNTgtsLHI4oZOTNuKLdYJe4dd7N33OBzwhHeI4bJ/ozRUCmZymBo3GT8N7e51OshIfIOC5Dg7h8VxhT8srOAFyWzAEnmmEriUD+9XxwTXSYG+0Y+IJfdxoQmF6R+VzWiwgUk+wLVzM/jPf/k2upMVdCYbSCYj3iPFpBBJDcqoOmPsNJTCI6XkBps3nSjcuR8Dd39TdarfrldpTqcqWS1zE2NcU2kgNe3H1yKplaRvpyvc/W5rZpW7jEF7/yR2i6haA6AUHv+k7HS9Zpv5qUHhc9/+Fcc/93Z6Y+f0fbSoYRVIyTMN8d+EfBSWMU4fOY87tz7B6WMXsL42wPb2GEmnx7isrS24nbKAwTTlenGKZOk1050KXKQz5K2U6SrFZ0+f49n2AFt8XkblX4rYeFXArZalUoh068vYjdcmrIJ7h+k72t7eKz0OCzzhHWoo85t5pmHbMWU8rvYmmTEaJE3pxXMsh9s6d89W1/Hlo6fo22KvNDdT4jGSMhqQEwFqZpXYZtOPaNFSlson2Rbm28B7b5/Hzz+4RR8lhX5KkAncYw75TiUNs9SrpjQTptxm1ZN7sUN6PN5UXWqlhKbTSuN2CO+V+w8ejKX4PurMRO4juWvIRskCzuzMEbx97SZu3niXl4Q8ppUuUipfsaKbdScvRkw/UuqUwlSLLaYjJScmHks7mpHlq8dPcW9xGcvjMTKtp6i2YX4X1QaLiB1hkgDldCPvdIUhpUc5t+9Cqi+qdN7cOz3ocajgCe/QQga3UXTTXzRGMhyiLpoLFDRmQy39Q0ODdheLW33cXVjEyjC39jwZFDcoXFer1K/emzRMdFJN6mkZszTe1Xi7eoT5pMad6yfozqAXSUX9lAivIaBdFSOXU9mqCq60uSF5jMZVrunAsxc6rmvUqUXner0ZmwpNPTSb+0R4DUkcdNST0lbLENrtDt+VhaO8IsGp/bKLd27dIfHdwkx3zuZB1SurdrwuMsYFC0yhVrpXtbXrwWstfUyPNR2LSVhhvH+9vIyv6DZ4bsL4LEl8Y36TmuQp0mp6ge4sc6TrlH61QDAvaGoKHLvpOqVzOdGfx2GDJ7xDCxlvKTElAZV8ZTBUylbVoiMxRBG21fbSnUGhwcGPnuGrhSVkcccmi3aGRNfSiNHIq3NB1aKBp1NndRXFuxq8XmXAaAOXTrbxL372Nt65fATDtcc0VBpm8FNAYyodye2MTaPTtGLjjMZZBlLHp6TlwCteykHuGhGehjT0GG8hCwa8ZeecIzwpGplbO3FAQaKJWGQR6aktk+8XqGdtrYIO37MMceLYWXx05xc4e+oCRoPMyCZWb8x8RJXH5BHkdGqXK6zwo1lZRESq2lRnlTzt4d76Jn737BkeDQYYJQkKKsmS6balGgRerx6gpvAsklUAk6pjHE+dKTw9eMpu2ojwLOk2Bz0ODTzhHVaYYdHnV9sYjTH3jPCmRkJd1FvtFBkN1DaNw9ONLXz++BlebI9Ifh2MZbzNkKhVRdfrt9rf6OQnfVP7TMrzUdnHPA3cu1dO4qMbZ3F2jk/NN/j0/SW8xtyZ7eOvXaKj1iBpqQYuy6nwSHqiuGbC6F3Ca+B8UDzKH6fwQpswWvtuDF5g1Zu6Vh1djO8OONJ2jEADxIuCyo5xxGgJWNiJwg7CVgeTKsbF81dx5dI1zFLlaU1DIxteGCnZNQrPVVS6c0Z9AXJemyUplssKX66u4dPnL/B4cwvjIELQ7vJBsWLaiM5WP7dvoJToCG7XMT1P/+zr6hn2HOc8Dhc84R1qmNWR2ZhmfmcYRGQlU8aIVrlMO3iwso7/9MVdLGyPkavHHUvgpUrzshw0VLLzMieuQlR+qnfcBAmNjXozdls5ye4Y/ub9qzjRqVBvL+BIWwZuHwlPQZ9udw2fIz1lC5GeCE8rIxQ0uno/qTW7mu+mVb6dSnMwm2twZCeCiyL1/GwITwrom9WgBxcTxk/F6GBMBYw3i5MWoiBlClB7HdMB1V4a9XD9yg3cvH4b7ahtc2xqbcNJpZoBN11cS214TDmKGflhc/O0NA0ByXRmFmvc/vbJE/zT/Ue2KofGfRbWyeWVuGRYXGGtUXj6rRO6kh9o+q3N7Xwvj8MET3iHFsr2zrBrv8n/juxayKlmtmnxi7YjvN/ff4w+jUw4M49MY8kSEh6vb+5s1JFgtEcySyYZgmwLR5ISd66fxfvXziKisuuvPMFMYpRi1+8nnLZ1ey78Lk7UcaKi8S1JVtZ8p3MkMbvOXnmvxdz1w5wpZqoZGm3tU9AZAaqa093vCPBAg8HP8jG3E0RMC7aMkL1fQMVXIxuVRn6aauz82UskvFskui7qfII0JGFRESoOpcAs5hmdjoT4i3Glnr9Dxls0fwRlt4eHaxv4/NlzLI8LjMKUaZPfhQWLnSWEGJ/mk30CebTXCVPS2/ntcRjhCe/QooUxS8tJktKw1xiORgjiCEHqJn/OqGbC+eP47b3H+N39J8iSDup216YOky4reI+MTElSlIKxAdUiy2LM0vsYnahGLyyQVH3cuXaG6u4a0mqAerSBI73IrnEG6KcAM7nTreharoU4aePhw8d2LrH17EZWTamu9W5SaFedpzXuokgddeiFCI7u2LETPB+5aj4abxFekVPNMM5iEoTuO+jYO4henXqsc45IiupfywRpUdi6YOxR7b199RZ++cmv0IlnsL68hbnuEYsrxboKAIpTbUPFK9NhxXjqzM9jnXG+keUIZ7nP+Pt3f/wcnz17gdmzF5GxQDFQnMZUlfwu49x1Lkq18DD9aqo7d9OZ204/k8chhCe8QwwNfi4KdTgA4jS1OTBHZYVCC3WmXdynsnu2PcI6S+kDRDYxdEFDJGPBjVmNRMZGHRHUm5OqrtsO0Q5LVMMV5JvPcfV0B+9cOYnjnQnarYxEOEGiKjCTTY0h2h84RSHskp0zj9N9vqTa8PRbWUWk9U3QyDPe+M/Oayu/tPp346+5b1jY/X33vxT45vb38vtwf3qoVTPOKsZHK8Xp4+fw9pVbOD5/BuN+CdIiU1WMUG17LBSotc1WS1d1cagqZca8Cl4pC1uMz8EkMIX3lGny0UYf6M2h5PEtHsuo+OJ2xwoZak+knrbvqy/afFX3GRhabV3wPA4ZPOEdUiizJ1RzGUvFMgCaysuNtwus3W6TxuX3jxbwYHXbCE/zjxQ0TJpP0nUwkA5q2Vg7rYgtAlM1Zjus0A6GaOWrONED/ubOZXx067wNQ5jkmjhKT1bHjml72E8GCsvUPDbVZNwfjKhYuaXtNXU20Y72efWrVZN23s6F6Hb58oxDl8V+Su/5l4G+otzuu+mX6m7ppI+51bi7UHFSavhAjHOnLuGDdzV36k0EZYpWEaFVkppqqjqlB6ngmoUlzb3KQlFdllSJAdVigiBMMCJxLg0z3FvbwKcLLzDgPWXcsarPYTlBxEJai4RXZFo4OOAzWxri51QenYWQwa1IeuJhd8TjMMET3iGHStVB5KqQcub/CUvJI5LYw5VNfP1iDc8HOfoTGhaWpGu10dBY8Go6ER5JgUZIdiOJNJ1TgXK8QWO2hWNd4NcfXcAv71yx4QitfAv5cBtlkbPkzjujjp48DcX+wak8GW1pALkp6dHpDbe2B478eJ16aTZX8wL7T9V6TuHxbqkUESKvmJ2d51ZW1fm96wS7+Z8BFE98J3s1RZDITh1ZpN4rFg5KpgvNJ6qhChN0kzlcIdndeOsOLp25jjBn8WfMNKhqTxaaNEiv1l9d0DumMTmqPVsaSCmO6XKb+4+3h/j0+RILYxvIY6at7qx1cqnpNDRCrEZd6IYtMFiu96eUneuQVfGASG/nc3gcGnjCO7SYWDVkSCMThCFL1hPkNOxlnGJ1XOBTqrulYYmtimSXdDFJOkwtNCI0EiGJLaQxC6n68kwleaCtZYN4POuvoBuNcOvKHP7uZzdw6VQbcd2nDRzwXhozqp6cftatVHQxDctPB7ukJ2sYYJuEJ8voFN6U+KaqTiTX9FB0Y+ua8wHm5kR4Aq/dIb294IUHGnsLB9rXG0lOaRynhhkwUTBhxCwIJaFm3KlRkNy6yRFcOX8TH9z6BWbjo4jKBGVGjlIcMR1KFJOOSHjq9ESyK0oEWYGA3oVMg5O0h+VqgnubW/jdwydYo5oLenOoNc0dlWTNwlRM0rPlhBgoR3gudPozhSfCoxM9exwueMI7xKgqkhbVnWxNSaOgRV23aFweLa7i/vNla7fLaUjqmIaGRgs07qrOjGiMWpWba5K850rUZEIt9JrQMp070cNH713B5bMziKotlMNVpCxud9opn5fQqNE4Vc5Y7idepiAZbR1xrjHkW1QTZtBlOKedNBq48XQ65xReSKJriG92dpZXNE94+Ukyvi9vDyqa+BL4LiaXRXpTZ5TC2FMCIxHlo5rk1sJ87xRV3ru4eu4ajs0c53kpY16qaVhUohLhMWHFNQmTaTTQPK4V9bZmrUnaGHG7Vtb4auE5HiwuWZqteSwnEWry6DhMrCrTJkBnScUpPGGq8BQcHXAHPQ4RPOEdYshY0za7abNobCYsYT9fXsXdJ8+wSZVXajb7MLUZVAoaDTPmLLUHtUrchbXRqJOAGf1Siq/CyWMd3Lx2Hh/cvoJeXKIcr2NSDEl4vL+sTOEFSY+lcSW9/Ut+u7aOe69UOzbEp04roxH39ii3vZBiaAhPsH0ZbqLb6bmdf7ZQHO3dCg3JuSrNFplGvVGtVyvTkYYpaBaW1iTBbPcY7tz+EOdOXUCiQhW/wYQFCi03JOK0GX/KnAUmkR7TmYYhkLwqXpNHsc2tuTYa4/MHj/CAxKfiV4vKTuP4XFW7I7qG7IyLuWPVmlJ33OrP43DBE94BhsuyezKt7TqDTWpipm5Z25yb2Nkd13llfumXSI0crRIZjYumDhuTiB5tjnB/ZRtDjZWi4lNJeKJeByI4zZtIA6SOG7RBdFR1NDyRjEfeRxtDnD8S4/rZOVw9M4e0lbFkPkYYqvqoxvawj8KqUeWpjOL+GRw92Q1MVkudxg26Tjea/UXHtNxMiRRDdT4llUvNBbbiOuNBcSkSVHxOCc9W+OZvKQopi06c0tiahd153l534MGXcCspuK1Ds3WQIi5ZENLE20kc2xAYJahSPV8ps65euYETR8+wMNQloYV0jD8ViOQv/VdvTcVvK6ICpNcaAlOSyCz1Rin6cRtfqIC2vI6hFgSOO6RZfSs3aYCl+0B5wO3reymsVt1J50LvcZjgCe+AwqiOhtlNnaRStcAMzAytjC2DrV6VGsBbcF9rhinT86QZFBn4NCqRl32U7QTbNBZ/v7CO/++DZTzIQow68276MFuwdYROa0wCk4qTBwkQdVGGMc0/S+BBgahYx/Eww9+9cx6/fuccqo2nJMEtpG11NghIqjR6vTZC3lqVW0hCTSYs0tsfqKRfBiS2QETHd6BqTSdDvt+I8VQhp3J9vDTEJO5iTFWSJBHKYpNqhKqV1tTmC6UizlUtzPiVAY1pWTs0vGkdYD7t8ZiMt7KY+yaMdnP/HKDXsIITX0hbIz97ORUElErk1D4cWWce1SJULDCp6KWaSxUSAqq+mzc/wNWLN6wDS1rE6LU6CKj+rRNLkiCLA4y4m0f0l/Et9ZZoCGMdIT9yEs/5ff7jwwX8u8/vYW1AJZnOMr0pzfHb8EFjuoxO+5SPiPh9koLbTIWTfyYfw+N7wxPeAYYzoLI29suOCaY8ZGBtX9c4gyvof1fNM8EwHyJok7RolB6tbuCrxTWsVSyVt2cwonGQOqRloemiUZf6sao73Snlp3aXEp2U14/XqYXGeP/aCdy6eAxdkkY31D2qaBIpT0vYDKvIWdpJHRv2hvlNw8LEiFCY9EYivYDh1Tz9jcIb0fAWWhxWXeYJC7N6IPJeGXMrQGif8dKou4iv1GZBQETQVKfZ/3bPPy/o3Vzya9TSd7m9cPGt+/J8giPzJ3HnnQ9x4fRF9Ff7YMkIR2eOYjTMGb8sqNGVLDC5Aht9U1wrIhm/W1R75cwRjFlYe7BEpbfwAv2Shb+04yZI0L18mlyTJ0SZiQolgebi9Dhs8IR34KHMPDWmIpSpAbdqOm1VklZ1nG2dodHVoh0WiFEkXWzkJb58+Bj3Hj1GVlDRsWS9O0Gy8805+Uiy41aIWjnaQYaoHuLMsQ5+/tE7uHblHFrqVk4y3HnejnNm0eEnZG7EfhaeXcMoNxyNTc1ZGx5/20BoklzTbmcqhcZYaI5p225r0uif0Pv9RKGlgNIoxdvXbuDOex9gpjNjvTljKr+QfzrffAsDN02hSQmrLAq0Ox0jw4cvXuDThw/wbGsTI/6WohNZ6ttFTLtNNaYKIZrwOqBi9Dh88F/9wMIZAhliczIGe46IbNzq43K7ZOdMB0u9ARVcZw6rBVg6XsN9lo7Xtvv0hwYi1pI2DeEJ8tFVU4nstMirjqiNbjJaxsnZEB+9cxU3rpzBbDug0slQl5o67KWWQ/dnAXX7+w5F2DfAEIu86PqDgbU/2fg7HeNZvY3rxOJubghPW3XO0Dkt/OoJ709D3z8JExRjrR4f4/bb7+CTj36BbjqD7c0Buu0ZI7yG9FyqdgU6R3pMjUFkhRDVIwz4+8nWBj5beIqn21uYdLpWtWmrN9gf/eK16mhVM23rfo/DB094BxkvlYDlGpNMIy2io0qLVM1G1wzAFVQ1pPa9nIR3b22AP2rMXX+kqVcQqmMBoXYp5ysNDZ9jZMdjbgkgnauR1NsIRpu4eekofvXR2zjWIZEO1mx1c1dFKDclWpHcVEnpT2i2+wULTWP3LGxOybn31aBzdbLhO4jweN2u6uVPvlej8Bpl54YktDAzMzO9yuM7ofiuNRtKjNF2htnOEfzi47/B9Ss3UWUiJpGdVJ7ifm864TmrVq6RMq1qrUKmXERHj2AzauH3z57gs+fPMYpiFBqETsILRXi1OsXwQhZKtACvZnOxj+pxqOAJ70CDhsBIby9EMDLXjcrTtiE70ZSr6snDGEs0LF8sreNrKrwBDUvcm7WOGKrG0/yYzsw4ItBWZNBA7V3pZBuXTgAf3zqP6+fmEZZbqMcb0/kyFYZvV3i75PdTQhNSvivjR6Hf7g+N8JzCE7FNr5R644+G6AQRX0OIXuH9eSgtlFmJGRa6olaCbFjg5NEzrj3vzCUjPU055khPBSwXny8pPH6TsqxJbPxmLGSM2ikeD/q2QrpWVxiwZFfofs3XyT+lOX1DkZ2cp7vDB094BxbOADT/mwkwuSKzrTa73apMOUHVdBrHVAQRRmGCz5+v4N5qHytZjVKzWMQJcpaANQOLVdXxNiOnPU8x9QhNLZbhaLvGbz66jE9un0Mb2wiLTXTC0qo0W5PcwtCoOz7cnPNr6n4SFseFxWKO8aOOKI6mNY9mBvWBYGS4c3qX6Z+RnZ3aVXjN1rfhfU+QkDS5dKpp5gqgGFW4dPYKPnjnIxyfO8lzMdMSldmU9BqoU5ESVp7nLJhRxVHNbVUlNvnVBmmKZ6Mx/uHufWyWmmMT/IYqwNCPFgsu+l4Bv5W885/o0MET3gHGN5XSXtKbuqmRlgEvVY1JshuHJLwoobJbx4tRaeuLVbEmj24hK9TtnqqGvug+B/kjElXPyxzRJEM8GeJ4F/ibD67i+vl5lP0lhNUAvYR2rBiiKkh6FoYGzd6rYf4pgOExVdfEmtOmWvy1UlhlKHVc5GfxyY0KEYrqPbBjRBzTCHv8aTBeI5LVYFtDQSK0kx7Gg9zWzLt2+QYukvicwpNT9aZS4950Q8IbZ25sHwlvfTgkwVG1zc5ii4r806cL2CKhag3HMZ+gyRPUpjfht3QD3PW9PQ4bPOEdUIjI3BgoOXdMttms8JT0ZIDdsisBchrrOm6j7sxQ0RX4w8Mn+GphCdsaK9abQ05DkKnNL9EA4WQ6YLhkqVje1ahLDSIfkejG6LZGmI0y/Df/+/8Sc0mBzaUHmE1rzLSBKh/wvwJxKJIQFKjGUDWE8hMAA6FlabSVUoi0CjfDJpJL1eGB+y+WltHtzWBIpVdope52xw2iZvwobrUm3pCG1ryzuHZq7/jx417hfQ+I8DQNWMnEGZDYeu1Z5FR5adjB3/7yN5jvHUM76poKVJObiE/VmIpnVbknscZG5iiYTmN+s6DbQ5/pfL2oMaDf/93/59/ieX+IaP4oxiS77bxA1O0i4nfctDlSPQ4bPOEdYLixXnsNa6PwuGc8w/9oHNSzsgoS5DQu2zQajzb7+CNLwKrWlOJTBxa167mqPLvROFOlZ03+K7WmeTK1SnkrW8eRJMe/+vktHO3UmE20HFBB5Tcm0ZEUNRzBHuvqjIzgpJ6mCsowJeT9xDQkTUS5fWYHqTupOhGcCgo25dX0PRpIyOlXo+gaiOT03u7dPf40WqhZeNBiuRaN3Kq9LmyJBNtGeu/dvoOjs8cxKTRRQow0Tt0sP3nJ0pu+lPsygvbUoUpKLg9iS9ur9P8PT5/h02cLGLDg15qZRb8oMabrcX/vN/U4HPA584DCzDTzunVIcYcs68upF6YZABluDc6lIahpLGQEXoxyfLWyTiPwggQoA8HrbFDvt2R+GhW3llhh69zNxDQUrQJXTsT413/zDo52YVWYcUvtdgWvL2m81L5C40W/rWpwSqRGphauJpR0U3LeL4iwRFJyap9TnGk9NdpJjLMS6oNiBpnv0YzFM7KTm8Z6o+SaDisRCwhSHx5/HjZHECPU4t7iVYSXGNm14xnceecjXDh9iSqvx3QeuvY8qj014Sndq+ZBKSsU+Rlh6htR9ZH0Mqb1TR777eMn+I/37+F5nqHsaO28CUY5VWTKxMu7PQ4XPOEdYLhMP3XTYw2piPTU8XpMpWKExwy+wf2vllbw9eoGNvjpC5Z6bcyZu9N5ZL+cK1RdREPRjgOkAUkt38SVUwF+fusMLp2gYWrliIKKPmkuCxl8GiAjABGIjjjFZNvpc5wC1bXa7i+aakiF2ak1kR/fhoUELfwq4pMy1TGXVZpYdtuGMIVmSIKqO9WG96r68/gmLO7DafwzTlW4sKEKJL2EKu/Y3AlcvXgNl85dJo3FGA/GiEhm3aRD0rPUpJTGfKChN7Cp3ESalam8CKOkjcWyxBerq/h8aQnLWcZ80EYr0sLHmp/M47DBE94BRUNyZoanttW6a9OJ7KTYWpGbU7CKYmQ0AE/Wt/D7R0/xaHuI1vwRU3cyGg0BmZrjnpxBflUF0phH8j4moxrvv3USv37/IqJshYw6QlUWNq5J99t4NRoxdXkRaaiKyapK+RwXpt0wWjF9n7Gr8EjIFoeqkmThgMZUs6wUJDFTH4zlhth03V4y+ybhpb7TyvcB04BWRnDOFSqsU5AlFE1ewLST1bh66S28d/t9zPeOoBipJiGwAeuagFpkJ5UX8XuI8DSXqUhPPTKtJ3ISo5idxTKv0/i8r5eXULfbSGfmMMpypXgXFo9DA094BxhWnUk3NcX8c9TVkEsQJwhZolUVz/P1TXz15DkeLq9hnfxUd3pWlak7XPnakZ5zDjGJMtakvZVWSahx+ghw5/o5XDvTQ5itA+XYyG4yrc7bGa/GZKXu/I3Csy79egofZ08Q6bk9u2+/IOJq2tzM2DKMegfN0j8Yjoz4RHim8OT0DlOyM1XCfUeYu/txHFlnFo8/jSYdMEoZtfpPccg0w7RpY8JJXFoJ/ejsMVy7fB3XrlzH8SPHmd5FhExzTI+8g65ReHKO8MBrlOYHPFv0uuhHIR6sr+Hr5wtY43edKJ2SFD0OHzzhHVCIqsxemOWwQ9ODbkemQEMMRHr9cY6vHjyyde5G6uLdm8XQCKm5Uq7Z55bMpD+pFhnwMhvhyEwLv/75DZs+TB1XOkGGlKVz9ZoTTAnJ6T7e7zqqcJ8eSuGJ7Bz2BnifwQAHDJsRlv3x3RVmvkym6i+qVPdOAq/Rj1eg6wWRpCO82Ajv2671eBnqXWkDwHfShuLYPovtJ1GKfFzaNGPvvfM+bt24jYjKbTzM0GFBTl9GFeYab6qVrly1pjpyqVYhsBX8R/wmAyrIbV77dG0NXz18iOXVdaR2/86DPQ4JPOEdUIhYNIhcVYbKuJo6TFOJqQFfqFqhTQrdDxMsFRW+WlzFs80+gu4sujPzViWkqiDNtSnqq7VCAEvabv08NytFOeyj28oRFyNcOBbgNz+7gXMnOthYWURKo64enJGqo6Ti+Fj5JBOkak1HnfqfR3hSTvvOxOj//TY2fD6VgAuHJEXJ0OlnTMJroyhiGmN1vOFlgVZ80DXc14vyHgu93sv25Rf/5/VBK+GxmPuMB3O60nzRHVPoWOMOKyaoSnUMIuFZnDrsqGnGXRJ3sLXRZ4Grtra8G1dvIQ06qMYTzPeOWhy7VMX7pyW/FtNxk+YipsOSz7CvlHSwNirx+dNF3F/fRt6Z3e20RWcpU9/Kvpf7rbyg8yXzxd7r3FAgXWtB9jhAYMryOIhQZhwzw1ZpipDE02LGTul6zORxEGJYkfxOnsbvV9bx//r6IR4WE9Tzx1HQoBeDMdKChoHbuORWC7zGzMS9jltNuqJSIVGe7qYYPXuGn70F/B//qw9xaqaP7dXH6JI0J60OJiVJoBmkro4xoOMWdOqp6KqaKhKryLU0MrZhFCICJj0zMvuGAGGQoshpcJGhnAwRkMSLMkG7cx5Pnw3Rp4Gsrfar4PmRXsuqPKUApWwT9UTl+2vVoF6qMWTApXPXuJ0gjXt8u4ZQhV2j7I7t57v/FKA0FiFipO50vmKa5n+WltCiSmb8RREJriDZ5AGuXbyJf/Ob/xK3r7yP1YVN+YAwSTAscwzKMcKuCCzHaLSBNkuAbX6bDj9Pm9cFrTYGkxSPx8Af+yV+tznEqNMzBThiXpEiVJqo1MlL9fHMUwWDkUXAmC6LWsw7RqXWjugXkD2Y8IR3QOF4o2ULkGqWfhlfmVfNPpFpwmNm5uWsxMONbTzpD7FFI1ImKQ0372MG75DYujFNQRgzE7sB15pWrNL5CUmMBqQaruKdyyl+dusczs630G6NqOw0tyRLv1MlKRMgyA8ZA+ecIXDlYZGca2dprnVkp6S33wZjSkhayXyq4GqqtLpmPFUJCVwzcyiuVe0mYlT4dcCFXEMwtDXDJ3XH68NACo8WUupRNxt0X+M8BBdvckwXr0SLSz9Mk6qFiBKmURZMqPKCCQthx8/j/OlLmOsdc4UVxnGn10OcxhgXY95bIU1YANTkB/RX1ZwtkpO+65huNZ/gLhXeHxZeMH8ULDC2SZQzYHnQ5pDVhNSddhs184K+t1bFd9Xy/MVtE1RPdgcTnvAOKNSjkoVOTIqMhFdQfWjKpAgjMtpYDfdpF89W1vBkcQnL65tGZoGuUcaVFlPHijhBK075i6qPNp/FY6S6VSuYo8/tEB+9fx0/++A9HJ2Zs5K4qjChdc5rFpWtEvPgQ2PBGqh6TW1LeTHttu4Yb2rpvmnkmuo4qROpWrXfaevb8P4cFD9TZz12m/3mOFMZv0HMNCrSE7mFVH1nTp3F1SvXcfH8JRovKjIW1BIW2kRUpRIxb02Tts2II6hgJiKz4ha/S879pbV1fPXwMR4sLmNb5Zx2FzkLP+qVq6Wx1FGrLjSFnlbCr6yWQlX/pE3LP8peVZMkPA4UPOEdUKhkrFXIY7KQ9pWp65DqojOHKu5igyXirx4vYHl7qEWk3crPyrRa/FJtEszMOX9rlpVJK2VCoGHh/6K/dmtIt4VrF+fw7o1zOH/qmDQLJiUzPDN/WY74xJxHDjrhTU3W1HjJmYGkQtY4LYtZVWOJ57ht0Oy9Smrq7alOK57wviemhS9Tzqagmq/gnKJcBYlmHKTUXEIyO3XyLN6+fgsnj58iMVUYDUaWFJvOVlGoTkMqyPB70qlWQ23dAUmxlSaWctd4zx8fPsGDlQ1sifQ6XZs8PSPJFlmGsGbeYlp3jkqTWxGeyFlkJ7eTEDwODDzhHVCI5FRt06bMk+gaKaNqscu5YxgGbTxc3sLdhRUMWByNqM4qGuGMmVYXa7XnMUuwI5Zox6ruIdmFrTYinm5lA6T1BuaTAX798WVcPJki1PCDcTa1HCRNdWSJXeY/sOCryF7ZzDDaN0JjrHJf1bvj6TitXWenp3tT9wqpiegawvP40xDJiexM0TEd2QoI06NN/CouVbWoKchU9V6TZUZDjQvt4q0rb+P6W29zv40hC3VaakhKz1Sf2qVFkuo8xa34TAW+KgjRIunVJMSM+18vruCzhUU83RqgosoLezM27VhOwuuoUxbzSzIlPqk9Vc0rfBULmRW9P8Cp/9DCE94BhRneOid/MQvSWGvV55HGHk1iPN/O8PmTRSzROIxJgoFKrzTCUnhmUGi41R28Vi82eVOHNoNFQNJsZVskuwzXz6f45J2zONrhdeNtkqsyPI06n5XETDgtmRAZpoMMUVkDsZk6ucPWwDPC4293xe5VQnPkVRW3l/C8wvseMFU3TZP6X6TXqDxGsBRzpRUQ1EmIZKa6xNGABS+m16Pzx3H9yg2bhaWTdK06nrFPFxpBuqnfAqvqZwlP/YqQ85uotkPDFcatEOu85+vldXz1YgVreWUrhpR6JguCqcZTchubc1WaljoYvmrqXkkWHgcAnvAOLJjhSFplXdhwgjppY4QITzYH+JLK7svnKyhoCIb8xAMaDZVuVeKtRXR0asMLNMaORkAGJZ60mLEL9Foku7Nt/N3HV3BqvuLxTZqR3MbcSd1po5lXikKzzf8zK+PaK9KY8T0Lqjyn7BzpuU42L6MhtWYrA+3b8L4/FEMiO2tXbohObrpv6o7fQ1WZAYlMvTdbTOMTEp9WRD91/Bzev/0h3rp0nUdjFso0PCeSnLPrNY4yDGJbCQNM/+qYktE/kVoRJSi7c3gxrPA5Vd7XL5ZIesxLvDZIEksDGupjA9r5LUV+aje3cDGdqB1PvzwOFjzhHWTw62VlQXXHnXYPg1aCpxtDPFjdwsIWqW5mDkNm+gHVikqkQUSVRyOiEmyc0DCT8KynoZZXoZXoBjmOphXeOtPDL96/hKhcR1BuoR3VSEI15GuCaPpDv8YjEd7BzvKugL77Do2a2+3osJfstHV32D3f8urNrC2uetTje2Enqhihe0mPTnO5MoHyEqY9pTuSWBp3uI2QDZle01lbO+/K+WtIo64tIutWSJfjPUz7qtoMqboDOs2woqn2JmrjS1JMOrPYJHs9XNvE3cVVLA3HridzFGNUTMfv6enToDXpxXX8cn8eBwue8A4o1JEiSFOrnsmZgcdhgsdr2/j7L+/j/tIawrlj2C5ZGp2WWJU1bTaQqUEuNei3KpCPtnF0JrGVEMZrL3Dr0jH8q1++izDfQErN2I5IkMWQ1+botGkkaIDUxtHraZzZwYXCLgUhhIwTxY2Maqfbw9OFBZsP1AhP1ZqMr2+aNp5V5x/GY7fbtR6Fc3Nzttp5s3KCx5+G4tQKFPwYbp9uT6JqZq3RbDiSVRP1QOFWSwhFQUolF6ITz+HjO7/Azz74JRVZgpxEOEMi02wrNnlCWfHblFaIsQHt+tb0O+fPEV185BiyKMVnT57hdw8e2QoLwew8htxqQWSNLdUKGuoII9Jz35ZHY286DyL8VzugUCvFmJl40u6iTLtYHGR4uLqBF/0MfZaANYBcqyFogHpTXdSY7UapVNkIx2ZYoh2sYLT+BLeuzOGTdy7gWGeCXqTB4jkNh1oHS5KkerToqfSDhsDNJLLHOh04uLhwcSIl4PZkzjTuWAaymVnDxdfuuzZHbF/XTKsvVZUpheerM78HLEp343e3UNEop2+LQ5kr9y1sVptJagPS02gG16/cwu2330MStLHFgp+GMCRRQmJUNaj7RrYUkXynF1r1PON3zpmOx7x2i2T6dGMbd5eo9MY5JjPzqJivqjAlQUa8VwVFKniGUxOvatL0Jg14HBx4wjug0MwQA5VCmSk36wD3l9fx5cISlplZ85iZVI4KRGSncqpzjaGWweDHL3Mc6waohy+Q1mP8+sPL+MX7l21R14QEp15pug828LqkU7sWf05Y2q21IsDBTj57qx6lfBUvqspUpxWN12oMsY43MbcXMqJ7CU+KRITXrJzg8d2wuLX00zgXzwZuXIy+THpKs820XipwRWhjUiZUdm1cPPcW3n/3IxybP4F8XEArKsSBOrHQb3nDb2TV9+b31HduSvpVUb0NuP9ofQt/ePwMDzcGyNozyOIuyTDBuAqgNWfVlqhOW+qDO6Gyl08eBwtKaR4HEFIfRZRimyXdZ1sDfP1iGU83+1Y9g3YHBTO2ZodwWtD1MHOLuepullS5bYvAhuuYiyu8/3YHH94+jdNHQ4TVyIY8OPMjo79LeHZkorkiRXhTA3VAodJ6Q1ZO4TnCy1UNpt6BendzumD6n7uc2L2vIbg0Ta0Kzldpfg9MI9WqjeVUY/CN+Bam8TyNdx02p+srTU3WRV1QxdURzp26QKX3No7PHbeOK5NCUp0+6vvwO4csAPIzEVR6NQt0KuSoypqEpwLiWlHi/uoG7tItDAv0qRbHVHh5K0Zeq21XvZQDqsgWYnq0E0SPAwNPeAcUNml0dxYrWY17S+vWfteX8tI8l3GCYZ7xKpkPZXan1mRaLJvSWBjh8Vi2uoorpwP861/fwuUzCcrBC5tWTCVjKbkJy9E20NYmUJ4acqm7OnFG5wBDxk9LGzVKTVVeIjoNOle7T1PN9nIb3stmbm8VptrvGsLzCu/PgXHK9GMqbw/ZObwcd9OvsLN1+y3k2QTtWO11KYbbObrJDN658R4uX7hii8VWlGUVv6OYyuZ15TeRQpM/mkBBvzXNns2jmbKQmHSwlJX4amkNf3y2iO0gQZ70+GFnABUklSeYBZjLNPWskajHwcLBtliHGKKvrBXh2cY2HjCDro5zVJpSKYoxZmbO1MPN2txIdnKqzpFhtzzqGvQ1g8Q88/EHN8/ho3fPoxuPUIxWWYKVcVDXehpvUp+bR1CE11RpNgrv4CefHdMqo0cDqHlJ1QEll6E00+jMbLO/uzc9ynu+TeF5wvs+UPqRU1w1273xpjh2zsW329oVTL9lTtXWSm0gusbmaaX0c2fO48rFq+iSwLSUkBFc7e6R02+mfssLIsG6Kq2QU/Fa5Z8BtxqIrqrNhe0htqnsWmmPfNfjN42U8Vgg1Li8aTg8DhQOvsU6pJDqeL66gYcvlrGwtoFcBJWkyKZVcrFGh0vdGeFpKyfwf5aoVc7VvJkfvXMJn9y5jtlOjSpbQxqVNCAhSlUHkewmNvUYfRLhyeiYJ5oYWRMkH+wsL1KSOuM/gzYlyUoGsJl/kRfxaENvL6NRho3C29uG5/Hn4OJVaVHx7GJweszQxLi2e52gbYuFi5Qqj0U/El2bhFQWLKhRlb115So+/vBj9LpdtJkn1AvX6iPVK5ffRmNJpe4SHo9U0CFUWa+aEY1n1VRjCxt9/OHuQzxd2cBYVRwa+M6cZKtj0DUk6nGw4Alv36BMq2wm1aTeY6IkUZOUlQhFpVB+IBlVGWX+qcdlHoQYBzGGURv31/vmVkY5ShKT5etizHtKzHQ79Ft+suSqXmYirumREBmSyQDHoyH+5vZ5vHvpJCbDDVTjoS2sGYYR8txNraWb3ByFDVxY3KwY00MHFG7As15jOrCZxleL12raaM23KDsn6rL2H/sW7pX1nVSlrPgUKJ7tXhnbCAkvUIWw4puGnDfImUrWtVMC3XWHFy4GHHE0+4LFShO5UygObds4/k7SGKPRwIaGaIqxcsyCyniCsycu4Zcf/R3m0mPohHOIJm20Kn4PZjdXSOG3DDVJgGbFUQWlq/HQ8IPQakkSDPn9fvfgMe6trFs154DqbswbNXhdJSRXnanqUOZdc/rGOqLwKbDKv3udnbCwN9dr3+PNwhPevkFWkqQS5KgDt/hq2YpJThozpzE/NMAsSZYkoZjntfL4mJmo7M5i0J3HF5sZ/vvffoUHWrp87ijlBa8vhrZga5e+lIMR2u15bPRrbJc0wt2jfIYWxBygS6I7Hm/gf/2bq3j/ZIW5Yhsz9CaqEhImyVZdutspn+aqQ7X2V1hHNPgy8iTeMKNNz7Q3fZeDB9ogm080ThLrvp4PC8QRY47KdWFtHWR+Ww9NpBiR0RIqhFgdIHijhnuo4MEPI5OHiKX/JGxjNmUcj1qY7x1HVbr2KXUuUvun5l/cmZ1jagQPM4zo1K684xyJOGd0sfdK5xiXija3ZRqfDJF0lCJzFCygtaMe4kkPk1EbM8FJ/C//s/8tZqMzKPsx5jqn+J26LMipAxa9pcwb5oUNNwhUGCz5JTP+1gTpTP9FnGKF6f53L5bwD89fYFHhmef3pQrULCxaUUGFPrdwco1Sjpe4ntH0n/9p7UflnUj5h/vusJuWrLImAr2fx5uEJ7x9hKtmVNWZDKHL1Dqqz6L/tcZdJ02hQdFDGueaxnlE4/pwfRu/e7SAAVXeSGuFsWSqEqPUYqhpw2g09GG3NgfkwuMstXbQHwyZsanuJmMS2zYun0xx++Iczs61SJKFdAlS+qXqIVXnaUYWkbIZHxqFlllrl1xkbGrruXmwy6gylTI6TSlckaiaL1sbjYarGcMoYywnlafr1MmhMWxWncmdSFNYaXorVfVaXEnhORWob2Nq0v4EfSy5ww2X4ve6V2OlOeLc7pWCPkZpzvKRfoq86hgBC25B1caR7km8f+tjnD5xAf2tsU1HpoWNs3FmA9Ib/9y9UwVPZz2gWTjMSW4r4xyP1jex0B+ir29PBThhAUnphJfyhmlVv/7XzdNfDSzk7vD0P3ed0s70oMcbhCe8fYUjEZfZaFCZcaUZApKWSr/KEFGiLtETEh4zqIirnODuwiK+ePBI2dyusQG1zKQyxLUa37XlubIu0Wkn9LekAcjQCanYij5LvRU+ee8mLl88i25Xa4epEo/Piqha5N9OG5Tlyimc0XFJRq75fXCx04bH9wio2BjN1jvTjKHZou96R2e49rbfqbOKXHPspTvdJYaDHWMHBS72u90e7ty5g2vXrvMb8LtQoasjS808ZCSkD0PCetk5MtJpzTXbp3J8trSMRwsvsErSK0mYVdxGZowlFUexSKetqdRp2jB/9Met6/Sl4w5Sfm7lf58a3jQ84e0blDVEMG4mCKcipMBcNWKLmS/XPJnMJ5o+rFKHFGa2J2tbuPdiBRtarZn5RVVq1iZAP6qWZlahsqDx1kwSvW4H2bBPKz7ETFwjrYfo8onXzh7BL+7cQDuRosmRFyOSnCYirEl+pRlsGW+F8WU3JTspvanaO6iQ+dHK7SI5szuML3GXhiRopW13kM7slPanMMNlBw1Nj8wkiq3Tiq40wuMxYeqL4dWtx18X+g7zc/O4/tZ1XL50BVoRpMo1L2xqityRkwp3u25KU8xT/Kk8QJW3SUV4d+E5HrxYtg4tZdrDiOlfCj7ghY7sXO6wKlnnA5OQq/LcVXQ678jO1KQ75PEGcbCt1oEGk/uUOCwT8IiR3YRqTKNmmWG0nI91ntCsKcxkmj7ss6cv8GidJNabNcKzTMqtVJ06rpQku5L7audQJ5ZytI10kiMu+5gMNnDjXIq//eA6zsyzpJpvU9GQ7DR9WOAyvAbkyqkx30EPaZwL7zRrT93BhZk7U9J8E0aW9rIsny7++uo76uqX3V41J7JLk4T7PKb6LmJv7Owp4Bte/e3xI6EIfaUwora24XCEC+cu4pMPPsGRuaNM8xOkYYdlO31fXeuq7lXYc8Tktjo9YuEPaYqChZln65v4cmERzwdj9EmYZdLj3ZrejPl2h/R2KrEtLDUPqjCq9lv98SAPN4SnlOLxpqGv7rEvUHKfKjwlfmUWNd5L3RmJUTlo+R4ST5m0scF8+cXCEr54toS1kpmo3eOVKkfqT9WZUni8W2QnpqP32ahvKx1oqrBgvIWjCfC3d67j57cukvyWmS/HCCMttNniljfwmXIy5K5aU2Hc48SsLx072NDciDJDZooUd3xnU3gadO6umDpBW3e1SvBOHdCMNQqPhNcoPJvZn8ca7O55/LXRfE/Fur7peJhhpjuLG9dv4urFa5jvHUXc0hyc/Ia6UP/ZjtyU7MxpKaEaBb9jHkTYYp57sr6Fr56v4Fk/wzhqM6+pUOgKRjuqTU7pQn4pb+7xT09QWjBV2BzweKPwhLdvUIJn9NMps7iOJkZdPKeMwlM0oGOSV5+k+HB1E394+BQvmIHVU1OzuTuFN81aU8Jz4+ZMZzBT5eglJLSqjxMd4FfvncUv372MU90aUbmFJKwQkeyk7qoqp1rh70jDG2j8rdOKHrAbzp3fO+6AQx1TaHSM2vnO/ARGdns7NDSueVv9Upxb/Op/1YkSr7bhGbiRAWxgxvCVYx6vj91U6BSURev0oPbV+zIKExRZaYvEahYWLRrbpsLTunn6qu5b6L9dp/+VE7XKyJBpQb2jNavRRgHmwQV8+uQF1nKQDGNep9mIXAcl93DzkLv0h64hPO3r2MsKz4Xe483BE95+wfKFqjNlNi0v0MmQulKhegiOeX7IDCV193ijb6shDIMEwewRDGhoRXLyRgRnHVbMuUzXQonZToBOkGEyHOH80RZ+8/ENXD7ZRd1fwnxb16izSmWdVoqC100JL9Tad6bw5JWFbo/75wG9ltHWzlZxqfdmjNA5znr5vd2e+0bOJGrfHVchQfMs7kDfc7or7N33+GvB0Z7+7Bc3nbSDYX+E8aDAlYtv4eqFayS8rpFeUKuGRaTXkM/0K3GjdNHu9TBmYhjTn7A3i3EY497SqtWyLA1za1OX+iup9Nw414b0HCwkU7Iz0pueM7KzZ3q8aXjC2zcouU8NLDOV6+igw468SmauPmWeGsjvLa7hd/cekexitHoz2NL4oZCKQh1Zpm1OlCjWAUNQZ8uw1uDyMfpLK7h0HPhf/OYj3OBOXFHZTYbM7ENmutw6ragaM9I4PqYGLbpZlqUZ8O+GHjR92AFGSwvi6gPwXTXoWDOsyI1GFdVBxFd82YC9Ck1B1ul0MBoO0W13bGoxVW1qnsYpYxpUmGnQ+Gaf2u16/FXAAuM446eN0OvMohiXyPoFbl9/Bz+78wsWI9VxJXGD0elCElaonrqlajt4gPmwz4Ji2E41wt3yXE6CS4+dwMLWAP/t//t/xHLOtBKlqLREF/NmxfQSTZckqpmHWvbhXTWpCNQlA/flje4OfhY6cPCEt48QqchZmxlTv4yvhiCoV6bmyYzmjuPR6hbuvljBgFm0TjrImFE0BwplGI1zyQymzhJudhRHUjTg5RiToo9WtoHzx4B3rh7HheNtqr0xiXDAEuaIRnnMa5mxLddJsbya+141xzr/qjv40FuYXuOOzbJSKE4Ete8547QbF80773l/3qipq7QWnl3Fw7JzzR072+ZWojm2453HXwDTiN8TqbYahn3YFuIwRRp1MJPO4fzJi3j3xvvIBsw/LRZSojbKnESXVSywJLw2toKkPpTSgGpb1Bks5zeWosuY10Yktn+6/whPtocY8voxj48KtQGLQafqTYUekZ39Eeafx37CE94+Q2TXtPvI1JY0ngVLkqouGTODfvroOb54umRVm+qtKUJUppEaU36KmUFFdtY9nmciqrUYVB6tHAEz31tne/jo1kVcONnjcam6Ia/X0AMN2t1LdM1WWXVqklUsbfYNusboYbrf3HMwIVtoEcl31BtpO9Lin9x7mez2xoN7f8V1wEP6bipoJImG7rvf6iwhT5o7Guz4pHMHO+p+krAo3RPpIUlIY+/UETdquWnf5E4fO4sPbn+ME0fPIgm7JDqe53ENV2hV/Ha8J2aekofKAmom0CwqypsZP3rG7YgE+NsnT/Fge2C9NuvODGoSp00NyOdJMZqO20kHU9rjD4XTwurxxuEJbx+x07lBhCcjSbXWsrn8UlJTjLvPl/HVwjKWBzlKlUI15IC5KdRX0/Im/K1prVQlWrBEKk0Sq3pyUti4u+Nd4PqFo7h24RjmOxPUxTYfmtk8uDYX5qvZbofglCub/QbKrLxnrzvAMKOj+Ge8WyleBQl+B81IY+csa8g1cSG4MyI72/J6VX+Zyk5pLPUdWfCwyYrlty7bC3ebx18cu5G6N3qtICjCY0lS0+JVWY18WCIJOjhz4jx++cmvMds9hjGVnlZKn2nPomIhUZ1cZrszO6nfkR4Jj/ty6rk5pt/Pxhm+3tjCY5LeiAXUqDfLa1ncLBkKZRFuGj8E+WO0Z1v32+PNQjnaY5+gQd4aVGAJnyzWUvsPVdyQuWttVOA/fX4XS4MMk84s6riNgkZZRjUO+eFq3lsyVzEja+YIGdhETFjmqEZbiFhs/eDWCdy8dAqnjzAzUtlVRZ85kFmW9+82oje5rtlvfu+Fcq5lUToRXeP0++DC4l0yjQTl+KmF7b4Ib/e3xdGrUcK4MNLTPxKc1EBbhKffvJGfaAfa3es8/tJgpL8SsQ2R6FvYMEs6TeatKpRiVGJStEzZffD+z3D10g3Mdo6izll4ISFG9heycKgxsU6hTT+1ORGfekeP6bbiBJ8tr+IfHz6ydj0VVCdUlVXBwiWzmUjWdU5RGJXf3P3m1zSMHm8WnvD2DcwAsqpuz4wuaDiZ57DCzHN/YQlfPV5gxiJZdedQMBOK3wIaaJt6jAqvLEpUPKgspQmQRXhVNgTyDEc6LfzNh+/g6vnj6MbKvGPadleNqYlraxn6nUynnT+RA+2UZVe3/Weg8ARHTu69NQZPLzoYDmxrRPdqnOyQvoPaiFSVrJ6texVe46dh93KPvxqUJl+OaCuS8JBlMSMe9aKNEGtFC6qwYBKim87hnVt3cOvGezwfYXtzYKsu9Npd9Le2zcu9qYA0xj81PUzcXJudLp6Nxvjy+SIeLC5jY5ihpY4taQdtFlybcXkKnjb6IaIz0tuTRDzeHDzh7SPU/V+2UYbXZlWhsdxmBnrCzPP5vQcYULkFWueLGXWUFwhIiCGJalJmzLwapN6ydgrlHstYFbNikePYXIqP37+FaxdPY7Yd8pja7rQagKrvNIMLL5aqccEgmhw4zYV79w26Uq4hOrnm2MGF5iDVB7A3476MJKPfYIbSoLiZ7r4CEZsIz9rwWNoX5M9LhOexb9D8qLb8j4YMME1L5SVUYSI+qbDh9hjnTl/Enfc+xPmzF+1402NTectygX17/sf8qRSimXksv/JoFkfI07Ytvnz32QIev1hExgJonJDs+GylnW/LRcpezb7Hm4UnvNeES7CNEnAZQ85NMbSbUSyTcE+lOnPa1yleELcqZi6tPNCyXpmbSPCiCHB/K8Nni2uoe3O2TE1WZcio3OJQnVJ4f0G9x+ujtMvcqemsqCqKAVrjNbSrPq6eTPF3H72N2aTg8T6KMVVLPbHMb13tK7d2m2b0d0lA7+DeYxd6gb1OaK75tusPFvTlYhUC7GtoDtIURauDES2ZKrbcNVLPtIAkefcdA34rVXcyzuRUTVarp22MSHGrOKF31mnlG2jSyqvO47WxUzCjs/09YP6ySRRUqOTnUMHEvqIKKSSsIi8x7o9tPN6lc1dx7coNzM0ex3hc8lyN2Zkj9MIVJN08tzs53b6j2tzzgj5GCQbM2HeX1vDZi1U8G1fYbCXYYGFVBVXlL00uHSn/MW3INrj7m7B7vEl8W870+F5gxmF6lVNmU0JWonYJm04ZhM4GnfJkya1bL8tsJO8u0cq30IuZEam8tuMunrVm8P973sd/WB6gPn8V221mHBJZEOaY7dA/kldQjpFGMSp6Mp4wszFzRkGNEz0+Z3UZV2aA/92/eR83T7fQmWygExU874z0pKRCnHSQ1CTJXDO8iABptI349mQ+q7LkC5kzep6CAW+uN4N/cDOs5i2dQY44H9LA8TvFxzAKj4FRj1EVo93p8m3VEYiSr1Xwm2nNs4D0F9NgKd5iRk2MgEQZ8DscmT9BddBCkrR5jgaNUSfIDpuBU1zRyJnBpa+uU4zH60NpT5GrNNmkRTlBkU9S04QKdKK6FvOY8l0+KZkPa4RUZ22SVTnIkQZtXH/rNq5ffw9xPMvCJajYuyxgJpgUpRV/uimVofxnhm+xsNiqqRb5vVtUiq3OHDZ5/X9c3MC/X9nCE+5vkzy3mU9arRhdEl9Hyo9+xcpSJEtrfm+C6/HG4HPdj8DUpu1kNXNWEnQZrnH62yEQYbqJ4xD90dCGGxRpD3eZWe5tDIz81igsslCTQfNyqkBNKM0sa/4rg1vHCj6ppuFuRxNkW4s4Ow/8q5+dwvn5FtroM3ONaIzd0j/uUzMDMhNqhgk1qLscp+N736CB7pkG1NCc1/WN23v9wYKtTFHliFSHxTiRwhuXMVWeupZrmIdi18W7ZJuLDRHW3vd38alqZVWXueEIuvLb46Xxw4hPfrnDHq+NJk027lW4GLc/5j0rfNCpkKozmm6spR4oLIkeIUFdvngNZ89colpn7tkeWS0Ki4Vq4EWlyR5Kpoc91aTdtM3CZISsmmDA77/CpPQpld7nyxvoxx0EVImiyzxj4ZYEF0vV0S8bQmQKz+NNQ7nW4zXRVF8KLmu5DGWZS7+4r2Rt9MSfUn3O6Q6a0qhNBTfBOIixlRW4++gxlldWWYJ03d0dZBhVlabV8uiYCV2VW4tKT4pyiDRkhmJmvHF1Dv/yb3+JY0dmqDY0RF0Zaydku27n2OGGfSPbsqROAzQej619Rt+wsu59gr7gXifsxp/a7zToXK7B3s4wDt8S3/oGey/xeKNQ1KvdtSoqKvwKs705XL18jUrvBo7MHbNJp0V2qqq2tRJVJcpCkHUa47d2DRX8zrSgRmBMB2qaePRsAZ9+fQ+L61u2bp4GqhfMuzbPLZ9qRZ26JJlOC68ebxSe8F4TZv5oGZt6fqEpPVoJsiE9JXAmbGsHYB5xxKejgc2eUnaPYGVc4svHz7BAshuXmqm/5QyoPFQWscwyJTypD2YgIW3laLdYEi02celMB5/cuYHzp4+SCGmwSxGeSJMZi64JiwK7s3/IQfvF+FAWEOEFNgavKX1rq+N/CiI2GUONw5PTfTs8+a1o4t3H//6D36pUjiJ5lRMUGUmvO4u3Ll2j0rtKApy3ji3q3amZV/R93UxG+nIuP43HI/utX60oQtLV3JsT3Hv2HH+89xCLW0PrZd3qzKBioVajh0SYqRribdFljzcNT3ivCyVebjRWxzIP/29mRm+c6jGaFptG2TUN18pqfS1TcuQknmwO8Lu7D7Cds+SXslRo8/A5Qyzym0z2qjvt07jK2JLoethGWm3jF3fewofvXEExXCMlFjYAXYTnOl0odMyi3BrZqX7FwqfjhxMuXrnDeBZHifw2t7ZQViI8frHvipq9x/ntjfBoEFVA0ez8phC/82ZB5/7UeY83AeVaqbs0akMLw477I5TjCmdOnsXNt2/jxrVbKEmCIkIbU8drpOLKukJFJ+LSOFoJefXczJhnkSRIZuexzd3PHi/g8ycvsE7SRGfOxuiJ8JROYmbtVl0wDH+ydOTxV4AnvNeEMsxLs56rdK/ETxKRE7k0hq3hFnO6mheqB9cw7mKpDPD14hoer26gUFfmOLbJoKUwZJT1iaTunNO+nLpYVwjzTcxHI5s+7OfvXcW5410UgzUSa45Uy/6YwpuGowkPt83foYailsSmyYVFdpppZbvft62Ofxtn2S1ul3C/bAkaWxpICs/d980qTaKJ/5ecx76B0S81npKINFRBC8NqNfRO0sOlc1fwzs33cWz+JPN4hGyU81rmPBWOrGqTxc4ocAv+qjaAno2LHMM8R6vdQdCbweq4sMWan2z0sUECzEM+g2lNY/g09qHFh7+SQjzeADzh/QiI7FwbXqPwnHMN5M6gaaNrHDHKINIx4xRqt6PC+/t7T/H5wjLyuI2SRjOjwghoQJuqNhGcIz231TEdDaniesEQ5+aBv/voOklvHkG2iU7kBplrJYSpduHV07+G7BSoafgOM2yslNpeGBWKKVelKbJiPIu8GNOK71fRHG2ITWTnOjJ881qzrH/SeewP+N34zaxdjnmyzfwXIrF5NdtRFxfPXsYnH/yCiu8cswoLm5UKN2o7Z+FIK6ETGkdbUtlpHlX16h0VJQb8XSUdFHQiuy8WVvBovY8R8/sk7WBcVizQ5jYm1n//Nw/FusdrouGMhkQc4bltA6f/FM2uWrKikc1VhRImWK9D/MO9x3i8voVo7ojNlTkuCsTqAk3Scwa3ITk9T9WjJRUcryGpHU2BM7Mt/PzdKzjSrjHeWsKRLg0vc22ZjXiPzDgDyTBJfZqJtUArxIcdjJApsSmWpOykrCtJNKn16TUuthq8/EvnRY6Ns+u/TRrugOcU//YNPPYbapPLxznKokIn1TCEFNmQv8cTzKSzJLyf4fzpizze5iejkidpqU1Pak9OCwXnJC/NsjM7P2f5vj/ObLWTkoQnZfdofZv5u2/rWNbtLgmxwpBqMIjjaSg83iQ84f0oyHA5Jx3VkF1zVDM2ZMxQGkYQRCmG1cTW1MLMLLZ47L//t/8Omzo3ewRbeWnTimmVZS0TpPu73R76230rRXZSjf1hyZDKbr7N0ma2gTYv+te/uoNTcyFG68/QjUoS3TZLqSPM9jrTUMhoG10a6e2iCeXhhL05DVN/OObnmOduiKWVFcSKf6psa4t7Kb4aNPHm1J2M3qlTp/lT+03bKws+1ull91pzO/41xzz2E+oJLdJTtXY1nWQ6ClJo5QTNrak18z587xNcPv8WhltjaIGRDonMXUfyYwFW04hp3b2trU3EGts30wVzPMZMB8rXT0h4/3j3Ab56sWLrWapgO1E7vUiTz1YacmFw7ttrCTz+UvCE9yMggyhnJCenv2l6lSaTzWszg4CKTmtlufF2HTwfZvj8uRq0a2wzo2UkxjqMrdQnwyuCYn7ANsnu+PHj6LRTDLY3bFHXFGPUw2XMBCV+/eEpXD49i25AIqTiS8PKdVZhzqyoFHcCQ/8c6SmM+ukNruJEak7Vx1J3WgB2lBW21ZCElibifgl746zZKk5bNHRuWjHtO4Mlx2ua6N/BHj+8yttnuPjfySL6dix8tjRzDtWc2u60gsJc9yhuvnULF85cwmg7IxGCx+ZNCUr12T30xD4n/9OfDe0jeY1Iejnz+1oxwb3lNTxc28RAbcbtHoZMe8z6hj9dK+Dxl4QnvB8BJVMjOrqG9ASRnTKC+oy0mbg1M8eopPrrzGCbxPbFyip+++gJCa+g6qtsaLOtlhAxA6ldifeL8PLCtQ9Y1WQ1NgUXZuvoTTbxwbWj+M3P38G54z20ygECno+Z4WKGgT6xxKpGcQWI4djJ1c3WZzBBSi6MYlPUrTDEYDBkdAXueDNV2I77JkRuapfttLs0WnZk6l7BNwoYzb7/DvsGfiYtkeVc8x34NUVidYSgZuGzijDbnsft6+/ZSumakUUrLXTinvXc3CU85TIWmtQLRt1S6GfFgxmfUSYp1ssanz99gc8eP8fSqMSYheCMas9NU+eebTUKnvj+6vCE95pQ0nTtdtNqTDl3ymUAHlAVSGCzNbJUF7uG7Gf9ET5dXMKDzS2MeJ0mctb95of2mWlEdkr77XYHm5ubGA/7LFUmpu7CPMeNsz38l7/5EBdOzCCaZCiGm8ycOTRdlrScZvFX5t3t6NI4y+fTbRPaw4uG8ApuFef9wZixwpjhvlPau9/0G7APTsKj0er1WKixi128C03V5jd9aH5/p88ebwBSYi7vTknPjurbOYUn0lMHFlVvHukdxfXLN/D2lRtGeuNtFj4TFjSnU/M1Ck+tweo9zcxo/mZ1hYIFqWErxHOmrS8WFvHl8yUsj0mK3VleqbT2TaLz1Zp/PTS50uM14IjKZZymlGgmzzKAavhDm4i2YsZopT1sUuV9tbSMr9Y3sMWMMIk1i4PucIm+VA8vrXjAfRnMNG1j0O9bJmqHNUrun50D/vaDq/jo5lmbZaUYbdNy57Y0kMbmTaTsZGylMOk0hk99OpvfFkJvaxkF9qX4j/HD+BhnGTLVAvOHem/qmDuv7RQ8ZndNt/pfxml2dm7aZtdgetM34rk58I0THvsAEZ2IaTf/8rup0MJ8ItJDRTKjyitGNU4cOYWP7/wMx2aPYWt1iyqvy+uUr0LmTylDjbPVVHSiMck/5lf6Sc6kymsji1I82x7ij48W8GB1E7nG5fHKhvDkPP768IT3urC84dSZtpZcp3ZOFV1GeK0IeV4y/8RM/DGerKyzhPcCS3mBstNhXtG0VGqslidufI8znDKkgXVfTtME3XaCfLSFhA/58OYJ/OydS2hPBgjKERm3QBqHaCcqbbKkyvvdAGj+5Od1TnpOgXMBnAbz0MNUnIwNY8QNSaCNo9RTD1l9B33TvWbo2+JtV+G5K5vS+a7CexV7ffTYLzS1M07dufw7/YL8c4VDrZ0XInbj80hw1y5dw9WLb6GnZoqmSpPXKq9bTmP+C3hC418dmbrV0Su18aoDWh3gydoW7r1YwXNNPcYrPdG9WXjCe03sZJKG7LRv/4nsGLEqJYp4lOqDCNvjzKYcerq+iVGoYQksC6rOX6VC3hDwS/AQs8AUzChFWWBmpueuIbHdfnsOP//gFk7Px6hH6zvLBcVUizK0pTqqEJG6PPPZ7vPKx2a713lo1gs3P6Jrv9M3kMrWuLpyZy7TV2FffeoYk4z3Tqdpw/u++EEXe/yVYHmXWcFtm2+iAyIxUhjzrmppNBi9VWmKsQTvv3vH1s/Lx2Q8U3iO8GzmI6vSdE7t7q2QZEfvSpGeVl6g0hvTvwWS3ad3H77UaaVxDZqCk8dfFrKEHq8FJUhGHxO8g6o0akRyNKLKP5pZIYvb6DOhPy8r3N/cwkZBkovaVmqcqBu7KTJmLBpd9RDULOwiKyV4ZZokrFEM1zGb1vjk3bdw89IxxMUGUgyh9fSUL5RZZaBzVYfSagc02PR0momlX5QB3b77Nw37oQYNlAySCh1U3+OMEcN4r7TsDxVeab0SXLwptmTUdgoOFq8qzctNkMTq4KL77X+7Q1sdsD+7d3qbnXO/Pf5SUHz+ECc032X3mOUXc0oTamIojIS6VHRKDtmgwrXLt3Dn9s9R2/JaupPX2tfWd53WqJhNoOpjntbsLFpNv6BrpR1M2jNYZVr7YmEJAyq/AdPamAXggnlf43QbsrWw0TW5VxNTN6lxmiLpPH4oDrvVe20oUYaknSpnQmRRTW1obWaUsBijledGYCK3wew8/jAe4/9x9y5+v7aBEQkwmSSIhiVmWOoTOVZ5ZW19edlCrkW7eG8chdZul28vYSYY4ee3z+Nv3r+I4+0MrdELzERjMuqIAalJdqVVn4TtDkuTIUZ8vipqlEXUphBONFi9NOMsA6zP7jLnYc40KtVXVlBohV08erqBQZ4gTI9ie5Qjbqt3rOJMcce43WOIdF+rxXNVgYunT2PU31b5whYb1dTf+hYyUDJKukcdmAJ+V6v+UrxPjZc3Wj8O+ha7rvk+LCx+q2vO796jdrrQhiHot74Gcw2/66RFoqMLIn5L/g2HY361FJ34KMabAY51L+L/8L/5b2D9eCc52t02AhZqt8bMx7wu6R5BTrvAZGJ5Lqjor5FeCyMWrtarGM94/v/2D/+IZ2GI/MgxbDIsZUuLMsc2p2dM3+kDU5MKzlKJbtukKwbQbT1+EDzh/RgwNUYkp4iJMFAJjgmbxTmW8FgaVCGRZPd8lOHz5RU8HY0xZoluQsKTRQyZGQoeU6JOeCyia2lQulQGCSwf99GNKqT1EG9fOII7N86jF2SYZJvoxsyIpe5VtZurlrES4B6njKJzzrTulg3dUUd4hzrDyGZQoVnc0dhkhSqi1JFAJW3GmJRfE3eMtsaoOigenbprs4Su84IKGaasLV6nzu7TV3b7dqv2+Y08fiSaiDfs3f8u7F6j2G++qfs+gs4rTTgn/101o76bOqckLNj2MNc9gePzp/He7fcw253B1qbmYG1hZvYo74sxHpH2UuZn3mmEZ+lIBSEWiNSuHyQYhikebQ/xh+eLWNYYXarI7UxtfxFmZ+ZRFKoyVZgcmqTjDrnQ7j3v8f3gCe+1IbNG9RRolB1Tnrq2k39qJvg8SjCKYmRJgodLS7j74BE2N7aMHEMaxHJSoQ5bVs2h9j2EmlRWJf8AcRwjpp9hzVJlvo0jaY0Pb1zEhzcvo5PwiWVBFdJBxQzoSMvjdWG2jJZEHVWGw6Ft1aYnMtrbnuIwNTI7W0Htdz3bqgr6G7d4/FWhAobrZemGAezuf5vbe57b78kWe9vSmnY2jY2dIyn97MNf4ML/n73/XJIrydIEwc/MLjHmHHBwzoIgeLLKol3dPdMyu7MrsvtrZ0XmDeYd5g2WvMCK7MqOyIisyEj3zHRPdVV1sc6srMwMHgHOAQfgnBm7zGy/7+i9buYIBAMy4YGCHfdjl+vVq3r0fOcoPXgC/YgeGL0ydWYRBne7Hd4j/09AJ6NUb3OGqQMuGaUVbBLgPr5yEzcWFjEI6+jTIO5RJwy8Cg0uyaEzWG2xYh67OhnyWNCemcYa85lJApnSoCfoSUmqbU4riQd1ZLUmuhTg22ubuE4L7tHyKjJeDyq+rZqcqEGABaKiQeUEPD2qiWdThuP5ZVQJbLVyjHJ3FecPT+Lt0/OYb1ZQLet9mqGBou+FLDyjyndM349ygCqrjWWAre22jcWTotFp+/1K8hYntBWXbUiC6SapIwvwaWA5Svmz33TLmL4DKdEFIgU7r2zI8pZcb8nd7O43ZPqOmVBM+ZWmKaIoovflZjE6cuAEzp96EwfmjhjoJfTsrHqUguRWQyDY8Z1FmyDfTqaMMfvV9JAFDdxZ6eKT67ex2OrCm5xCj+9Zb7dQ1qKWvN8ALwc98xbJBnrfMe5j2k1jwHtWskKTosJC5UkItdgVvTsETfTCBlYp0B/dvYc7axuIDN+q8EEhZ6FJKMAlCbR5dmq3o4NI704dTgaDFIO4hVJvHYeaA/zRxeO4cLCBSncFVb5PHSQ6qgIpBxT5cfY9Kzl1R+WkzgJMf03jxgw0Nm+tuOcr2KR78i0vTk5M21bDENSWO+oR5CG43VGyU6P3jen70k7KFuBlXIDLKNDsBp2C+fu0nNlFNjQlN16KuS51LmUZzjRzUlzBuRNv4u3X3kfdn0Bns4tyVkKVnloSqcmB72QJL6DOga0Ar2RLgbW0OkO9iRvL6/jN1evYyAhqTU07xvBZtAXQrkp96OWpPdmtvkK2mI3p+9BYYz4zEXT6GgbgOoPYQNVyFXGlhqV4gBvr2/j84WOsxrxercP3Q8o+FSIFdVChtciUV5WmelZaJ5UgRIVBpBEVr5b5GUT42ZvH8eMLh3CoQWHvrcHr92xZEXmEMc1EFYcxPSsJ1Gi5l31mSwmtjlavdrZzMb2bU3VPI6W7uIxmc8L2DfDkIkqjGeVPPzWLxvn2vFTkgKW3krrgXQcjvOuQ+cXNt+WCwK6YCEJDWLTuoS30S+DTEIWolWFu4iDePPcOTh45A78U0vitoKa2+sTpBZMiAlfhWWrsnxuqUMFaN4U/e9AWgv7ttZv44t599HyfoNdEVwvN5p/mJNV5eKrOHIPds9MY8J6ZKLyD2IRZxUeKs0/A2+5XcHujhY/uLWCRYNcV0PlVJCkBkpah2uhKgYdeRiuOoUhHyisQkA3Snnl3s/US3jg+iX/xo/OYryWopuuol7p2bUCQ9cIqegyPD7qojOkZiGlX8mhrOE+504uch628JHg5T820Y8683ZSrzhfbMuq00LXVcBLqIvecNNIO7Q7DUR6G8ZiejZhPA/WwHGV1HstZ+185Ht7rOiB9c/oL2EZBT3lbeHrqo10ZsGzHHg7OHsVb59/BobnDPEdQ7JdByeJ7ZAhLPyjvHegxKiY6mlw68WpoE+ySsIk1Gskf37yNywsP0eETMY0u3aP7FUvn1UlSHYi6mI/K1Ji+C40B7xlJQpup84mEmMKZEfDicohNum73t7q4+njFwC4LqrTmyuhFsU3oLMCrkKNE3Z4rLEQS6Iy4GbHwbMNPWzg87eO984fx5sl9GLSX0G+voh7wPUnHxgYFBDxbwiYX+zE9K1H8CXpSIzJIDOw0vuAbDAkZNwXYaet7VHrcpyo0wBuSDvIT42z6nZPywcBLwwrUdm5AVgAbeRfQFZzfa0MRlGffnDHy6ESqxhToFVu3P7BJpDU2r1pp4PTRszi8/yg9PF/uGwIaUg5YBcwMRGLDDQ+dzqCM1WfmsNLq2bRj/uwcbq+u4dNbt7HcjZCyjKvaM6MhRd/OyWYRyKhsjel70RjwnoM0QFlrX0VMRs2Nt0GXTasc399oYzVKkRDwurTwNIYmqLpFXeM4QRJnVsVZrlDZ8rqGGGh5nwk/wXxjgLdP78d//S9/in5nCVM1zaYSox+3UQ1967TS63URMryx0D87SYH0qX3SbIBHj5eRJFRiUorMkyim8WED90dTmAc58y7zEJrNKYSh5lQU2GnlczclWdHW454eK6ffCzFJk16KUkajg0ZLWA4QlGhMCmTUdED2CRZB2bPr8rpKmkuAZdTnfUGFxmjqQMyCo7VSAFrhzWlfeSojVVTkrao1BWRJ1LfV0TNuJ+vT+PmP/xBvnr9ogKdVFXzV+qQDaLFYjcvVszJY+zSUvcBHm8BWDmpIK74tJRRRJ9xd38SlBwvUJYxPtYEO49tjGKqJsJU9GGcznDW5xJi+N40B7xnJLC4/QMQCFlNQ03oTywSySwuPcXdlHX0JMgtcxoKjagnVx7vxcbL4HKu3l4Y1NLScWm8dYbxuYPfBhSP09DYJgl1aia7a1ClfKU5VkdAjZKGR+h3Ts5LyQpZzxcBO++6cY+sW7m5zZIfKhJx5v9pxNPmUK0Y6V3BOtvsk2BXXR+4b0zPRRHOC5YcGSi9Cq9VCt9u1Ad4BwaRWC9Hp6FyHBma8A2LKjjhK0Ov2DMyKdrlRFhVA+PVEA4fv1qQRmmZMHa/npvbjrdffxZnj55B0M5t0OvRqqPp1xos6QEOKKpQYTz2D+QBJ0iEdkdLQ6hEUtZTQg60Wbiwuu1XSwwb6nhaM5T0MQ0BZ8T0kmXt+TN+PxoD3jGRKESxstB5VdblOcLtJ6+yL+w/xaHMbntZIy5Waq/ZUNYYargV2rkqFpRCBV0KVHlwQb+Bwo48/vHgc7507iHK0LlVsulU9OK1qQwWWoeo8RZ7hfFuhHNPXkwM8Gstotakoua+8kQJ6EqJ2k/LAsVa7rqgNkMpIHp48P0fct6wvQhoNtXh+TM9LrXaLHjrLTuhhYrJOZn74A7Q7W1hafoSZ2Uk0mlWCBFO8TEOR7AcVVGsBueoAkFSAW9FGp60A8ttItd8CWBmvA3malSpOnziHt954H4f2HbVV0/0S318ObbLpPhHLq8hDFPhFLMepJJAhDZDxvRFBTz01NZzpszsP8bgVoccwBwS9Ht/RpXc30MMEzTgliLtojOl70BjwnpGkIDV/LKoTaLFEXV9dw6XHi1iOKIiuhEmFUrX1CXZ9pMYCP7Ul0JLsU/B5T0j57XfWMeUN8KMLh/DOyVlMlNvwshaf1ZRh9AQ1mF29CQl6LF2oDGgpkhX2mJ6DZETQ8BiOwbOTpvB2PDyjHKDMsxvu16oNKjC55zJeimvi0We1P3osGglnTM9E6gAS1H0MCHC9QQ9bvU1sRZssKzFq0yH2HZnFRneN5zbQirfRYXmKeJ/u7fXJaVcl06ot0zQ1LoDPOqWQv5lcvlJUKAdVq7JsbXWIZBWcOnYOH7z7M0zUZkDXDXEnNcCrCEz5TL8vYzXROgzwdGGQcY+gR2+zR69xJUptdfQv7ixgpZ2YjhnQS9TsTQmFVPemMnbHIvS9aQx4z0jyBmINRag2sZJk+OT+fVxdXkJWr6E+OYm41yOoufEzmt0hM8CjsFNK1Wju5vBjockiDLo9nNzn4V+8fxZHJwfortxBM1Q1Zt8ariNaiFG5yudtThcWEnp4g5ghjQHvWUmmSOHhbWy3mNbOPKGm28Gu3TAlD46/PGlV0vzTLCtaGV3ZuFNlxqds30rW7hCGlL9gTM9MKhvdfgepz3JQZzrX+ogrBL5kA2vdZax2lpBovtlahnDaR32uimCKeVXN0Bm0DBxlP456dAK8AvSKqs2vJ+WxAFOt+PLcPKQJ49RJUK9O4vzpN3Hk4Cl6cVV06KlRCSD06elpuEHSg0bg+v2Y7MBP7XoZ36lOburEsj3w8Nmt+7i3RrDmflmgx/NxDnh+oDa8r5OvMX0dVf67d07+9/n+mL4HydvSMv3tSkigW8Nvbt7B/VYP/uSMzYvZ2t5GqF6YFE9VZ2ZWpSk1KbBT4zrF1QS+g7mghz999wT+ix+fRWOwge7GQ8xMNRBRGwvkNPdeRuCTgGuRSX+g6hABoqYX+7aCOaankQAuo+fc69fw60/v4c5ih4qlZlM8xRr8T4Xqxj5RKUrhyLjhGeWgvAudO3X4PC6ceRNxREudecSMVsBm9Rv2mUHCAISSBnBihqPqbB3ZefGYvi8NZEBW6P0ELAdeig69uEcrD3Dl5pf47ae/xj99+Ev89d//FT789EN8cfUL3Lp3G6ubaxiw4E3MTGL+wDyijrqbuXa7wqMrqjK/rVpT5lKF2ZmmsU0VWHRu0RhO9eBUeJqYfLu1Td5C2SshCCs8F9HIihF4ZZZ9gqZAVn8MS+tjaqpBLSvGGKG32UKTIDlTb6AR+vAlLwRHRVXtlOq8Inkc03enMeA9I6lDSkQld2NpDR/eu4+72210KJx9WXHqVcXCok7NJSpOeXdu5gQqTCpOX4BHXVgup6iWI/zkwkH8qx+fx/FJKlhap9VKCi1dI1jTUIdEjdcqBPQW5d2FBDzzHOHb+TF9f3KAFyAi4P3q49t4uJqgg7oBXqRqJubZVwHPpbUBXt/D2WOv49yp19DV6gqq2jStJcBTW5AUpmufcTpJP2IpVqdcx4D37JSVM5QawOP1BQLaZ/jNx/+ETy99hBt3rhL4FrC6tYz6ZA0J/7a721hcXcTC4kM8Xlkk8K1idW0NQSkk4Lj5awV6hZcnct7615PlHfO3QiDL6LUJG6u1Gs9riAvznc/X6jXESYRWZ8uAbqBuooq3ZmeiCHipZEyGsAxihkMu0VM0GVKbILf9bg/lKMJULcRELeD9DJteoebkHag36hjwvhe5kvdKkhSNFJIqCFxVo9ajEpvXRCtcMu3msFMhkEdFwaR8xTSxtIbVJj28K4sruPFomaDko9qcRC9Obcxdo6EByZR7C4Oq0liVaHoTCwp6mPBamAlaeOv8AZw7OY92ew0RC0g4PYVOr5c/K69O0wrpOZ6wEBRPVW+Ohf15SEpN7Xa9mF44DQelsOTC8lwGCYFOy7Uova23LZWV5mfUdeWnurfLQ9AYLwXklKTySBKlLYnnlOvcybkgd993Y0fDM04SnqThtVGWSh3l4bW9puH37KaiF7PzhPM481Dpn5UTZJWYHNFQ+Xv8w2/+E/7h1/8Jn1790MBvEGRozNYwNT+JVrLNchkBIY1DP8FWvIY7j67j48u/xj9+9Hd4tH4Pa51FdLNtpFqJpOLyV0OFLB7G7ndI6njmWHPi1poNeGFAGdIcmwQ0Gj3yzxphE/un9+PogWM4MHuAskKjNZZm8RH6BEZ+m2RPExZo+ITHV2oFFbqLBNASurzYb07QkG7hk4cLuLm5iW2Ccky9o5qfWB1Yirjl6TVMN4uobUxzSXZ5306+2z3D+14leoU9PIpAiYWBAq6B4/2S1apTDlQ3rgoFtZVpBnQfSdyhcKaoE4hatOZWCUb9uf34nz+9jKs25i4DbXykfYYj447CLItPs6przJxPIQ35jqTdsXXz6rTWEC9iur+O/+bfnMEff3AGpXSLz0TwQ9+ND6Jg2sKwjJUatsUCPQm4PDtVxznvjoI7pu9N8vCgsVjeNP7tX3+G5Y6HysQsWlRc1bBMwGMGxuqy3rDOSZGs6pBeO7VSOaaXnQX413/8X2MQ+6jRK5Ri7Kep8M3a7zSA3VWJSck4xe14qELl/ecqaBcbcUc6yQXotjZZ+C5m+HatuH/4DgcUNN547qusZ3WPu3MvSCljvWItLu6cxdyUtqqOK6hWm1hcXMa++XkVRrSTLQMu1BL8h7/9t/jVJ3+Hlc0FAk8XlYCp7Kt9K0ZCbyrtx7YQiQAsU29ngmW5wvRm/qX9DjrRFu49uI3Hqwv0BAPMzU/zfEJPjGWNDyY0XLUtcku/AjmleQF46jHZU2cXxtlTRzXmt7wuxX6gjjA0Xudmpmn81rCxsY51clnVp9QHnY7KeoP4xnupMyhpCPkr8FOHKXVJS3yWfnp1HX7DWnvTOrvsY3gzjTplM0HAd2r+Vk1ZKF3hW3Uoo0h2QEfmCdvypPujiaY0tu+xT3ulSDL2ipIUjUBPokFB1a+UgZJEkkBWC1xMwFJXYi3Xv97aZoEjKNabuLzwGPe22lgi2KnqUVOLSeBM6FQtQab6YwrTE5NAM7yGGprjNnqtZR5v4s8+mML5w1U0g4iCSvDlOzI+F7PAa5SdlKFAVw3b/kA9MxlXhiOw02KRTpGO6VlJikUz3qTUCGqTVc83q4bMElu0U9WWJXp4WqOsT69+QIXZt+EgWjWPSo7PyMNzSlohUo7MKNFWMqUcLLRKAUQF8QF75rtQEY4eGd0vfouAuHURccTbiqtfZf3uLX01BsWXiWRA9LFv336srK1a2UgIYr2sjX/88O/xcOUOQYyGJ8vNoBwzjzSER22v8tKYB2V+IbeOdY7X6B3aveQ+PbrtZJVe3l1cvv057jy8gZjAyeJqz6hNzsjyVrFSeVTa20nbFkBSAHeR1/bHZ0LNoEK9MDs5h9PHz2L/7EFE7RSdrRhhOGmd3iR35hMysIqqKBONsWU4lLdWv48udcY2dc7DKMKttXUsbG0z3vJ0qV8YCa2q7nm+A2cZWOqFpc8X6llM7RPcVvErvoVbd8erRa+wxlR2C9xUdUm7hxLhqg5V3ai6dYEhLfsoNvAaVHy0opQeQY0A5+HL67exvNlCu6fxMFR7tNxUr66ZUEQyvGwVBApiolO8Fvp8X9KGn27j5PwE/uin7+PY4QP2TqSxq6RUe0A/NUutENMx/X4oo4LQpNGaxFsOXdF+o60quU1FMB+Kbuo7HRkkLzxWD81d53MabQv6VjJtNMIml0/sG3+VnIol832OKbNiya6ZTG6c127WOZli7vv2jhwwDL+viIu24j51d2LeUZL00Iva1unj8eICPvroN1haemx3P42+rf3N0cA8us3tDVy9fhmXr1xCq71tg8L5ZmubM1iTJzcSnNvNf3lBxX3Iu9+r9jyN0ZyensNrr72J06fOwaf+SKhSAl9rWrrOKgJzeYwajK7Fn9WRRUOWNMZPMqYVPba7Ee4vLeOWFozt9JDQu+tp1hXmu16rBYtVM2TteoyLvEwFbWzHbt9oR7ZePXqlAW9AsHOioV+11VHQyWVa8NqXta7pwxIKSFoOUGlMoZ2VcXd5E3eX1tHNqDYojBUKnxQftZzrdkyhTblvEzzTyhvQApNwa77MYNDD8dkAf/TeeRybn0aNIJhFHQM8Aa2mbNAsKh4LgYqco2IrejUF9XdNDipoRRPwZBSrs4C1qTAfBVhSmoXetKplGjROZlR9VKbiojzw3NOUq+4ZBcCnk54r2MmhcS6TO/tSTgXbvQUVIKcvsZjxCXFurJk8iyXLoyzQK+75tjj+nonf5NRy8WWKz5CzQYJOd9sGkHd7GjqS4dKVz7HVXkeSarIAGopfQ98OegN6RiUE1Qq2W+u4ffcGVteWLEwVZZsJhUjhUqhIJ4a5kw9fF75S1bGGKSTkCnXH/P7DOHfmdZykp9esT9u0ZJI3qya1bBbgaaozfpPJGOgh+sJ9Bkk5q4TY7CW4vbiM+xvb6NAAj/mN9FvNYIsSgiW3+mwZ3vIbXYcryQSD4L6xxfDVJaXFK0vWxkHmjymOChWCG9RNxUDBF+D51arNcqChAcHUfjza6uKLWwvY7hMIy1V6flWmoptDsZ8lFFAWGEqUuicLKEsBr1NwNT9j0tvCdABcPDGNP37nNPysg1LSRSmN4KsahQVa1Wny8CqUXFdsRulVFtXfNTmIaLUjKlYaPcxDpbYsaneV2ap8FKAwLwR6OieLvcx7q9W6AZ+ujQKcA8rvmk+67ym8S6maunqC83fpTzJWKDPF1dgBmvPoiv0RlvGVx3fPKY/7LrKPcdXLHQJdtU7jgrr/+s3LuHPvBosby0qg/HPpPmpcfNf0F8Coero5EVpYK6uPcX/hDtqdTcqCPHyBqcItwh7Zt803vUPXZDgxjv0yPTrliY/DB07g7Tc+wJH5k+h1BKi8h9lrMzDl4TupNK1kbf9JlNCApkdYn0DqVXFvvYWri6t42KahFtKY9lXdLn/e1UnYEAt+f1+T0zNdxCYTFra94pWmVxjwJAJ5b0z+DaszxRRGbq1iiyWtx2TqDCq2hMf1R6u48XgNsVenhRW49jQKkWZsoHRTSEvwKYRaCaFMoNOSMyyByNIeAkQ4fbCKd07N4cQsPcO0ZQAbsIAFFU0SLWHP8mpNFTjRkxIqsf32Aj2mbyanAmjdt3sEPOkeB3jm4WlPaJffpXzVBMDqIKCtR5moVmsEPufh7Va44u+TP3l+mjen4pjzzv6TYbl32VmBHe/R7/AvD7FQdDkXym/IXw35xVMR21HS94nVXsqy6JcM9NSd/zcf/Qqt7ibLG0skAbDoTfk0+vY8GLgxcWqBZ1i9pI2FR/ewuPzIOr24ISMKv4iPqIjvMOzRt8hOkcQ4g0XGkE9dUKUxXEavm6IeTuLU8fM4cfQsGtUpghNfnMuPdI2GslQ09Ri3lvt8bRpn9Ga544XoB3WsJwPcWtuyOXu3MzcxRSmsokzwU22UW0SacE19pE8ovDz7HBe7V5qUrq8kuazX5xOYKKBFG56qfVz7B5Ubha+tqgK/htbAw6V7j3FlYQWb9O66hC9N+eV8QT4hpUjALApKn16ap15WmaYsighoGQ7N1vDjiyfx7pl9COJVG08X8n5b9zyLKZQpfAk9o1V0fhjT74tK5oGrbSQhkKmCz7WHcCuDQ7Jgyoj5YMxrZD0nRSXAE+lcwVKyRZvedwM93UM2BTm6b+ouPycqtoU8cGu7MtgKHj6Th2R7+pN8O0+wYMZR7/khildefuThqQd1GHroRS0sPLyL1fUl5pFWslD1nSZVL/LkWYjPVvpod7fpYSX08spY31gh6N1Hp9sCcYRZwbAVj5GEUnran9IxP/ckFR1Y+ny0rKYQ6YmIIJSUMFGfwUkC3uvn37a5N0slgpTyglRmwbe5NnPA40fyHhlVmsZwYNML9unpLSd9fH5/gcb3IpbbHZ6v0NMLaJyr4xVlWdGWDDKIIStG/GalGU/8ULP/902Wrq8mSQz4+Vb4C4FwbR8SBVvdgB7apiwsWlZtenOX7z/Gg/UWsmoT7bz9TvXrTpScWIlU/5/EXQp6RguthVLaRt1LcXiuhrfPHcbpQ00MOquCTPMmS/0EWdzjg6mrf6fgfr31KjF9FUX1d0vKL1Vl9mJayXSmDfCY5Oq5aZ1WcoPDcrRIbm51ukwl5dbBc/ktGlW83x3wnkZ8zhTgyHaH9A5GQNV9BgySO3mkBbv7h7JI3glDClCy7tgVfZ3/IZG+iSylzO+0wfsCHFqjN25dR2OiBg03CGs+r2fWUeNp9J3Snu+oVjV+rsM3pVZtGqc9rK2voN1p50kzzFM7YWlJyk8XBoS72fHwiRJijctjOqsNj8qCukC5FODAviN448JbLPsEQ/UC5jVVlSt/CoNJaFkicoVBlSCouFFOCZzlxiS6lRB3VjdpfD/CwsYWWrwvUbMJ46KOcn0CpJYSsljIEBMrZjuRG/2uV4teYcAT5UIqZUWr3gBPskZOuLMRpZg8dBRbtKD/86dfEvAeIlYVRdiAJiVScXOKjsJK8FPnFS3dIQ9NYJd0NzHXpKnYW8N0mOBf/uG7OH14GptLd1Gr0JtjYc5ihcTCF2ocD49VFcF3y6oT7SgvK1hOUA2gX12Z/Z2Q0tWv1nHnwSN4YZ0Wfs2qg5T+qpKWHIh1p6xumc1a26xJC3trcxuHDx6x7uB2B2WgUFQCS4Xx3TwP3VNkpjjP6xHWNFVp6nrv6R55HsUMLSYbNLj4cmMdy7rX4sAamKwJEGI9K4wkSKuKTR6HRn1Jwbp37A0Nv3A0FvqmPC14Uh5PksT2vUsri1hfX0W9USNI9VDyXHo/ySJrT5d79Q2ktyRZguZkg+W4j06vQ9Cr4tbtW5jbP4du1M2Niq+jItbFFwyP8y+gd1q1npMZPTLfq7Kce7ZsUFCp4/jh0/jp+z9HPZjQxCnUFwS1OKXclAhyIb87MRnqmyzx2+jdqTapRRmkKY0Bge+Le49w+eFj3Kc8tnmuX62hT0+Pr7MFjYs42e/OpxQp/E3f9s+XXmnAk4GogiGBKBSWGn+1tH7mhyg1p2yJjmsPl7Gw1UFLHgEVTMT7NC5LilBz4knxCfA0/CDS2moM2CtnmKmX0F6+g/31DD+9eNJWMg/QQSPkG/sR3yuQza0vo9FCIx5a7frXXcPi5K6M6RlJec381Bg8YyoVS9G8I8qT6awOKpb6Uj5ULtb+8tw0fMfTyBk/MABW9bg8u0wdo2hMlSoDdKIO2nEXXQJAnGksWh+VwEO1UTVFHlKBa6Fgj+dc26TzYsVa+HbvafTbh/Fxe1LJLJssY0xy53XR48sGLF8EIl0bfeZZSE+rZ6OFwoTW3JcpDV+BjfW6zqWgoNHy5vb1S1YEjUbvKMjd47zrirFbdd3D2eOv4fSRc/AGBMakhMmJGTNYNjY36X1W+QxzLI+A8o65S2O8goS6JqLX1wtD3FjdwJc02pa7MQGvbtfoSKKs8cL2ZCHXtmvkqjVHv+zVoVca8DS4tLAMKx6tJhYozUbeowLUOJd+bQJ31rbw2Z0HeEjA66lqoULAo+iVfXp4Sc+GEwjg1K4cZVqdWEpJM6PEaBDcStsxXj/UwJ9/cA6Hpj2UsxaFmZZrX4NkXaGTQGqrIiz4FZuAM15uUKu7ryB31T05pmcjpaCmcErINkk3810GhryKnRyhcpVVJEvb9AVPqQpLi44GtMJ3NMgzUZF/OZsC2s3WRZ1bVe3Jy1G7sNquMs3LSHu/NllD2Azg12lwBVTYpQSdpIWtzgbWt9f4XVoGp0dA7KIjUOTz6tFXoaKsNRoM+4dAxffuJp0RGBnwsAy02ixLLKMa7iNQEOv885DKlsqtjBxNFqGpweQdyzO2ySNcrufsyGRAO7bl3ijYKaxdpBP5Sd3Pe11/AeYBQe/YvuN458IHOMwts5TS51Mn0UOjV+4A1z3oyntxRL2Tg15M/XR3s4PP7i7g5so6tigusc7ze1Ky1tGkeO9EURLugC6PUxHoK0SvNODtTBrLfStEKgBkZ0GFNmXYzaU13F5ex6Z0T00WFMGKitDzKe700gYEPS3bb72jKKxETiuHPhKkW0s4sx/42WsHce5gDY1yj8+4qhIFt2uWBgrnV5jZs1vcRYXA5kI7pmemmMrN2kbo7cjhMcXAdJcBJCPGpXzOylTt8T4BXtFp5bnItGfhqWh/91ZAp60GYBeeXbnMuJRVCZegFW2gk26jN2jb3JIIeL06QIVRE5cDfoPP8H2G51HZ8TlNtJUO0h2Da29J3+m2SmN3XDB/tbH8oOLmgXrFqpxmMk7cLc9MeqPUn7xdlTMNNZHXq9qaNFU606P+BrKkYwIWf/mhu2DMCHIrkCneZqf0DLe2Hmbs4cyhc3j39Q8w1ZjDxnqLd2kx22lEGp1uz5FlcOn78zAUpK3W4tfQCes229PVh49xh7qqp6prnuvxwwr9IrLH+LD9aWsBvXr0ygKexMDaRXILPqE1rf6ZA614EFbRK/sUoiXcpYe3qgGkQRX9gB4eFUYsC3sQoUYl4ksJMbCy2u8oaGXep9lWdGeNqPbn7x/AH7x+GPV0HZVk08baRaqqqtblJ1L8Rlng61jVF7m47/Duo1dTYH9XpBTsRQkieeRUQrk6cRdJDvSoGEyz8u78kg5VnanFX58rC0zhOJ/egZ6Y5+y8Yz+Ql0HAo5yWCHRhyPfWAyplWe4xgibjGEaIsIWN3hJW24+w1n6Mze4yWvEa2skGpZCeUdjnvRV4dcqVl9HboxfY2sjfs1f0pCznPMwCO2NeHo3IickpXlOPRdersXjqecgmnlA1I8NV2JoTszkxxbR2PSO/mQRcjKx4J9I5oNifO9KvuIixK+nUFwS8Uq+CIK3h9dNv4a3X3uN53+bY9NQORy/PPZtrCQGeOA9DYLZB97Q8O4+kPokbj1dx6c59bFOevcaEreYiUNTTZroV8puD3TB+rxa9upNHUwJUheHkIKOA0YLW+nW1GtolH0vdDL+8fBP3NrvYyErIaNGnFXUPJuQNEgJXH3WyBomnvD8jQEb9EuK4B49e3ES5i4tHPfzv//gi3jzSxKC9iMqgRyVWsTr2igBU5iVpWBSGW0eF7Tj6q21xR3FmTN+X0lIVj1ol/ONnd7G0RSUaTFlHJWsH0/JLSuM+AYdKUZOLS2FoYoGwEiDk34/e+SlqfKZEi/yr+aDj0XNPOabSoVRxX0qnuD56D2WEwOaGukjpqwpe7UwRWu1NrG2t4tMrn+LqrUv49PPf4qNPfo3ffvxP+OTzD/HFpU9w6coXWFlbwtLKkrV/OeBwVfdiTx1z+I5CwvaG9G4n9Q7wSQb4PG95we9l+fKqHh6tPMLyxhLzgmW1H9HoIGAwl541/spPtdcL7NSZRCtjBJSJ44dP4sKZ1xF6dJEFSsx/946R99iufvJrI5dcfpJ0Tt9i+ezItjrFDX1KVOmGd1tdgmwdk9MTWNtexermCsEsIxOmVJWt5xhZ2xorfAd4W0mCOg0B5WRnY9MmNZ+fnMBMs06nXlCniQf0je4ZixmfM+KmCPNVold4tQQBHhVYoJ5r3Je3RyUwCGpYbEX07lbw2d1H2GJB6PlVqxtPJDRSiBVa3xQmP42oFGmBV6rWZbhDTzBNYkxVSzg24+P/+Cdv4fVDVcx4bXp3W/acOrtoLk4NadBcea7API31zx+LqSNtncA72n11TN+HklKIB1t9/PqLB1jrhEA4jUjuG5WNWmmJLVR48rJpjzOv5PFptnpNBlwr1w3wgkqDqf88gJd7dXb85D18vdoQdZ+qMGlkCegePX6AW7eu4+ada/j8+mdYWLpns4Rsbq2gG23TGFNjkLrsx7hz9xYWlx5jdWXFZuvv9TSguoR6vY5Go4E0lsG3+50vnpQOTiEb5fKvs6VKxXpkho0qDc0Ort++gpI3oFfeNWPA8OQZ428GDLmszkcENnUiCb0Gzp1+HaeOnyMa8hzLvgPV3d6/vdjIfDV3xX6Kb9BP8U36EkfaFvtlAqk/CJkHCb8JaEzV6IV7aPXorbfWd96h77MY8NA9785r7bwe4652ugoNZy/N4MU0BJIIDV6bbdZouKlTnKuaVzz0zRaK+yfvfe6/aPo2v/0HS8pCCayblseRFQBjZqTlris4ynJZRBpbZ+PryAKuEhVDQI/LCyl45QDdQRUt8uN2huuLq1hPU8Sa8LlaoUeg+TGp8Ggd1+idqVorrRC4ghB+lTY/NWQNMSZLEY5OerhweAo/fesMvKyF9tYS36OqMHoW9CRlbathXDH8KjnhHNO3UZFOLv+V3dYWannuqMh9V6lTFHw9IXnw0I3lY9G657M+NYqndjsqDxsXZdXKzsAB8153OtlySs4UJc/mWmSEnp6reusOmzIT6+78z8LVs65IqtpJ48JS9KjcInSzLTxef4Brdy/h4yu/xa8/+xW2uquI+i1Uqhnq0wGaM5TFOsF8oN6bm3YuLXexsHoXX974xNaBu3Lncyys3cFWb5V6Xu14BeiK3PcU5UexcWk2yiJ35XmpCM1BgtrM5NGK3b6qFbWqtyYA2D+336oBPf5VtIDywGcc1evxq3F5WoyHVHyfAJNgwT9b3Jdhy5iZnphG1acBpGwv7tVTNIbcW0ZDHL5F13fI9kfvG5JF16iEbpfgVJ/CIKE8UhbPnbiAN8+9hYlwkiJHkDMPUw9IDuXVFTpPp0qYDJtItfpCj/LZbGKLBvtni4/wBY2cdaZZRBmVnCudKmI+KNFTiXE60CJC0k5RdkZZ945ScearV14Wemk9PGVWKsuXW2WX1ITWeSpmTdE5ZQllxlgrjgv0dgSGBX2QdW0bs/Ck/iSicA6PehV8fH8Zv7lzH91aiLYsSimGCj07hsFXUhjViO4hIopFnk/LuYc6leK+CgW48xgX5z38X//NjzDrb6PhdeH7BDmqLsow40JzjgXLo6cnBWvzGjKmX8dWJWFckCuAotGzryJJWVt7BNNSg23FrpFeacS0Y9oKtmxf58VM/z7Tv1eZwq+vbeDSnU3mHyVFAsW8DWQ1Z1IJVIRUIDJyqlXfLRNFpaFqr/0zh+gJvIGw0qQCKTy8gkXF9uupiJPZ71LeVOQlCET5DURgrd8mMKtOUukHXVy6/Qn+06/+Iy7d/QKdUhulGqNcSXl7ymin9OzUupwwDSirFFLJq3ppln2G2qB8lxMsbSxiceMRP7KP6blp1AOiIwuD9LMMOJp+mruc8dBYMK21JqBXYXIy6Mhizq38jmcnhaYkd/klKXdt1zvAp9NkrUOpLl4NenmNeojbN29ie3MLs1OzNEyUb1TeNCBFNh2c0o5/GpSualvb53V3j1Jbf8xbyoG2JSqGrJvRa6/hzbNv4uLZi2gETcSdGGE5NH3iosJfoYUOSIqf7ZpBNEwjd6945Lknmd9s3yowohVsHYloCHuM+/TEBDIaw3dv3UEtDHhrDnRM8oGvNtgKc5nH6QD12EONF8r0dnulDC3qqoTGeUQFuL61ifMnT6O/3YNHQJz0qvCpwLqU4x5l2gspc4y6UoORcen9FSpOFprGfaPaEu0hy7unPviDJUnuS0oSZWUUtxK4nJ6WBXn25PfaKduv1qjIWKjla5UaU9jIKrj8cBU3ljfQo4BE9OCIbbxXStNVD4hkMWlCsIgKSu12mv6onLaRbi7gzD4fP33tEGbDhHqlwwS2ilAKrcBWBVuJXgDdN4Od2MV8NxVXX23K00Z5unNUpKijIg2LtBZJwRo4Mv/izCNQSMGq3U5DSTJn0PCaeXhMYrWlDAYJ9ajCoZzQK1DXcevowDufzt9GeQwV/fzPlLyxYiw57aPWCNDqbeDWvau4dPNzPFq/j4RGVUnrf5Z6VIKULSq6Ubb2H1VpkkvETw1ViPpdynFEZZmgnW7h7uNb+OTSh+jE2zallhe4KfAUK/VOlFeVqKrNzrhU3YmzkdTGd/nO70Kj6TYEPLWdWurwtQNV2REc9s/M4+SRU5iszmB9mYZKJ7IelZrwW+BmQ0oYjNau1HhE1xOVJZXXqrWqjW2zMbPKfwLGRG1Sddv0fDzsm96PA3MHUQtqzP0yGqEW9R3GzH2/5YyF6Y6LdCn4SRoNYcgujyVbDswYAwPeMuWxTiPqwMxhnDt+nl4fr1E+FV+ND+wlMbRmY4nApx7mAQ2zkAEItLSOZodguMrwFqIItze3cf3xChJ6q42pOWgITtxLCKJV1Os11xGKcSiibZ/0NWT3WaxtM9x/CUlS9ZKS8+Zc9YQ7IzArhMhO5Tkk8bLJoWkRGtNCqfCGMi04G4fFOzSYfHF7C5fv3sH95WUKlVtgVdUdqlrQVsKnJMukNLn1pPwo81UWsIzKg5iIC+eP4L13z6Ma8r1UPq4qRM+qakFWsSIlBVx0WBjTs5JSUnlv2fwtZEYp78ydAttGVOqaycRWJy8CybcmS1Iu3Kqnpgaeux6bJRuS8G3d1r8TmeBKgdqv26dsaEiEvJqKX8byyjIuXbmEW7dvotVuWc9NddjQDCS7SREv2JHGcsmzKWbv0Qr8Ol5cfIzPv/gUDxbuIZEXSCdVnqxMfq0DZ/FgHLR1PEpffc+z0DAU9w7bV1nhoatG5BlGoaIGLpZRlZ35fQfx2oWLOHTgCKIuvRQq/bIG5FOLxYmWEurYDCnq6KJzAj76doiSyK61Ox3bpzlgzyb0fDTzSZ0e3anjp3Hi2ClUCXgZwbAykr9FKhQp8eTxs9Ewjd1E5UwJ6hmtl3f40HG8dfF9m2tTPTfl3un7zadWu65YX5EbNjKO1A8hkNfK+7oEtuWNbXx24yZWNFcsQa7L/G9rfJ9H8Jdui7nPD3Amlt6udM+Z+47cNQmoVa3aByvHRp96ueilBTxljGVYXjiUF2KBnVnlYgq2ZQ8FS264gM6jghOXZfEkzLagjpQW+8LGBq4+XMDCFr07AZoBHoGK1p8sQA0UdZ6dAzzlvyqBqPqQtjfp4fVw4kgZb1w4hLlZFkR1B6d3ZyIhoMzZFWYpUnmMJkFjel56ajJ+9aTwSiywU2FVJw4NNC4Kr8Z6CRgKLkggIfCQfpLaqdcbdvx8Bd7Fz3kLTlY1Sk5bAY96Z3Z7bQOl+/fvWu/fRrNuPSxt3Kc+5BuJ0sV7BOgG2IyvvAJxmiXY3t7El5c/I6A+IkDIlaASpXtLXM97hOYK/yufyBN6dZ6Gz0VKb4ZjoWifmx1WAWOU/IomX6YnzrJaC5s4efQ0zp16zUDPGR3OiNFQAlv5m6yQBPLKN+3LA3SL9ar8Kl34Oibz1nobNa/BcnsKp0+cw76ZA3wXgTBKbZovFXIXK9v9CueXnplsdX3mt8DK4qYOBjSMpyb24fixMzh+9Ix1pEkiGj9Mgxo9VEldSllQJyZVU6d5Oyw1CwEvQJWAWSZQRtRX1xYe48uFBdzZ3ECL+Tmgl5swsdJIi00rjRUJfYbbcV9LtnwRu3uksdzHFjIvwHs5oePljHVOAjvXEOsyxHl3OVtDnjLQgZ0nZmb7ZAM8PhNRwLzGNK0fD5fu38elhfto0SosTTTRZYEYDAhcFEADvIwFhkKkqhbNVqFXltM+QhasrLWOmQbwsx+dxakTE7QQl2khqjozdQJjYJk3svNPFWqKl0nbmJ6RmAFfp3Us0XNWGu+6xT1DnCOg0PpNqXSk5WXEUEYsR3Sck6zvAvDyi2gS8EY9gGchF7PiV1vJgxQ0FZi1W6VYXV0i4N21WfyDqm/zSKZZbD0X/ZCW/1M+3ZG7oOWMCvC2b5Phx+/RVGUhw7t1+yruLdyi97NJb5LX1TEri6hMBX4KwT7490ROZe4o2GJrrLLNOxj/gIDnaxkurTZAblancObEBXzwzk8Q+rUdD7BcqiAM66iSvbJvXloSq1t+xc41m1M0VJoGoEqXqBNjbnI/XjtzEe9d/BGOzB/nnQRLNfZzz5yv3OiRES1Rk3Ei3aLtc6eMPjI3bPQaxZ9ipuUweSlEszaH99/5GQ7sO0pbRG2r0ll8jt6rraoSUIYrKTLPgSYRnjoKCEohZbNKIAyxSiPh47sP8OG9e9aJpTw1Rb1Gj7cbo+67qcssHkr7Xeyy3+WPSwPtOS7Azl192UixfilJ+STQctWa7pyEsgC7XR6emAXJqjHJ2upsn5ZQh7bOwuY2rj16jHub6+hRGfRrNXSoCAmNuzw8VRdIABSyCVmva0MTZqolvHV+Hj9+5wT2z1D24mWEgbw75+EZ0FmVZu7hsTRpVfMxPR8VRVA03DqF6cjtuF/mG3f61F4aYKz870Za/JX3G8C561b487Lsqpy0VRUXpShXxubhWRve85NUp+TUlJ88PMqFbcmLS4+wtrbMeHYYR95FZdejzMnDCzXZOO/aRXaYp4rkjB9SeDbyeBKNIaVGVVhlFoJ2dwt379/EytpjEAfMa3CrB/Be3vdE6L8H0hukYJ9gnnblRJw3JxCI0h7TKa2wjB3ExdfexU/e/RnOnryAZjiJqJ1ie62D7ja91UQVgaHxIC6hsxVhY3kb2+v8Nh43wykcmD3M8voz/OS9P8C5k69ZtaaqN1WzG3gh07fmYiNjwWIl3VLEmMRI2vaZiaFaGMpz5gfzSG3HaUIgY/yR+Xjt3Ns4f/pNzM8dps7yrSPNgHopqKgNT/qLMqMZdCg7/SRFn55gP6aplLqhTzGB/u52C188foy7rW20KA+aRUo1V37Ryzgnl+YFF3kwZFOuTpPm2+GzLxO9tL00BTzmxNmBBDMHOgM7yyG7pnsc5/dbxhGKmPFR2MTtjW18eu8Bri6vYJUZGVdrvOYjZUFTrzXrzmvP6h05oFJZaIxLPYpBXxCvn57Bn//hBbx+qoFgsIZKukkLiu+XVab6dwGnCYoiIOvZdQhQ256EZ0zfn1xxUz6TWJAtt91JS1s3K4UMHnfGQE5VY8xPdViJSk38/af38WClx8sBrwW2tIom3XUWvUCI1rRkiUgobyLuZKhRMb5+/i2r/irnXvuzEWOssBVBxc1inSuvkjqjJPj4819jcXUBcdZlvKjICEiJOtfYHLCUG8ZL5cAFUlBxzPAYvClSlQ8e6FjehHbUEURtPr1OFzMzszh8+CjDI6io1kJjaOxmFxL3LDzHTuGJLP751WchF+Jo6A7kFLr25UXLu7Zpxfi9uiLvTMvl1Oo1HDiwH5PNSRsk7pcDAgFBTh4O89dj3ojLfeYRAaRCg1Pd/Q/TYzp/6jW8RiB54+w7mJ8+aM8L6PopS2qJzxEM9D6rDuZb9a+yb55dvi8yY1sXn5GUflbtyu9zRpeTY80kI4MqCENjXdra3kCrs8m8Vw9azybKkCyoh6biQcGmMaAQ6PHyWc0NWw599DJ67bqPYtqgMd+kodQgYJZUs2HkPqj4Cm2HPPx1ec49RkYly+07qX2ZSDF/aUnlTSxSnjuWUOqC/TtWhvKUMkpzVGa01gV46ol5c2kdVxcWsa1HGg10KXwdCklQq/Ne12ZnnU0YhgEn1aUWidXk0M1KiolShDdOzeOd80dQLXeRRWuohRSmvtZJdz2hDGTNOxQPhWtMz0cuf13eun3xSCG0PHc8vOIKrFhLqJhXJ+9NWVTcKxmyv+IcgdOQwnHR088dPwflj9s76E3qPfZugR5lZ3lliecyVGuBVUXqvpAKUEtQaSjMbhrGr2BhlpSpSPFXu5w6vOgdqhrVGnBrG8v08BZdOx4F3Ko8BXj2vS5uQ9K50TO7r35/0vO7wxvNO4+Kua+p37S8DsHMzX5CZa515VIfQb+GY/tO4ufv/Qn+qz/7P+Bf/fy/ZDl8HwcnjiDsN1CJQ0x5Mzh18Czef+3H+Pm7f4I/+uDP8NO3f44P3vgJ9k/SaCEg9lpaj9J5dhqekfGdWsuuiI1jF688dr8Dcmmt/LUesswnzdGqNsSKQJu8Rq9UQ2BeP3cRB+nlheUqal5dcI7Odtu8cEmKzaBDefSor1TFW5a7HlTR4XF/YgIbvHZ1cQk3lpbRpTGPahOtWIMhZPwxDjKITJ6f/L7dgOaORmTsd5cYL4xeWg9P0EGP3tJdFrlru3PbPDtowRKwWFhkrFckBBSGiPspzyfM9GsbHXz64BHubGzQ/a9ab6atfH5FLc0v764fp9arSUsBtdtbNJ6ogOq0sLtbqGys4L/647fxr/7kTUzXe8g6CwhKbYTyDCjEqgpVGyBoaSpGVs2aew5SOmMP79nJ8pgFTltTziq0PHJ5rz2ms9pHdJ2/flBDly7cgJ5AWJ/AzYeb+MdLD7HRq9h5UKHaLPRCQCki5Q8BQGPRZO2HlapVh81M7rfZOGYn9tOqVjXUs+YfVZ3aY/iuAjxtn7spjaUo7eA3H/2S8eliQJmTdxelES16KjUCnoYOSMm5Ly7IhVOwA+lR4ncqbXjaYxkoZ0wXlouM9tnFi+8g8Ov8Xt3C75eHsPP8MEwL1/60+3waT3k0fMNIuPlWHpbaTq1DBwHBsob7AgYpW8IDfP0RHOp+E/um9uPUsTN46/V3rLrzZx/8HD9+96d457X3cP7kazhx8BT2Tc5b13+kahdjehFAFZ4W9ZVhKvvA4pEDgCJSGNDFschSmvcXMX4Wki5QGmtMoEIsFoEthrzIk291tjE1PYH5+X22HuCjRwt6EmGN8mpTzVE+qU4CASV1jaZLlBx79SraaYywWeM3ZQRI6itm9PzUNHXVBFOuZF6ijJsyDQvNNKX0lpGhNC969roPdqxSZcJjX6+8lyGo7ctDea6+3GTWOPPBBDMn7co6rDDzJUAxga/HmzQxdOSHWM8GuPJoCQutHrZZ8O0ahVzLvgRaC48Zq7EvLA1UOhliCk81KKPhUxCTDvx4C2+f3YeTB2qYCmL4/Ta8fkTPLyPIUTQsKvohG9AVNqIER8n+zyLp95ScsslZyWxnRTrI036HpMQkC/TRmdeaGV8dVwwCmNeurSZ/gsDjgh2GUVQ7STGKC8X4/OTCkH8nMikxy5/xoxLSGm32bgNDeaa6R0rRVK4948gi/BQuyD3pmF8teSRLYcZJbGfdfJs08ux9RfjDcIahFeE8P1koFrAL05XhfDuS/rrJ/vKaErXreRkN0bQKnyxvzycXx36WM/ftHK/ZPfmxnlUnNNeu7sK0MilFwiNRHpOdPbFFi3u6b5gez0oudEcM27b6RseEYjSrE7LQaXDVcf70Gzi8/zg0X4Y8U/UpUHWmecGqomS+aeJprSGoFUC08kuLBnyXYWo8ntbMu0QD//baFg38Bsr1JnoEubYWnlVbL8FPbdoCv2KZrFHOIe+lJn3VS0smmyJuJTqWLfk5iaMEQT22NMN6Lx0g4jatNrBFK/HOVgufMfMfdyJ0qQgTPqheYSELfMDC3k80RotWjl9GrLaTPq2loIRgwG3cwnxtgD/50VmcPz6ByTBhIerQgkppdUoImaxyNyUiLCGqtrC2OwEfz0phqeF4+AFj+t6ktGNiFspBf0NyqaxfBx6UBR64Iqu2GY3B01RxuofP5da8Az73pMbg2RM8Z9csMFedqY4g2n8+cnFxcVJY2ioq/BK+0wwugl2ayvJ21rYBHiOoqDkvoCD3vIv8CBfnRykHElWf6k/KLaKsC0A1iNunwkxpHArUv/q8i622vwvSZAwWVe4rRkOwU9o71nmrotF2h/QQy5l1JmN+ZAUHPCYPCIQ5a9/O6Vpxn8CCzzqge4IsPi4uw3jwtKLhLhvIfOW5Z6I8FAbu9nJpkMGhz+WrNRBeU49VvQZeO3sRZ0++gWp5Eoh9GtiuB6tPD9FmnWF6VtS+SzmRgS7wk8MOj+Bea2IjznB5YRFXl9awwfzt1xpuDDLfN6Bc8wFbD1RDdWzhXf65dH8av5z00gLeTtJTUixjbOvIKUFuDXRUT+1BqxlkYQ09enALnS4+W1jAw24P7QoBsVrns1QgAkgKmUeNmEY9tf3a8LkYVDrllN4ilWRnHROlBO+ePYx3LuzH4X1lVEsCux5YjFjAXEM5WKBUKBU3V5VJK8q2iqELuBDzMT0bSe18VfUUUiAa7heAZQ36NGwiWr5qp5G3J2WvvBgdh6dnnWel55idlAmd1+BtgYKF9ZzkvDSxqbl8q/Oq5ipT/1Ap8YysbkVE1XnSawLsIs5fJReOsQnbyD2FFjUWqLtqwoqUHcPT4qqaI1QKT9VsVhMh423Xe4q3KqxnJz29w4xXUY6HoCfWh7q47mbFSLEgYMlDEzhzqw4b6mqmrfPcdp/TPbrXrvHMbtkZCV/vVhopLuSd3Cn0CtlAT7c/D+3KHwVWvIvpr5ynoZNQTjUBsF+qYrI+a6uknzh8DqWEeiatICTgqe3RkxGmmglml3rZqnagIgCkvFuNRCVAh9/9qB3hmlZJf7iIdbXjGRg2bKaWiPlvY4wJdpI7Rc3ywzhPD4tnnidF1F8ikjS/pKQCoaSnaCpjdIrboVCqMDDjKCw2vCCoI2amL/ViXFtZxWcPH2KLBT2p1VAO6+5+CkCg3mtUBDYzC727lJ5ZRhQs+apiamMQxTg6XccfvXMO81MZ6v42gXKLIJdSKNUzrErZqDJCGgBLpcTIDGxwaEY2MSIL7GShv4QS8wOh0ZQr9p2yMEnYIQdeTHOBl5QcFbsAo0s50Bg8KXwpe6fStE/5kVfI+101pjtvY/F4T0iDSb343IiF0Vh8X9KzBeegJ7llXBQHKa9ms2ntOGpP0XUBnraK8058n0rFNW4tOYpjHeRMuXRgF2B6Zo5nlC5KB4EC32MeZAF2BZMsvCKd7eDZSFEjF2X3SdAbMstMwVK0POdKvf5cGMXWxshJ6UtpKw9H9u3ayL22zX+HR8N3ufcMWYdiAd0Q7BjQM5OezVkR2nmXDCsJF+WNAqFetB6ND/UeTToDHDlwEm+/+RNM1ecxUL+liHFjIpoxxENNpybAq5T5PUlMvcTvoSEfRRlihtElON7aauE/X76GO0srfLwMvz6BmHFox7HNOaypyxJVpe/EaSRuxsW+ti8XvcSApyzgXy4vJjM5URwpkCWrzhyoaltWnTqk0KLXeLsrjx/jMb27Hq11my+T9+spn88EFJ4qlU5VM03wtADPxrpUpBwjzE2W8OapI7h4+jCCkqYTW+NNLQKk5t1XN3Va/xm3cF6Ai6PAzoGeSopZzmPAe26yXLOymCugnfK368CAy7YCM3kyNGoiGi6qwpPqVBWQgVwObvZ0/owNOCc5D09TP2m2kt+Rh0c5ELhYNSVf50DMxVHvnZ2dtV6Z5tWRdb9+rbu8i96Qnjy2sEa3I+S0tYURVms4evQYH9c7WRaoHEvcundIfp94fndCPye5NP8K6GlrOeP+8qv2bgM9nbV75ZH00ddk2VLw6mS0c8y7yNq3mZeK6zy2Z+z5Ydj6G37bKBfJ5Uqt9MoO6On0c5FCGAnFXuTATsBivTgpdwI8eaVJ1EezNoMzJ16z9ryG1yTCwebIVK2FUkxTxGm8pu/Rr2V4vmReiSp9E9SAxiQ2mDC3VtZw9e4DrGxuW5VmwleLUeF9lL2M+szlQ542ZEmDS5OXE+xErjS/hGRJb4Uj37ezjgqh1OBKWfMSVa3++3hjE1/evo17q2sYNOpIAh8R71fPPHl4gimPytCmH+N5zdSeGeDRkFKHlMoAF86exDsXzmK6WqH1RMDrtxh6TAtMjcy8MaUCU9tCObT3SoiLiX1VQK2gmbhY8VF0x/SsxDx2f0/S7sIoGRFgGBFMpMw1j6aAT3lgYMc9tYMIbEa9QrvOc67dTNNzBXkb3u+C+F5Fjv/2fh3n7xPPzu0zQDIwJNv5XG4sanpwF+n4yXNPknuTfpUmtVoDR4+d4BEBj+VF1byaHFvlZic8xdHIXppvi/1npyKUr4Ce9rgtqtEMmMT5+R2wE5gJwIwzd1wRU/nbdri/6x7jXKHnvBP+U5mXxdx1W+aR0sRdeg6y0NyuUfE+JT63FLpqGFjfAs0cE/p1ZDFN6UoD7178EU4dPUVPbxKahUUyrXY3fhnzVW2+mqVG3V5A3VS2PC37VSQVrfBCI6rWxO2Hj3Bv4RGW1zfM6PcCp7NUtW2f5z42j5NjAbHt75x/ueilBTzJiVMQTmRovBGAaKBYXug8lRQVU0xXX0v4dGglPWp1cGNxFUu0iPzJGbNkpTlcpwAFJOu5bx0FbC4+Fn5VbQYsXOW4jUYlxoWj0zh/uAEvWiVApnqLVZ1X+KMwMoZlypVCJlIcFcNCNFyh4Vmnscb0XKQ0HEnHvBAyJ7hV+it/VeS5yb1sne7TIElTj3lVoSVrtrQLxZ6n9cq8cbmmP4IMjSEBk/40GLqYdeV5yGLqXpL/FLF279Nwlsn6jPXOUwcLaujiZr5bVZwUdouDO+c2FmrODizErpyQmRYy5TRlnqa6q/RD1CpNzNsgeipFyqw6a3madJlewk5YFrZ+nuTnI0tRpWueEEU8Re7Nw2ORvqTYc/FywKfzFgTJUtGi7P6KfZHucfdKoRdhjVIeyC4qnt59bZh3vwNiOEVsHAC7rSbBVpuxhhrENNCqYQ1RL0a3HeEkwe70iQuYmTzAfNMUa74Z+DKJlHcDYxp1SWQTjasTVsRwWt0ejXfK8cQUHnQi3NjYwsJWF5GeD+kxqrzQ1dNYPyWR4iVjZMj8sYjqJ99/ieilBTwVBK1XlhFdBEzqbBJSUsoxLZyElhwzZE2Dc6fownsefn37Dn57+x5afg2lyXlsdghyvC/QOlTKWaJlUqZgebSm/DIqFLQ0SnBwYhrlrS14mxH+7OIp/Jsfn8HB6gYB76FVgZZKdQqWG8eS0gssaW67ivZjCq0EQspRwkOm0nJTnGmOP9lUspbG9GzEPMu9Zit7RlLySlPmS99HloQ0Rur0yKg4Sz0Myl3mDy39fg3tjo8k9Zk9ocmKhqBoAC/VjCkJGTBq8NdUTRVVT6eUiXKAieakQufrh299VlIIGgqgQefWwUDhpoTfrAxCEU4eoqzNHkNYVtf0Cjy+Xx0Z4qSDCuVUhpYUnIWlKijG3Y3x1Pg+hulpcH1q3izUvuw3eGeNYK+5KUNU+5P4w3f+BLPBDHwtQtrtoFEloJd4/yCmbCssl75mQDKGDjTFQwB+FhLIyUgdrmHpgN6MCwvfvaMAPcVB6a6iWrBZlspueTfcakL4UsawtNSO8ivjt7BsarajihkRxfPaPrlf8DAejofvV3nmIZW+QEnQ+bxkoeZhF8x8sHfwO3wfPRogGnsp4IujmKDmoVmvo73dxXtv/wFOHX8DdeYfRQJZL6OB5KPGe9UiozUSmQjqhWfjOJVloa/ZaCrYjjK0ZvbjE4bz6/uP8Vj6kMaP+iB4NAbrthagS3+16yUML6XAuYnzlffk50+AF06K+UtLKQuzLG0pCtk2YcmjN+ZRwVGR+QH6YYBtCs/DTgt3NzewyoKfEfC0LHTKgqGB61ouSOIsIUuZkQkz1DKVf1UqjM7aMppI8OPX9lnPzCk/gp+tY0LL/5ikut5sTmhVEGQ9OgvSiXMuHGQWJ/6p4Lo6+jE9DymtlcZKZZeqTvspP0VKb6ukNhmxYSE0RJjTBBnn4Zm3Y3nnnlCGKueN9YiUHZmqx8LTnzw8NyBb731+KqSk+AK915ajoqLW4PZjB07a3JECvKSb2mr7k42mxaif0bCjoWUN1QqJ2zSNEUVddLttBD5BkppPX6DZQ1INvKf2qpSqCL0m3jj7Fg7PHUGdQBhWqCSlyBhe3+bbzL1Ii18eOyrAQpbdueej3UCjWO6kwtewo91H2h+G4camfZV3v6OgIqTvxi6nyDzU9ndNO+Hbu3azi4Mjt6fvCXDy2HmbiWWyPo2UgCdQVz72aLwoJKeTHO/EmpuM96iH+hq158NWF3eX17HW7tEgJOAFMop4E9Nt+F73/Y52x+dlIn3RS0qqinSNp8pgZbTquqWgdDbTANpaDSu9Dm4uPsKdpUVs0Z23ZUQEkZrnkjmoAiDI1FL4Ga1gbZWxDAE1WkdZewXH9tXxZz97BxcvnIQvT5BeobxL3jSmPaJRJTAsiCIdOVZb3FBpOBL4aXybeT07lIclTZaH+SQprGKZmaKN77nIXsVYjQalQ8ah+AsJbqdOnMbr59/AzMQs+hEBi56YzYgvKVYbjTonVNSZpkyA86zNp1GvodloYHNtnUCZ0Wur0nijEag2S8p9I6xh/+w+vHXxLezbtz9/Oe0BlRkagMNZNsb0QyXJieYVPXHsJN55610cPXQ0rwGgLpQ+Yz6bFAm0crm2UmEy5+ROeazajdWtFq7dv49b1JHbvCulh9hjOREoKowKw1QNmhwE6UwWIv2QXz56aQFPWWhtZ8w0ZYtmEU81mJg5KE9Nswt0ad3e2VjHlcePsEyLJ1XvIxmttHZ9CoVr71PWaZl9Wv0GehWTjwpSOgTbODjp4YMLR/H+a0cx1+S1rAt1ZokJei9nlv8zIeaRK3aShIJG9yUfKq4qn7qTe5QLVRnFidaai/JnyZaR+f4IGWDyWbH2NURArB6ULsznJfcFjoaw7HQSrfROhLmZ/bj42tt4g1b87MQ+IGL8WzTEuuq2zviqzTmOEfN7xKmAnEJeInDpetXTtFtVVDIqt26MKpXi8YNH8e6bb+Pg/EF+j2/rAiYsO+oUI9K3ffPyR6PxHtPeEPMzVhVmFYfnj+D82ddwcP8hVVMhjTJUAw21ct69KwV6hLKsv3yr8uCHVfS4f3dlBV8+uI+7m+vY4nGsQevUhWrTM6+xT+YjNouUPa3nXbAvE73EHp4UGgsmE73EQi9TReNR1BFlwAIeByEe9Xq4uryMW+tr6GgGgkYTiW6lQNQrmoFBMqDsE+D5zGDNTOB64GlyaD/ZxNsn5/AHbx7BgRrBtLXId3RsfF5P1al255j2nlTyXLEWWBSkOTBFUuAaZK2iqoHmMfOuQyXvnvt2KgBPHlABeM9PRUS1HUbaxcgpKdVUypNTp5K3LryDd9/4EY7sO46wX7MxWOpYoC7rGhtY9TW5smbeoFFG634Q97F/ej/CcojeVg+dzQ5q5RpOHDyOt86/hXfeeMdW95aXEEdaUJbpxe9yIF/EpOBRGsZ1THtHknNNr9bZ7kJj9M6dPG+rQFQrNUSdlEBYs2pxgV6Rj8o5Nbe4BbLlydNookE0IOitpjGurS7jEr28R3GErF7PlxLyqCcphXkTkADPGkwVxksoCy8x4DGxpYi4NfVA5VZhofeqDWTcqlfmlceLBLt1rKgKi266VjGnc0fEc6uVy8NzCSDLX6CnrY60GkIPh6d9fHD+AF471EAQryBtL8EvJwZ46gJscjSmPaanKWWdoYdjp1ksTYPzQHkswMs9vOGzo88PjyVTetbKOPflDWnF8N+lh5eroZwdFe1NtbBO44wKKgYO7TuC9y/+GD96+2c4e+wC9k8etFn+425q19Wr00NADuEPQptCa2utja3VNtJOH/NTB/H+mz/Cz977Oc4cPUfFWKec03pXmyTLits6VlTUXPD1tDu+Y9oLKtHACRF3ElsYdz+NorMnztPbO2bTkGmpo+EanCNq3ixCB3jq/WkroFOu4zDAYwLdleUl3KCDsEpFGZd93unCcG2h0rQi9/zLSC/taglGLJSqsdafVkIoaZwJQW2VlssDend/d/kyFqjYtlmINcuKZV6/jMCUg8asaOAxg6Ei1MTR1GpWlRkOOqgPtvCn7x7Hz988jKOTvKu7SAs5gRaa1oS72Jlrbkx7QTuwJI+EueZOMF+Yo65g0qAhAChbtfoAjVSbPikrTWBppYzffnkfy3GZhTofRC5ws60CdaSBv/KANAu9wppszODk0bPYP3fIOoBoKZZCBXxvsvhqx96aswvNTpM0c70Gx2t8oMb/NepNxmESE/VJTE1OYau9ZQPhB7S+BXwamKwVHTS9nU/PzkMV87OHbIHTt15/D2+efwfHD5+iF1gzsNQ3adYYvVEArvZJjTcUlsubdTSM3U7Eip2d4zG9aDIJl96yTnc04GmIBQHzkgftbhsbmxuUX8m03Wzk2u/yLXc8yq8mGhAelrXGHstOu9e2nsA+y8P85DR8yoNYzoFkVc9ShXLrgn1m+d8jerkBT4VUFimLrjqbpLR4WizmD7pd3N7exi+uXUc7oBUUVEF9gCwZIKAiUDvGgF6fZZ6Yzyvf1N1FQ9FrgxaaBL3/05+/jdcP19EYUHiiDUw2CZqDFNvdjk3MKgEY096QK2b6ZenL88+64zMPVQit6b7EPOL5ZNAl4PEKjZ4UArwSPvryAdbSAvAUBpU+wdMeMIUgwCtb70abmb8UEmimceLoGQLeQQOX5wI8IyorvW7nr9h3W80G41sVKj1TAh8/DY1aE9MTM5ib24fjJ09ifv4g6mETttApWT0up5qzvGcfLr72jg1Qfu+tH+PEkTNohFNMInVeIYATFDVJtAYkC+BSWvtadFRVmnq7gZ8lQ/F9+dY2/ClOj2lPSCKqKm+bOJrH6q1brYZoNOrY2t7C/Qf3ndHCfBI4WX4VnG/qYR1xQg+R4h80aojpAGy1tpHFsVVfnjt2gmA3gEdBUFWm9KVmq8n0cgbgZDUP8CWh0tX/9k9NrF9GsgHCtETSRBM8+/AmZ7FJi/UXN2/hL7/8Ao+Z4VvqccRz8gKCrIKQSq6ayAoe2Owp9elJbMcpunTn1fESvTWcmB7gz989iv/mX17EZLaERuZdE34AAKabSURBVLZOW7hjGW5DF8zVV1WBE5wxvXhSutvgfYKRDS+wwtjjeSpueXiawDurouJX0O1voFQPbBmoGPO4eauG/9v/6y/wuDyFtqeJw513tzOoVm0UPKO18GpBHWUCRHczwdsXPsC/+Pl/ienmAQT0kmj7MA4Cy2chKRC9R8T4mleab/MSqdjwLr3EfV/+jLaaNST1YpsxRHe5wfLuWReWuuIX7NpyCnbdzV1Y2g5pVJothd2u0ei+SM+NPjumF0nKY2ua0Qr9tppLhLDhoe+luHb7Ej758kPjcIKGUDBAJ+kg5j2VgDJA4y9L6MXZfL8aY9fP5ShF2E8wQcCbiSL8i7Pn8Qc0qs7PziDdXEWvvYGgSlMyLNMYi9yUZ1+Rix82PWtp/UGQrNCMCT6gF1dqNrCW9a3d7trSMjaojGS9a6iBFKLNIEAhUQb3aaXIs6vWq1heXkQ/7uLAdA1h1kJ9sI3Xjs3gR68fRzhoUyh6fDrmW1x1mQr5uKj/UGi0sO3OFV3Z6WXJfeu0QmNFbXiRqgm/Ivr2hNsdCXe0Dc95QHyOxzr3/OTUhYHUTqCKp120eFtcBFA74EUlQy5rtpR+lWBcc9wXMPO4r32dD7lluSCX+1r7LeC+Y/MG9bwBn8Xga1g0uj+mHxLJg7PmHOk14lUSURvSXds3cwBvnL+IU8fO8riCzdVt5nuZHl0DfXr1UVfTJFIfkpzEqV7ETb+YlDxEdBC6lRA3llZxfXkVjzpdxGGIQbVGLUiwpNenOV5fRrl4stS/RMRMYkanLLQaNxJXq3jQaePju/dwfWkFHVVxEuw0MFy9jDSWROrFvLRyHymtGi2DkSYxgnKC6qCLMFnD2fka/vDtk3jjxBzBrkM/Lua7ZFVLJTl2bx/TD4JGMkK7wxzKLxFEXK9K5n65YjLT6anyRrLgnvgqi5yXVACbwpCCEbtqv+cnGwfK+IyG5l7nQG+UFR8DaQMpbQl6AjOBnLGATVuBmoBOW7dUlXm7fbVfCyyl6Aov70nWiwoe0w+dbHknGeGUTc2Dmhno9TFRm8KZE+fx7sX3Md2cQ9Lto6SaLY/yQWHS6glq/6MQWDiSQFez4GpLpDdVK3ZzbR1fPn5snVi2PToNzab13Ew0hpnbl5Ek5S8lKau0iGdfGeAHWKaLfX11BdfXVrGc8Hy1TlFQgffg08rxCXhSMLL3BXgJuRP1MDs7jWolw/bSHeyv9vGn753Ge2fn6dpvmXcnz06zsMj6cWP0HHCOKtYx7Q1JBox3MsIVYG2L/JGH5npb8oBbllVs02J1V5+WgzxnARLuCJB6znXooAxRzlSroGs69/TnvxvZGxjEKDuShArw8q2dcSxy8c69Pcq2rXy9w4X392T1pfsesUsZ0fDcN/OYfpDErMn6qU1pSAF3vYepn+IoRUrQ80shzp2+gHOnLmB+7oCNz0t6qYFePaxRdnKpopypfU5AqJ6YMoZUKxYT8NqU9VsbG26oQq+HmI5FxveoZ6fm9nwZ5eOlBTwltpSXlv3pcP/W6iouM2OW0oTeHl1vjy43C7+XeQho3QT5OBJlsLw71Vt304hGSxVZbwvlCPjg3D780cVj2BdEGLSXUdH8cyQNSJdlYwPTzTq2YEj2M6Y9IEt5VVlaoSuUuEh77kiOmHBJwOUWdyXgZX1stzrm6dmzObg9jYoqUfWEE9BJqVhHAL1v+MJnpuLNxdvtKyRYBdhxa8vd2JYWPVnRzb9uR0kN54ksuAhzNCz1xlNHLTH3rS1QtxSBjvCYfvCkPC1rdYRB32YOMtUmr43510+o4+K+TTf25oWLePP8mwgqAdqbbfjlAI2wibjnaq7UECjA00wqGlyu2jDNl6qqzcHEBBapTwV4V1eWsJZRH2r9UOrWJFab38tHLzXgqaekVkRYbXdwg6733Y01tFRNqdkDmB+ycD16dn4mVkcVPZd7eDzQ9GDt9jYGcQ+vnwzwJx+8hmMzPkqdZTTKMRMnsfIvz05ufiYhUBuOvd2pnTHtHRU5YNuv6Gl3VQpB1ZGmEAR8BK/tdptX9cA3KHc+vtMGSFYYBeD9Tqo0FT0KVwFOoyG6SwKpJ3nkPO/hhsrKbYdApytifbFTaDaPqIEcwa6sWhHVWhQK6ynfose/lorwx7TXVNEwhAoNf3p5sRZvpTHnEdiCoAq/Elqv9MMHjuDCudfp5R2kqU4w05RjFAvr9UuSvAjwpBsrBniFl0cDPwhtLuL725u4/GgB9+hU9HifT4eiqOl42eilBTwVOXWp1nIXD2h93F9dwToz3RZ0JcdUbAI8ZaC8O5/5q4mi9aTWxBLo+VVaPe0WDuxr4I9+8g4unNjHe7d47zbqvjXj2nss83PAs96ZPCdBefmy+9Ui5+Exn4qtgI8ZatOKKWO/llwO71SFcl9eogaeu4Luwns+YvjF75NBCbl4chTsnJeXe3p2j/2TJYcFa0hGDnTSagZ0hVeXjjANOVuFv3ixQholxenJc0/S837/mJ6HlPpaPkizK2g9R1NtJJscgXpReZjSC9P8mgf3H8b5M+cxv+8AkihFr92zVRMUigM86cbCw5PhxHJC2W8RSLNqiDbDv7O8jOsPHmB1a9veVZSDl432FPCsK3XOBWlP9quryhEXBVZVMPrTDepRpDrmBh51aIGsdW25n0ztdMzCkpZ6MUVBV5/PpczRjCxlIVKmBhSWBtqoE9zOHZnG2xeOoaalY5IuqqEWjlWvTBebHYWiuLoYjOkHQJY7zOeSVctQ2ZuX4xreZZgIIDR2QJWQKtCajaTP691UbbgOHgwN87DctpCy/IxKN1lypYZ+kzDKn/VwLB56RiqkSdtR2jnmRQNEkn535NDO6Mkc3Ir9AsB2B5eTe9PuZ76BLaynsajYjmkvSeM0ldlqW9aUdwIhq76npxdponyWB80vPFFr4NSxUzgyfwSIB0i7KZphk9dd9WUhVcpV6UhboohyHmlyBb+OMu9d7ya4u7KOB60WVikb256qPfkueY35s5ILK1VWFrnPk65Mqqw4Vkkafd+Lpj0DPCWI1oVza8MpcXI1w1QqVieONG9lqGP1qovhBRVrLG13InjNeayGB/DhUopPb6+j3a1gNpzCBI3aIO6iVqGbT0DLmkC73scaInQZipRWNS5hotNBc2MR/+qNBv71BydwaNqzcXgZBabNMLJKYPGRl+cNIvha8ZwhluEsYynLvciwMTmSA6IBszJkSvRYlC/qrIGB1rermkGU9GOEVcJhp4VSjwZMVGEeT2Cp1UbkV2gIuTxU4RPwmQS6Umpn46iHWhjaahylbICDavxPANrQNktFAUbPQoq/lIuWotJWYwClaBSXQkk4LtrqBNjFVuVFAJe3xRkr9u75Qrm4oTjqvKJwim3BfNy+14XjToywrn0tj2mvSbIXeFUKDjWUqimV38zGLKGc00DT3KpTzQn0KOsxPbqTh4/j/Tffw4lDJxDyr5x6BEM+U6Knpza5Shk9ykFMkUkll5T3utdEv6dCVkUlmMK9zTb+6tKX+OXyQ7T2TSOp1ql36QVSLjVMQctKDchV6ukynQ6bupHxK9mcnhogL5b36GReX/GiiV+zV+QKjyyCpxciFWA1xmphQikGWi1alicI4dNi2aT18dHdh7i21sJ6UqLhQrRiKpY1FZQschbacqj2vU1EBND67JQlcq9ND47CMU1ld3wKeO1wHcfmqvAHXVpMVIwVWvK0mGJaScoPZ+uoq0rhReSKhZk2pr0jSYzy05lJlJHcw5OCV8HS0ibWZZtevvY8XtPsIllGo4n7WvfQQEZ5aUzayVJ3RoXDWarOu9OsPg6MHI888EykIBwP4+HCdGx/I+/bwaP8DsVzyAUVVxX7US7OW6j2t/v5p/GYfvDEbHJ56Wi4R9uMzkFAMPPKvk2SoCWm1Gvz4NxhdDa7NuuU5lNV1b2qRVlsKIvOgLK2aurJMo07qNwQJDvcLnUzXFlex8d3HxAgfVSqDespr4Vq9XItnKwJGzQ92VBW+StAzo8c7w0pFntDSgOyU1q7qbBmi0ZWjfnoM8G6BLMSwU5DDh5srOHzW1exsP4YHXpe/UAzoBD4mHkp75eFX6pUIYMno8liGcuXBXT7q3T/B2mEsydncP7scRw5OMvrCZJem8IxQEjrP0tVXcCIWOSGCsnUoDJPkd/DjBuT6Ol5YFlDEuBpEmRloVaN1vkkzWy1BK00/m002mlFCkDVRsW5MY1p70kGXc7ad8qKREGnsEe92DqxeBV5XyVMT+/Dm2+8g9OnztMhUC0F9aKabvqquZJRzzAGqi1Rk5DKDcuPgiwRCQmOfVTR7lXw4OE2vrxyDxvdBKVqDX3PR0+dYASSqupk+dJjro1YJim3I3Esqju1/6JpzwBPnyqwK7hQUoU1q3a20Oa85Em5w6qiYsJnYQ1bTLfLDx5gYWsZLa2TUuf1qo+4rBXLVblFwOIzArtmY5qIN8DW+gZCvmMi8FBJe6jS8n/zwikcI9g1qmVmvqsWkyfn2oRcxhhZ5MRKrlEe095RLjBGxb7yy+VZUZTUS1MgpR6aMlhUJS7+LphlvTtzYBTYqZfmGPDG9MMgyaBkUwqS2xxchpJPT4uny/TuiEQEPC0QXMeB+aM4eeIcTp04a05FX707o8iAryLPTuERAKUD1Y9Bw3kyGviDUoCSP0FvrolWt4LHqz3cXVxFm4jY90M6GRUWEnqS0r8Zy5xNrs/YMF6OFVfHqjVz/OJpT7X2KOCJlUkGdjkHBDxNlaPMAq2UQVAnwJVxZ5MWxsMFbPYjxCETvFZBRCMkYhiaA77PzNHKCHHUR7PWdN5imqAZlJC2VlDqrePd147j/MmDaPD5NNrk3QnqRMQSLZw+vb+K2oZMRebEMJxbLsUp1v7O1TH9AMmBE2WLXAwi10rnYo3D+7b8E+A5D69vYBcElMcc8LQd05j2jijMApERIBkFO6n2iuYQ7lOGtVSIqiVTAZ+H/XNH8cF7P8NEdQIedVqmMXn0ytTVq9R37XBWB8pjeXlyAXR1UK5jUJmgpmxiO/Zw9f5jPNrapt5l+FpbkeCq9UYVExmIKmLS66592QGfAzqFOBrfF0d7Bnj61GGCDD+duwZQauzUkIKKskFpT8DTYPKFzRYuP1zE/e1ttJnZcbmPiInXzVL06MkNmMlluvHWWM/AsyimNzfAXCNAvdRDxucONPv413/4Dg5MaxHYLtLuJt8TIfSZIYOYQqG55hST0VhZzHgq5zHg7T0ZaA1zRwaKyzOyCpfAyfZ10VmePVq0ScrCxuNvo8LDE8AVHp4DwfyGMY1pT0mgIVnOwYMyPyQa+ZpLWA4bHYZyKaQDMECnnaJWncLZU6/jzLHz2D857zqx8B7Xk1k60AESRT0f8jCwYV4R9amGZ8UI0O17uLO8ittLK1injjUvjzpRs7CUNWTByp4rouotXVRvKr664vy/F0/fXup/jzRMELfVsdJFLNAbJBmBj5Y5WV6bpnG+xUS+urSMjhKYCihlwia8N0rVxaVMVzpgRjHBCX61gBZHe5OAF6Fe7gCdRzh7GPiXPzuLC8ca8NE2oNN8mWW58RktHXp4skBMkHZlimlNY/tThMe0p1TkxleyKSd1QNIleXjKTfVDSihTWgPMJoEevfkpNFp9qTY8N8sKXzdGvDHtNZmipECXiirNQl/lsmn6Sd6dtuqY4jqfyMPDIEBQaeC9iz/DycPn6enNQosGq5ez6ztBD80M/pSAx/BYVDQiOeU7M5YBcUIDcD1JcXtlFYtbLes7IVBUzUmZ11IZiry/WF3dAI/7FisDVTEPXjDtHeDlVUL87B3Wv3qsCUxc/TKtDMs4V43YZd4+3NjGwnYb/VoDJYJeRVOIqVGV18vcehqLooymi94ImHVJB8Ggg2T7EZKtAX785n78q5+fh58tYxBvMuMzhDJt+hFd/h6f5DuVQaYiC1IcnsZj2ltiHqgcub2cVIocmze2c0QzhgUypSEk4PsuNuZotaXCKmZZGQPemH4YVEj3E2CXlwgtXmwqXrVd1I/y+MTqwNJpJzh36k0c2ncSzXCGrkTVejHbyuYEL+tt2c/BtEygsqlYeOhRN4fUt3QmeiwPS+0O1ro96/mcSmdLd5e0cLGGb+WxygHPxS7X9nbgzrxI2jvAs28dqiodFtlWnFM3cF9VlEzAAcHs8co6FtY2MKg30OM1dUyhY0bzo2xjUjQ2KotS9BN1xy1ha20J89NVhKUuyinw7uvAT985hn2NLr2+DVS9hC+lO55GfGMG35NSY4YxI8U7GbYTTxevMf0QyeWU7eUFzGaTH/QRhFWrmiwxT+/eX4Af1miB6t5vzs9Op4N6vW7TNk1OThroad/zXIEe05h+EGRi7GR/lNRurXY8NyuKq9GQDGs2lsCvYXszxvkz7+AnH/wxppr70dlO4JdrqAY1bG1uG+hZp5h8hp5+KUbS7yDK2ogQI+L17bSP6/ceaDw7X6FezCobLHN0RqxEMoihXndRFdi52Lx42jvAIxU9Mu3TZTlzYwmkLc85C1uVVoQjmuWR2mDSDD2ez1RtaS64mx1elokbN6K7ab8PMkw2Q2yuL2J7o4UTR0r44z+4iLdeO0YXPqXjt8r71TOTb2IOuMyVglT7La+ri6dCyq18l2H85b+54/mZMe0NOQvRSUex50gX3MUdq5KXXI4RsBJVztBUVUP7zjPfTmq/G9OYfliUy28h5E9QlmW72Om1hBxbT+UgqGNyYg6HDh7H4UMn0KhN2Zp6WoChWZ+wMuZCJWSp6nRkajrNgJXR8JMu1sxFalIaUHmrqUA6eKAuojnt6HQLzOlzd2AnXijtGeDpU914O+5bhrl8U8IUjZw6LqqQlGG9KLYBjqorHtB6MfdbI/mV0GQXFlWbqpxKqppUL6MUc/s8vPXW6zh35gSvaS65LdRrGpvCTGNY1p6j6i9uzRrROxnDPFoWQXekreqhHTNi7vqY9oR2jCXj3ZRDnf26dcNc7nW66uKk3PxuVZqSPW2r1Wp+1pEzxsY0pj0iU0eSQalw8mhZyAVbIkq1Ri5ZzZXnaRqyMoKwgrDqY3VjnR5bH3P7D+D0mXM4dOgI7wusk2AYUN6tc54zKU3f0dsrEfAgwCunBLwyIpaPVhRZZzDd5VEv6/4+HRPpdYsmz4h13bZyTuz4xZO+aG/IAIoZkW9FmvkiYyrtTJSbqymRAE/WSUzPywYNC6T4nBLVPe2S1v4M6AY2M0u92cCp02dx/sIbmJrZj3YnwVaLmaOhC9agK5Bzdc9SgrL8SxUfvlb4lcIrQuaLDOho2aj604HemPaO8tQ38dD+aG4MZUEC4o4oK8zb7ZZWShDYucL8TSRQU1WoqoGazebOuQIExzSmvSPJn3Sg5Ljg3eXA15jjCsGl7Az/voZckbXvqijJlQz1yRqOHj+CY8ePozkxaXZ8mlC/qe1P4GRc6FqVJFei+gw7Gsi7o+doAEfAYxkT5D3p4RVOhEKwadCMh3F9UaRU2hPSp9qwg9xD08drYx4eY+W8PNfDhxeNi4RWypvNzuuud5Jjm2yamdunq6ehewP1Fir5tEAG2Oj0bfxIdeIIvNoheoo1lP0GSp7PVPD5dAUpXy7WTOGgpUJVx/frrXqv4pK/a+edOj+mvSNJkfjryQBKfyrF3N9utcy4cb00v5lGwa0APJEL6pvfO6Yx/f5JMlyhQGpLebRtQdSQfTd4PMtipGkPaRaRe3QcOoizDuaPTBEVI6xvL2KzvUpvL6Laoz5WTZccOTUV7Swy7JqOSlTOzkGRnpSDIr3tyoJUdVFrZ7Vt7ixjUjDjZ3F0ILoXtDdvJbnEoXdHgNGfiKcc4PFi4eFp2KMG/qqNTZ0FfDKTi+dGuuOSBUYGdnwuYxiWEV6AdtzHtTsP8Tf/8CH+6u8+wtVbq4j7U2hMHeUzVWZwQCbo0TKRIlRnBvXk01yahYfnoqcsc0BXeHj2EWP6gVCRFyN5wnzTkeUcd5SfrbbkyZ3LM/YbyTq78LlGo2HgJxp7eGPae5Jwix2AOC5kmsz/KBLIxbLdrQqzWvVQrXlWpekHAzxevYcvrn2Iv/vlX+Jvf/FXuHT1C2xsrrGsaCo9rZ4vD8+tol8a+FSzAj0BoPP8VKY0lZjn+zZOVW8eUG9qNfXKiEFZlEHHPG9xHur9F0nDWL1gss9Vglmm5Ynhdg20rJcdzzolM4BHyyMMfGPVSct6MZAraQkgtwyQWLPny7tLy2X0GFhaDrHdK+HLawv493/5K/y7f/9L/OrDm7izsI1WJ0VXS+ITZdULVO2CqioV8Gmh0CKThjTMNu2P6YdGI3mSZ5vlFmVIciXLtdt153Zu+AYSqBXgVqvVds6NaUw/DHLAYZyDiGORdCcNc+pRteOp/S2KO1hdXcK9+7dw5doX+Nt/+N/wi1//NX776S9w+fpnWFxeQJT03NNyRAzwhqBXNtAja8vz0pEaqlOtVW21BJEmjpZel77W1kjRs2gpIgUX8XyxZEmxF2SQkSeEVVta6uSAokP9OXePCa25LpmwBKQqc69KKPSYsLZ6wY760lbHBeeeIb28anMaXmMfNqIQH99Yxf/yn6/hf/yLj/CwE2KlR0DsNxBXJm3aHFTqKJcCWigebOgJFZ4NlLQYu/gWS7qI3HsLfno27r5nNI4uzDE9CylfXBoqVW11BLIBG9NW11y+6ShAOiBz29NIFAMxgWDBMq2c965wHSknGR5lj9LHAswCnSsVyZUDvnH+vdpUlPhv5qLkS1pG2f0Wcvd1/ORTBVM6KY8VelvquOeqEHV/xgsJZTpBpUq59mNombS1zhJuPbqOD7/8J/zlf/4L/Lv/+D8ZyC0tP0A2iNGYqGFiehKNyQmU6bFFWWKOg1PBrnyopBXxsp4P9B4DXqvKGSHwaUUSHhpbWx5vVxl07HSenrUyxz33FS+WKv/dOyf/+3z/hZOqmMwjY8ooUUtkAYzPdPGZ2hPhBLrbMZ0uWhATM+gwhRZWV7C8tQ4vCBDHTEgBE8MpSwGp+qmvCW4yjY9kImsGcL1Il3i2UkNSnsBa5OHeahvX7z8g0NUxse84ysEEEk0Elw6YiXw/n/X5YNnWeKLVwnNaNqhEANWYE3Vv9+lpWq9QXjPhthzkOTvh2C7ZnkQ+FxQ9w3Ou7ru4Y0zfh5RqBmpMV6WjwC7T0BL+K/+1zmLAc1r0wq/vQweTuL2a4a9+fRVp0ECX8jBQTQEL9Y7BZTLo8saMLFq1SCtohNP4w5/8KQu4T3nStHW0Xml4FVWcY3oVSWVfjSsFS2rEIsmhK/Hat63Kuh3y2PYlb/LAhBCF/BWs+6h7bEyw9IlUS99qtaxmy4w5ym63j7Ds25qNGnfc78fI+l2iTYJSSD1Y62Nx8wE+v/Exfv3Fr/DJtY9w+/EtrHZXba1Rr5KCWAXNSKQx6ur9LtbsVa5sqHsLgW8Q8Vh6WDOvUB/2eZy20fQyzLCIvHv8OI43JjCRZmgwLWp0TKJOB2VPBijLpwau63vMUFRqqMzyi5z9+EJJr9wTYvq6xLAPZiIoIQQExi4d+poCqrAauD/BjD06N4v5Rg3lXpfensCOrjbVkOzwCpWQR0vD17yHzEkpQ4WtJM54X6Lpyco1dCpNtMtTuLNZwb/7h8v4H/7XX+HS/RZKjcOoNObRTeUJ8Hla9b5fQxjwfVSeGnQcRRHjSi+zWrXQXYa599g+2c7nB3q3Y/tKY/277x7Ts5JLScqHLEb9Mk8c/PEaC1aFjIzWLpVSkhL46OG1e30kvIEqg3+qBnd55eRQQkZ2Z8gioaGb1MBNuLQHJXRMP2AqZKWQF24lHt9RRL7tVuuVrrlfEwKZBsdR/LzAQ436rznZMK8sJcitb65go7VKcOoBQYpWvI5Hq/fwP/37/xF/8bf/M3796S9w88EVrGw/pqHXQt+jDgsz2nIp0jLLAYHNjD6S4iNvUSynQZPol9XLU+Pu6DWmJT5TYbmjkzLotTBX9TFTDW0lGg1v1zRGWZrutOmpRBrQqc+Fla/cW7T3uXe+SNozD69IWG1N8eRs5+2aq8o0ZUYg01p3WeAjpdWw1tpihq7TXW9SgXm0QKi+GJa6khTshhkoQIXoEl7eo/peShnqrl7SQ6sXY2V9i679EiLNqDE1abNqVLTGE8HNhkAoLrRaBKaqm5bqMwiTUs3J3mVn3bbYL9iOZTk5ETB2XzqmZ6EilV26Mj3zfNZvkboqTtmgQs+vhqhUw83H2/jVZ3coQzX01PCuzkp6YqSwy1K3v7z9Qo30s5NzePvNd1leaWDRwxukGm/kMXz33JheTVJpdlKW8y6DaVj6R/cLD8d0nzxEsoyqwkvUtpBE9Sco1mE0AKEnppmDYgJgJ+pg4NEjKxHkCF7BhE8Q6+Phyn386uNf4m/+4a/weOUB1taX0e5uWecV6S7NkWlTJzKuqZciI3hJ74pUblS7FpB94pPPOFo1pf5YvvrSwfIGFR9GsxZ38NbRIzh/8CCmKx7qGsKTxrbaghdUCI4ESX6o9ZjXl+U63cJk2Pbe/N0vivYU8DQGT1t9eME6oYS3zix0r9UDSNZ7RFc+8wk1tHBa3RYW1zZoadRp1WiKsVz1qHWWwCQ4s6mj1AmF+4WwmQ9AgVGVl7nUBLXa5DSfL+Heg03cufsInW4b9eYE6nTRPd+N1dMs+9ZzieGr+rTEjFU1p8VZ71Wk9QYpXVO8eps7XZzbATtTzk6odeeYno1cKru93YDn8llbl/P07NV+503g81uL+PT6I6Q0lGyF/Fw+HLn7dWTqhwVTvdEEeIfmj+C1c29w32f+uzUaK2PAe8WJ8iKtbTUD2jodU2gEUSGjT2UqD+vmb36RAz6ecVdzWa54mkNYPpH0T66zeMaqNwkmXbQRTgnoBrh+7wr+4m/+Pf76H/4SDxbvIWxSD9IbKxO1BD7WM9P3XAeWPr20QULgEhi5+Oq067MwZNWQ2Pm8qcDerKjxuM64nKj7+ODUcRyfnkE1TdDgJfWt6A8IrnxfYoDnHBoFoO+zpd94Ql9r+tOuvTjaM8AT2itRlfHOpS4+Ximt5GHiprRcgsASuMcE7dOsCEICIDNfzW3rXVop6l0py4I5WfYCa3BVb0sbS6dcsn8JowM72xLwNIYkoaXfSuQZeghqIYG0i7sPtrC8ukTvr49984eoEwP4YdXCzxK69PQCJSMBhSdjeDsZVmxF2s+FNj9gDFx8DPS0z+uKy+hjY/ru5FLV7T0V8Hho7bqVEL2MMhJM4zdf3sWV+6sEvAlb4kQWtHtmhJmhxgxFY440oe6Jo6dw6thZgp3Ps5Qv9QsoSRk5WR3TK0ojgGfgR3lwclRsR1i32p87NrCjbDnQc7rBXXU0MJ1I/UZvSdWWmQ0WZ7g5GvXpnXWwjY+vfIj/8J/+V3p1v8Dq9hK8OssCr213Nw3wqCmpp/gsy4R6bdqE0GoL1DGxVmEKfPSXa8wd1kxUmqGl6KDVpw4WCAaU/QmG+bOTh/DGgTnM0HHwoh5qDNNjnPv9hPEEAc95d9K1CtE62bBsCfAsfJ7f0Z8viPbOw9OHmxAo0fXxznrQvhMY/hLwPAKLbrKqRQKfrJQqvbx6cxJLmy2pNvQTrXSgOm4+RWCS6DlLSKGTJCh2lLMyhTkSlaoUB7dooSo7K35oOnBju4NbdxbR6nQIfBkmp2YwOTFtY1M0A7lVbzKTUwmPvcIibbvuHaJi68hl8fAO7SmmOhrT9yeXknl6WsFxRy7dHasVtyyjJuK12iz+/qOruLvUQuZPmYc36mnbs7k8mq0tD4/KiHYxzp66gMMHjlp1pkfQ09qYtqCsHhjTq0vUK/mOcSGBtpfL0vCKzpkZNXJc1EAVN+ohGdECUVUj9KGlPf1qRXNjIEq7NMYf4cada/jy+uf493/z73Dn0U10EupBtcchQpR1kcmzC2jQG1AS3AR0Ms7J/DEA0zp3rswoTi5W+hrp4GIctNq5K4yi6szKqhLVItp8Zl+1imO1AH907hgOVX3UqAcrdATU0U+rpusbXHUmw1IRy8O36ky+QICnD3YdxnT9xdGeAp6ntOG+JbJ9/FBKtHGDF5n4yigjZhYTNayUCXhNZrLcdCqeuIc06ZlFogxV9aPmyJT/LtW3E2DOOqdOLGkpRFif4IkyttsdnqflMjXLQx8ra208eLiB1fU19CJVJ1D5BVV4QZ37+XpPFARlrjJTVCjaop7e9cY0f47n7BaSEy3HYw/vWUmpWKS6S9xiq+Ll0lVjiQR4Wx0e1/fhr3/1ORbWYnp4k4j6eVWSHtCj2rpdMs+riikHvNfOvo4Dc4fglwIWWp+evsaF6vkxjclJjNj+KDfF/s55yRL/pPDdFffnyIGcAE4dQ8Q2SbMNLUjRS9vYaq9hafUh7i7cxJUbX+Lzy5/g0vXPMPATGnObNMy3qI8i6idKayhDboCUVllFU4t5HssAvUjVZap8kAV0tgQQEaki0CVLGwmgUh6mVMxZRbVgArwMPoEzTGPM8rkT1LtvHziAdw7N4/xsA0HcRkBdWOOzgyRmKARHX2DLb6Cedl4cv5bfXiG7mj339XrfKwV4Vk9Msi6wsij48QZ6JKWDT0ATiKnKkmnGLa0GVSlyGwQ+5g8e4D0lpN0WmYDF8w5IJHTK2yI1uVXC6yoz2iq9VCWl1XtjWkMUhkaDAMpXr21sgU4dpufmmHV9rKx3cOf+CgFwhYJTRX1yltZTgxkqT0913AI+vU/vGgUyVZ1qW5wTu7vEBRSP6dlIYuLgSinL1FRe5/ltacvrrhm3hs0ej2tzBLwv8HiLlrA8PLXPURaKEERu3xVOk6FMnVMCnD/zOvZPzxP8QgNATbukpavMw3OvHNMrR05aJG0mieatOUCz86bJTZr45zT7zjVjSk9ZQCdwG2VNzByTE1uG59HiXXx59VN88vlvcPnGF1hcXaDu6cKvUcPwHl8TQYc+PJ/aho6BqkClkQRwzijX6yjhKg/cdWdcnCr5dGHSRCo/as9TnAS61KrUzwS6foIGvbvJLMUBvuPC3CzeP34M7x47hGq/g7S1gSo/p0bHI4nodMjLI9Amqv0i4ClsvUtsnVbk4bF8iex9SooXSHsKePbZ/GBlgeqste+Oil0mF3fUFmNqjVufGanusqpP1vxwsxM1HN0/iwYTudvawtbGOsMe2FRQaUoBsuD4PBWUBqFrHJ2N2WLuy0ZX14MSM0fAqhj59OI01VicUHhklQShtdUtE/hu3n2IxbUthM19OHz0ONKopTxlJlNYUoIfw1WVq8AuZuYTx61ziwZe2swDfNeA8dZ3V4OAcaBQjOmZSPIhUDPJUIFm2u8CPG3LlImoj8bMQcSlOv6///ZDenoT2EhYyCsaajCkYr+odpIikEU+SIA/+tmfYI6Al0X06XlcC+uW3/bQaCBjeoVIkqf+A9QRJc/Wf5O8pSmFkjIUUG9I9xiYaMcUGe+lES+vSz0uveoA3WQbnWiL8pggqAvEIuoaenMPb+I//OW/w7WbX2Jh6Q422yt2b6ahB7zXJoTWUAVZdVJm2lq0pF2cyV2uuGp7AZ863llHPh6bLtQK6JnrNKN1IrnDeFJXEeh8bqsU/AodiYkswtFaaAD3x29cwAcnj+IgQbbUXkeZ3meV32GmH+OiTn0ae0cXxcCuaLtztSW6T+XKAZ/F8VUCPEuIfM/V8458PLdOme0c2t07iozX5D1p8dYyucrnZyebmN83h2athu3tLTxaeIR6tWrdx131UxkJXTfNk1kq+6iGBL8kgkeLxIWvH4NVxxIURkhVmRkzSW15nThDOx5gq5thaXkFJ44dRGBVBpr7RYLlhM6A2aNQqU2Rwm7zgFIANCUa5K3ynN5kt+dKekzfj3byTCnJvHIeno5dDhrxfCLL25/Adhbgf/mbz5EENbS43y87wDM50w+PCpkzyLNCWqFxFeC9t3+Eifo0wU7WqRv7qbW/dh4Y0ytIZdRrEyzPJeoVTdBMQKERlNLIVpOK/mTQaqv2MhtAzm1KfROnMaKkja3eCoIGMLOvSbArY3VzEV9c+QT/9OEv8dFnv0ZMT05enjw6eXNqAyp7DI+soQVSIAIP/UkQnXZ05UFbDbNSvNQ0JAAOaWRr+Z6MRnfci6m7AtNV0qVlVYEyXkEaoUZHopH0cGKigbcOH8DPzp7G28cO43A9QBB3Ue61CHZdBFTEriOgyo8jFYthbYtjK0/6zeOqP90nwOPuC6W98/DyX0sgJViu+PVr6aCdkQTSoW15TdkppebRysmYAZpOo063fv/sDKYnJ6yNL6CAdQh8Nr2UnmGAHoFOg8bVvpdGEUJaMx7tkWGG8S28z8GRmBlFb08s8NMih9vdFEsbbTx89JhemurGPUzOzsMLGxTiDN1eRGAr29yLGizqGooVrIv7EOx0kCvqMX1vcvLg9nYDnp0x+ZFNmapzitfEanuAv/iHy0j8Gtp9GihlGjz5vcPnHFvVi/4IeDVqpPfe/gCNcIJWrM5pLB4NKD620+Y8plePKCOq7hbICVRUvrUUj8bLaTiBVh0QyGnye61YoDY17auXZdmjfIUZJvcHaEfruHb7Cn75m/+Mf/jHv8Wla19go70GBk3Pb5sGm2Y5UR06ZZqsKkfrYSnD2SZxVlVqIclOMxbaUhNw2CxU1DVl6jzdqQ4oHnUSowYtamyrKRDASr02mlmCI9UAr81M4+LcHH5+6hTePjCPc3PTmPX5fWkHA3K5kiGUl0dAN18lJ6tp0RkVPouHY4uRFL22xX0sO3tRfvbOw+OH8nPdB+cfrY2zArifJ5Ajt1+ccenkPCd1Yhmos4ra+ggwmtNt/8wMTh47iqTbNTe61+kQGOmiE4gCn1aNQqGV41N4ZKFYoBZi/ob82M25yAzkcwI2eYbWu5Ov6/YSepEPsbndJWwGqAR1gl7Nph8TpTnYuUVmnYBKECw8kSnpMeA9KynVXEqq8HwV8NyBhqfQWClV8Wi9h7/5p5sEvCo6Axoj1ChDA9Mk0fYld/ZnVZoVTDSm8O6b71GuaihlPAc3ca7y0XVaskfH9IqRZCSJWcYJJJ5HqTCQ49m8DYz+nm0HZbVlZdQL1CVkTdXVTdtox5u4cutTfH71E3z06W9x9dYVrG+tYFChTtLchgQ39bvMUYphUdrE1EnqHW4TN6takhIp6XUgIlbsGD/e61E/+jwXcOsJnJLY2CfI+dRNA3pxfj/BJN91qFHFeQLb24cO4INjR/DO4YN4fX4/pgnSaG0ha28T4Kgz/TK/hSCmwqce69xIb9q78y0/mnsGc8O/PGJFHMWuZuXF0p4BnmUSNc5OBpGdZS2LhMd2wVLFrrstEy7fihObwiagqx4SvCoEPGVigoD3NMMAh5lhTa1szkyOOm3XrkbgESjW6BGmmhWAie4yilm3610ULmvX43Xz0nLwIwv45PVtbffwaGkbdx+tYL0dwavW0ZycssHyqs5QfbbCUpwlqHL+3fOMK7dFHfeYvj+poLnUc3lnBS1PS+WZkx+lf4iYvvztx1v4xaf3EXshuqU680LzYqrAKveLsApmHllbQyWfZeU93ut6aHp8zmaS5/vGgPfqkskJ9ZTPMq6VxLXaZpx2kWQRcSC2Y/gEvUqKkmY0KUXY7m5gYekuPbrLuHzzM/zyo3/A7fs3sNHaRCVUTdEkQgJPlEbYJMhU6zXqyKF8U4XkLLmmAU7j23q4S7/YfbxGFpDIq4vbLc0LhDqv2YT7BDgv6iJIItQGBDpvgKNTdbx17CB+dOoY3j92GBf2zeAwdaOArkp9Weq2UaL+DPidtVoVZd+zMdHtThchda/AzYGYtiqVpsGt/GnPDEix4sZjxW84HMGVvRdJewd4/FKtWyfA0Ucb2BlLkCypeDZPDqXWzpZsCSwho8qylHN3u04oCouWFcGsyQwSq5pT7XlqP4t7PSQ9CmZMsKNVltFUscyycPkwyYU4cO1wdlpSRjDNQa+sRl6BntegtVbCOoFveW0TS6tr6HQ7qNer2Ld/FnFMEPTUhuiqHSzDpWKL9xmP6VlomHqSBaXp6Bmx+x1UAvT6Aa7fX8Wvv1w0D68nwKN1rLYLVTHnj3HfPaeiao3rlK/9s/O4+Po7FFYqNoEknxvk4/BUtVQ8O6ZXi3LpYtmmWiG4aeouDSOQJxdU1dlNKqKPtkBu8R5u3r2Ga7cu4erNL3Hj9hXcfnDDxs5pejB5cKru7BGIIuot9XbzqyFfIB3Ha7xFLN2hjjEVGtvqWKcWQulQgZ5zHniTaRbd2be+DRobV457APWSx+1MUMHhmSmcIrC9e+II3ji4D68f2o/TBNuDVR8TBEI/6lgVZynt0TsEQjoPGg+tGa3UD0JFxuf7Vd+h0qJOgEN9VpQg6XIHfDprqaW4MrzMCpri6J54kbSHgKdR+MwkskscdQhQqwsTSOlR3KeEIg9H5fNH+0ouSpVAT8vRq1pTkwbLqQpVxUCh6dLC0STSkxMTmFP73tQULRXeH9HaooWShVUkFKCvKkx7o3U04b9VnfLfzrsYKf6qKmug5E9SsKvo9CIsLm9gZWUJcdImPqrXkhQjhZcen3l5ijPfpW9RO6LaFcf0bLQrt56af3lOVao0Siq4em8FH19bNcCLy00qCwGe7hgKmwshL7A54B3YdxBvXrio/lHw6S0K8PqpqtPHgPcqkxtrKzhS0wUtoFIGL6D+0gBxlv/N9hoePLqNW3ev49K1T3Hl+he4++Am1jY1i1OLOkCDwwkZfoXPqcaIeokGeaomECEKQy/eIC/KUJEeXcHy/GzqLoEl42I1VYpTyTXTSDob9NQC6qGQPOWXcXRmGhfoxb1+/CheP3wAb8/P4WQ9xCx1cC3pEuS2AcadMSGo0uBn/Coa58x3qZenhnCpHTsoV1ELXE9l6WQrRXn5c2UnLz9EYgeJrpA4b1R6327lPWS78uJoDwFPiaSkcgnEXdtaSvDfoEXbERbQ7ZCAg8ml+S6rYWjemC2fQXdbnpwSUstm6LifJMz8APPM8P0TTbrzGQPMsKnJoRlmseadMsDiYbFiJiep1ZVbpigO8hz1rCk6nmXGqyOL9YQKAhOOLt/1aHkbV248xtETJ+gFVuGHNd5Lz0AfZUG5HesSbKEzcPs2+2qy2XU8KzZx0h1kpZN+XQz1736exk+jPAK76Ovu/WGT+0oWcv5mUgC5PJWpBFRgJR0mI14N21kVl+6t4dObq8j8Bu8P+Zy8OJe2ynRXPen2TQaYzuqReXCOCuLsRWQ0kj3mt+uoQPmighoD3l5SUSZcHg75q2RS724e8nAzQk+eGX1gN1vZJGipfa6vTiUedQM9tk6yjfsEOlVb/uqjX+Lm/WtYWLqP7WiLstin5+bBI1dCDQBIENMzjFNVg7omEFUZWpMH3yKd4MaK8rz2dU6uHvVNRhDrE5TU01HfLWn0qf8CcrWfoEa9V2ltYSJLcKxZw8WjB/HBmZO4SMA7NtXEnFdCI2rxvsja8cRlseIhJyT/VDXN6JUa16dendbjnWIfR7FrxxPxPtNN2jX9ZKd2zjkw1tbt66TF2e2+UCpd/W//1NL2RZNeqgTQ3k4iFQmgC/y3e3Sc7xe/useJtlikMLS1VrI8XG2dN2Xj7nhFMFhsN4Ma/t+/+hB32pFNJaY5Fyu+m4w6od+tZ5xXydyVUFGR2npQGihKi07VrgMqUtr97mUWN4VMQQOFaNBBddDGz989jX/98zdx/sgk/HgV/c4y6pUY0/UG2lvyAn1UPCnchMIVcStBonepdgFae+4b895Yxqq05ZakuJmi/jpyiULS9sn9Iv3yxHrJyIZ78DOUT7F1JpIMJfAGMYIB0YlC4Dfm8GCzjGjiPP4f/5+/wKd3OlhpVxDWpm3guaDRVQdpVgtZqyzsDFOLavopC3dSw5/85F/iD3/05zRN6ui1+zSu6B3yIVnisoKH6TqmF0lSmEWX+CIHdsp9LtPKW5FdVz7bkSNJSxYlrqu+p2Em9Mxije3VRBRuhQKrtla/ALNU88C47dOg1rpzfkigoApQOVzbXMUtenDXbl/FwuIDbGicGj0+rfNpC6jm7y/iSCihPqFxLv1CrSFQsa1FmiBH/aJ3Sx49yrZHOddWxjwvIKIO6voytD3UiDy+5LHbQxDHaPL6BPXC2YMHcHLfLM4cmsf+Zt2GHfTjHmWZckxwVS9LaUMXJYvAyFZfqv2dCJO4b5d1zoHY6P3FvcVpdzQk3a+v1VbXXI/1F0t75uHZBxs7sDPijglGzvkNRk8c5vtKsDzR7AR/xPnVwiqSB6deSqrPDmjFaPYAihMm9++nxU4Q7LSRdbvMDQGbPAYJKrc0c5RBGnhu1rw8PAkJjynx0PgWvUkxUCVCZtWc9ADoQWTcavqyxeV13L17H91uB/vmZnDowDxBroz1tQ0Ets6eC8G6MXPfqj7TjBYUC1PF11eQh9+k/520kVR9hXYu7myG9OT9I/e+ZGQxl+XI9FE6u0IkCGO60WiQXGjoQXtAL7x6AH/34U082qSiSAl4amyXIrNGf94psCs7wNNz5t0R9Lx+gDPHz+PYwVPMc5+ePHMjB1fXg/dlTb2Xn5TuViUtkX4iH1SG3I7baFtI/rAElBD6LKdJH71ulAOdJo6QAeoMSm01e4kmX+7LyOVpVfOFNR/Vhoeov4Wl9QV8ee1zfPTFb3Hpxhd4vLqAHsGwHIKSqFlTJFsCB755JJKSM3llDrQVQV3UnjSWq/WpVkPTCdI5Gj6gZXc0/s6qG/0SaoyLx3NobwPbm/TmUpyYbOKdY0fwwemTtpLB6dkpzFd9NAYJgrRnus/Xagb6HqUfw7OtvXE3F7+7yC66bynue5KNvuaCNu7rdk69UNozwHsRVAj+6MrUO4WByT07OYO5ehOz9RpCCnu318aWpinTcIU6CwMFK6Wwqt5Z9wt6KvT+ypqhgJZYMdfnTs7t5KAEKLP2xBYFcXk5IsAtoxe1meI+/Nok6hOTfD7mM5EVJhuXRw/OTVBc47N1ypWGOMiDdFUcsiRVfaKphzQ2x15XaN0dzuNjcRYV21Ha9YBOvHRUxNwBnmvDy80OWrCpXaW/h6jUQM+bw9/805dY2qAFT+DSrBjEO+brEPDM0iabylE+9NVFJcRrZ9/Eof3HeDPvZd6rZxw1o8mUUxZj2gsyMd+x/JSHxX6uTotjkwu3X/zZOXpQxAcS85oGkE+Z8OjtaWiBpudKMg0O7zC4lF5cGWFd7W0Ex6SFta1FLK0t4NMrH+LKrUu4cuMy7j26g83WGmXR1RaoE4oiaT0m+RarOTAmYFHU1FfFpzx5lKtyMb5TU30xPhRKi6dAWBKuYQCer/dL1rVuAsGP4OV1tjDJeB4IA5ymMf36wXm8deIo3jx+BGcPHcBsLUAomVZ9vCbp4L5qJCXCBsBFerxC9M8a8L6OBHrmN8UJJnwP+yYnMD3ZQKiJVwl2CQFIM41r+h8DS/7rfp/WvWbL11gsSgxKZrlRek1oXEGy8B08mbCrK686iLa7Ce4vbGNh8ZF1k6/WG6iF6hjD+1jg+iyAaUyATUo8F6JebVpnnB2B1HvMWqSwk60w8Rl33b1vyAWN7ovysIyK/dFzLw8p1u7L5ZGrDU/HOeAx/3TU00rn/hRapSn81S8+xVqL+aV2uAq9cAM8VVszDKWtsUKTcnKAF9LoeOv19zA3NU8r21Ulq1ewpmxStdYY8PaOJPfOG5I35rofaevKoGNn3BYlyD3hjsgCvESeEoGOXh3RwNrStLiqvH11QNFacuVANQUxOvEWFtfu4+bdy/ji6if06j7B5Vtf4NHqQ2z3toheA4RNGquhZCpDkkQ2wFy4UgCexZbyYzGW/lCXRTVT8IyN7czLs9NPul9e4IBmm2odaOBaGxvNOMpqnccnCWjnZybx1vGjeO/UCbx59DCO83iawKgZU+KtdfRpZA8YF3Vm8XbATlWyan4p9MerQ68k4IkkgDXJfa9D46eLiXqAwwf3YX7fNGU/Q6+zjSym96VOL6mquiiEBDuQWUwoNLlnQCVrhU3HFrITIP2qM4uUo3Uh5gl1Od5o97Gw2sKNO/cw2VDvTcajOUtQnKQyVY9TWoeqbpNCVYh6h1gDWCmoElabd5Qka9BZuTp+kp8kFy+3Hd1/Ocl9RVGlKcBTDklBFIBXRkyQSr0prCd1/CUBrxUz3zSAnPcrdTPeo+TbDXjM5xzwapUG3nvrJ5isz1IGXP5nGmxLzTEGvL0lp6gd2A05lwoVKiPd5e7c2TM5cdfCoGY1K724h4g6QFWXFV9DAirwNCOyn2Gzs4I7D64T5D7CZ5c/xLVbX+L+o1tY3HgEzZqcackXtcHTbbNFVeV9SS40CF1v0Su5tdixWBrwWZmV3FKOVB3PfbHdbFecHE8Q0FRlmUkXtbfhE8Tm6iFOzM/hjYP78adnjuOt/bM4TZA7yHunGXiV93hxh8Z8FzTrEDKJQn6Tamkl5+a9qsOLPE2N5ctT41Whf9aAV1RlDqsxHTkLSrBFpcXMl+qjUYQ6PbyJaoApemX7JprobmzIcNO0KQZ6Um+DskdBVQugAnLVFnT17Bp3drF68mnuTk05ptalkle39r2NTh+Pl1pYX17GtnWYaSKsTSEIa9ZYLmtsoCqIijw5NWwzjsW7LGxZZrlFuOuVikvBPN6h4mDXzTrx0pJir1TX3ijgKWcq1pNW56vooY7Fjo+//scvbALpvtpXbT5VNd0rP3Mv3Vj/zLc+AW/goe5P4kfv/Aw1v0njh9lEzzChHKjqSz2ClfYvdyq+zCQIcd6Ry4URtkyxn5yG+5IRGSpG8uoyGrUChrqPiem6jZ/T1F6Plx/gyo3Pcf3OFVuS5+bdK3i8+gBtenpaYNWr0WiS05/3IRPYpfTA1NtSYKcJotX+V7xZcuX+3L51tiKnjIO6/avMKm4COutrQI431xASwKYZ3pGJOs4R6OTFvX38mC3Pc6rmYZ5g2+AXaPmeQExAC/heVWVqikUNz9L3Sl4TXtfwAukl36OxnVedvkr0ynh4BegVWxlZPVlBaoDWIM80Rdpqw48TzAVVHJuewSSV4rRWO6eCzBKrOVczNBKWNbXtOY3n7DGz0IwVvmN5d/ImtH5enwo0oSJVZxb4DYJbA48fbWN5NcLyRoROpC7LAb29AH7AglLpMYQuyyS3BD1V1xnQWdWHZv2QdZa/im93LBrZL6JiVByM8stLzsSQhydgU6cVFV0HeIWHB6+BrcTHo+0y/u4319D3J5Aw7eRFa4YeZqvJgQM8HnBfHrPz8Hw0eP9P3v+5jb/LEo2nDBFTDvyQz2rcp+530RnTCyelvMCuKHM5PS1DBCaWvznbuT7BTmNNKAv01EqVDNudNdx7eBOXrn2OS9c/x+eXP8KDx3extr2MqN9FKWBJp8ukIQUlWsgtdXazsMgEl5Ia7QVgFAzXEiKJdDyMp6o8KbMEsR6jnxCwVAVq4+eoYTScXB1LqgTiSR5bJ5Sjh/Cjk8fxHsHutdkZHKdBPs/7K9srCJIuHVECNr8rZPAaKO7JeyP3qdNUU2TTG6oOn+9Xz1MNXFfvcColi9GrRK8u4JHVGcWWxpBQaEoynqwSwEJVV0Uxzp84iRottVRTk0U9m/JHvTVtHSuSTTUmADJhLgS6EKG80ZmAJwHTeMEyzUe3eCytu7SMRnMf1rdiPFjcwGanQ0VKK3NKYKgw1NPQdVt2RUaK2DVuSyHLu+MpR8X2a6mIl1jxfPnJpQnzgt8zBDx+Ha1bTQhu0y/RM9uMPDzcLuEXH98EQjccQXMgahkoDfDdBXgW5rADgQO8P+C+QHJAq5iAp2c1vlOWvMVjTHtCVo5z764AsjwPjbk/9NyfZF1j2QwSemoDemc9LDy+g48//w1++/GvcI1e3crGIuUqJuc9LSt8hkZuxudjelE96otqrUZ7l56ewIU6Q28WuRjk0mHxFIsotfmuxs9lDFfDnCo00LQkTziIUevHqBPsGtQ1f/rORVw8egAXDx/A2dlpW49uIolQ7bZRbm9ignrCI1hWKPNEN7WhOF1GvWPTIpadTpKe8tRZTjpINRu8LaUca6hDEbNXhf5ZA57AreAnyYRTma8dCoCmyZHPZNYRT6pmPSIITdHCOnLkMGZnp6hEE3Ram+j3NBsBn6nUqTSpcFXlKWFjWJqBw1YTprBJBevNmmDWBnCqULgX81Yf3ZRvrE6y4FWxtrWFW3cXsLTymEBYx+EjRxhvF56cu4xeidrs1KVezmUcq3qCwu4TAAmqthQJA1fXakUkYYHU7O1GO4XOFQBX6simIF5OKmxn/TrAk/erL5SHxwTjXgx6YsE0PrmxhE+vP0JaaVJRqbt3wxrtzZSw5OCDeVo4wHMe3qmj53D88Ck0winr0ac5NKU01EtTnRtcHo9pb0jVhgGVupNjyb6MEPV45h5ZA7l5myZzlgFEoGIxgVbjtlUHvIQeW4Q7C1fwj7/5e/zTh7/A7XvX0OptEtxSemsCN/ecVXub58aiqLLEgAQcalZQ2asoYMqNyjnFys4JCItpCDXbkqRLg8VVTnna2tRCGrR1srW7dVto0OM8Md3ET86fxp+9exGvHZjF4Xo+r2XcJndRIxjW6AGG/K5M9U2Km3ScVV0y4J34DQ1x61lMPaW1+/QR0iMWZ6abi9mrQ69spxXmNpmCYV6TvCd3hhLhfikbWtIjtQbePhr1EPOzM9a256f0+Lbb6PT6FCwftcBHnVa/pi0TgtrK7NSQVhYtNG0pXPmxdKvGgEXqAEMFWpIXyefUeL7d7mFpeQ23bz/A9PQ8BTlEGDatCkJtTwJNFSCt+K4qOQk338jzeq9OONDTeKKiDTN/K9kVALc/3LyMpPR0LakjgGfnaKxQ8SlvaTOjO6jj0r0NfHZzEak3ia7WtKO1q/TS9wvsXBdtC4r/hYfn4/C+ozh97DyqHgGSzraUnFnuTF/l7xjw9pZkdGhVEoGc7EKt/q2VvwVymsA5UZVlxfW4tN6TBIjN1joeLz/EwuJt/O0v/wOu3PgMDx7dxVZ7nWWeIKgO2HxeU39JLty0XSpVanejhAlUKCOSkzSiUAjheKQhSjZgneyuAkkccUsDjOVVkqPaB/W2pPRYtWWtt43JpIf5wMPrh+YJdGfw47Mnra1uP+Nb47Vanx4dOaSx7ZPlDUqyBcQJv1ljhSWEBl2Knw7s7e5YMVGpUHmwWFGxWe2Q3cG4F7L/itArC3gCOj+r2FgYU1sUDs3xJpYgib1GDRELVETB0woLc40GDjbq2OfR6qrWsd1NTMhLvA51/aXHJfHSwHQDpgLhnDi6axQyq4qjwHYpvJoeyHp0MZysX0G3O8D6eowHC10sLm6yUPuYnNqH5sS0yXHM9yT9nhXEIGgw2j7fQ5tNvUGpgOVpCgC1/1XAG+Xh5mUkl576VlelKbNZn+MAjwWZXhrtYPTQwEc3FnH57ioSjx4eM1jTIznA492m0LQv1j/loS/AC3Di0GmcPHoWQaVGD4+efwF4/Jc3YRMFuOiMaU+IZSD0EFY1GYDWomyhG7eo01P4NY2d0xyVGctwF2uby7j38A6u3ryEzy9/ikvXP6V3d9VWElfHFTVpeQHLsiegkEzIMxSY8S0FkORgZ7JGFFSVoAxlq7YhmwhJrqgzSuQaDWBNYqcZTko0ZsvUE1XGc4LvmScwv0nj+b0DBLrTJ/He8aM4MzNJ8KtggnqhSVDzki5lWTM3qWOdAzmBscYJp9RPKXVSMQbVZpUy6XdxtK3FVxJaQDDllXpPRzqr8CzSrxC9woBHgdXg8Ty/JeQO8ChaZO0TRXjeiUwpodBFMcI0w75qFYf3H0DQaNqyGfH2JnrbGxhoGY0KFSoFXUv5q25fzxsxfBUbvolbAR6Lk6pWuHXLD+m5Oi3UJq3VJs+HuPtgDStr2+b1CUprBNxqs0FdXkYUycOoIo1pzWa0IunRSZFb1Y5ZnXyPANfF/gnOaWT3ZSOloQYXaE8z3KiAF/as2jT4yzysokvA+9Wl+7iz2KYR06TyUzpr3KNLH6fcpKnE2kiJOQ/v3MnXcOzQKVrnVFuSFWpFA0rKh1UfjwFvD2kAP5CST5EQSJK0x3zUsALmvOZrsE4o61hae4SbNoHz57h8/XPcuncDj1cfYoNA15gObVC5F9JoZJm1Mml/lA0aobvL766NwYdqdqxMq7zRAFJdi9ae03J21omkT+Al2Plx13pbTtEuOzLdwNmD+3Hx4Dx+evgw3j5wAGf2z2I/QdvvdYD2Fiq839NzLPVqj9ZXqu1RcdG4UdVMapUXAdwQ1MhPlvfRY234o1NuV/Keue0rRK8s4Cmf1V5n4EMzTR6TCZOxs5Ba7Q48WvWNsE4Pj4UioYBo/j1KXuh5mJmbwlTNR5OCHNAq0xCHvqpYFD4FMrUCwzeY4LFMaGOC5rY1Crl5KTb2TndofB+VMcGORQX1yVmsbLRYYJewuLpCQfdQm5wh6E3D9wl8MUE6VoHnO6mIpX7VbqfZIqzdgO93NFIodni4eRlJisZ5eKOAJ0NGUKfvJhxW6lal+fcf38TDDaZTDniBX7W00Z/ywQGeAmUKjgDeG+fewqH9R83j0yzxAkp169a91mZi8RjTnpB5J/Lqtm0GI58oMzFVhx+WsLG9gvuP7uDDT/4JV29dtmV57i7cxurWMmWFRmuDZXaqxqLcNQlS00CSZba+poxHGTQqv2rKcO+SlEmm3NbtU2+oo4h5+gQ6Gr6+tjwfMlSBXZ9G8CTjeLAe4PS+Kbx+cB8uHj1kHVHemJ/DMfXKjiOkNJgH3TbqLKaT9Fg1BLDM5yXHBnYi6REdlSjX1oxBF48xMTPPxHe3PO4MvbCTgnDdlMfb0q7Y2l2vDL3CbXgUXMt0go01/JrUOIXHnQq3fsmjheXZmG+VCskHywLvkReV8DjCDIX5oGY3mCAAsYB0el20ez306AnS7bIqB1f3L3bvsPfw/T53Nc2Qe6P5JtYJJiZ4JUTHEr22vqo6KaRbnR4eLq5gcWWbYdYw0ZzBbK2GBq1MT+9hQVW7kgw+AR4j44TcKH/pLh5uXkZS+kkZSP04wFMbBVOR3+3OUylUamgPavjrX1/GcqeEpNwwwFP7pnlqSh/micsXJsYTgPfOG+9jfvagxqCYWS3Ak1LkC8ygUBq/xEn4chMLYx8RanUfk5MNFqQBllcf48q1L/DZFx/jyo1LuPfwNta2VtDqbTH7UpTp+WlIwcDLKDMZer2IWegkSR071KxQyIHVuth7HMAJbgQW2tpE8tym9NpU3jyWtwrjU0oTDCJ6afTUyr02jk41cWr/NN46cQTvnjyKNw4fwInJOmYZV/XC1ETPFZZZlXABnI0JNqDrw1M0ciNWAie5lBHnJo9Xh5my1U6pSlVXdNeQJdvacfJtbLKeg5y+Zef8q0WvLOBJyaWVhEw4kRBQUjTtj6o5AylF8lQ4oZoRZFHqZkCRQgw89P2KzUmH7gaqiFH1y2jUa6g3eX8YImIYnVR17a6OXasRD0FP75agspD0+gg0yJnAqrZAszbLFErN3ODTUm1vwqsFaExO8hkPKystPHy8hbXNPrpbEWZZgCcCvrtRp7XZp6cX5+WV4WQEZDtw3+t2Rnm4eRlJhfqrgKd01Z4Ktdr2qjZ59F/84nOsxz4iDURXb3QbOK57XKHfMURIzBUHeJmH9978EeYm5+mBu/MCvDhR7xW+Qz1gx4C3d0TQyQY9ZP0Ym1vruHnrGj7+9Lf4lGB394GAbhVeleDGAiyg0/BXNfUmgxi9pINOt4eqync5ZNnzaSS6MijPzrx/5S3lwtikzAGFAzsNJ+Ax5aBM1NEclerVVCFYTVA/HJyawIl903j/wllcOLQfZ/fP4NgEgY73NehVBt1tlAiMwjCv6qNaZxyIcGqf72nmp37GeDEu1BEad6tew5JJ17acMx9W1b1k3UAvF+EC/ARuhXw7dvF3oEdNo3O80wHqq0OvNOD1rTdWnu0UrmGPTedzqeu/ZEYLNGqtO0oh75eCpGBRcKaCCtJO24YvVIMqDuyfx9zMLJ+khxb10G1vMxxXWHTOCaBAT+8owU8GCNTpRI3PvEdTj8UskNaRhe5fY6pBj7GHre0WC7aW2J9ioWxgfTPFnVv36XnQSuR7a1P76L1U6L1QoD3GnYUnoweqda1cQdW2+FOR0NfpT+fFSgOLWr5fnH/i+hNc/O4FFXET7FmnFR7pWzXLijMdmDflOrazGv63f7yEzaxKwKszjWFrJ6qLuIs+Q+HWfRNTRwYIFYpWSnj/rR9hZmKfzbKifnYaklAYFZ4GGZuX+M+RnvZdufzusH6HspQn5si2oEJ+RuVJ5YFnqcjNa7FwdCYPiT+6z95jCpoyrLFwpqhTllvVysToZFu2QsHf/uJv8JuPfoXF1UcGcF6NcdIcmLw3pUEa9yMCnda+5PMCKBqJavMWmKgpIaFnJkNGZVAseZD/aJHJ4yxDSk0gxsz3gAZTnZaQxs1VCVQ1ysU+ytX5A/vws/Nn8YdvXMBJengHa6F1Qql0W9Y+p1XHVXVZr6rZgs4gn+t1CXJZxvId0nht0GOk7Cb0SAV6FonCwyvKrf5M8rnNqdjh1nbzYwfYI+eMXf5JF7jwXx3as/Xw9pqkKp2VI4Hgn8oWJd3YhMAJQpE4AkYVBHuCW3Ux9in0Ejx5cWmp4kCn4qNXCdAl/81Hn+DO2iZuk7dYsMtT0xhUG2jT++u1OpitVFmAXI2ZZm6J8t5XWsVYdacaZiDLTV6nz/LnZ1S7YoJf2O8hSJbxxpl5/OxHb+Dt149ippmilKwYh+UO6iHVPgvbQOME+XyFpq5XqWlSGXQ7XRY6AgWVwJAcmNvWjgtyKTI8qSPGz9LDrrxwcuDmPLxEnUm4lQIQ4PlMt2xQR6s0hy8XS/i///9+ievrVB6Th7C61cb0RIgejZSKHzIgfRsNFBoWzVrDPLvuZoLjB8/g//y/+7+g7s+g5k9S71aYH8XLqRiZoG7M4958/++PikzOt3kZeZI076tarOSmuBRQTiiBeK89wzTSIqTqxKVeyLpJ+WWJqOsBf5tIKc8axqN7AhqVUtAxPbCIHlBmnVA0TJUeTVi2xVLXN9fwaOkRVjaW8Pf/9J8M1Ny783jmAEkJsK2MWtWauHXpZOQKCFkumJ9B4mYs0hg7ncvya4qsaqzjbhc+96ss15qUwmO5rWQCPKCq8tdbxWytgkP791NeDuG4DF4CVshvrPDb1XHFUxVlboRpcVWXRoqj/EQWdCWMvEmeFdl253jX2Xxb7Offu8P8LS7l5M5+PeWl+pWiV7fTys5fYeWQKXymxI0dIBasQqxCJZbXJiDSszrS0zqWAvaoCH3bZjgwO4P9M9OYqNUIOila9NS09pa8wX0z9MpoVSa817q50GMIaPV5VBAZPUvzGj16loyMvMGiDt8q7Aiu6qWVUhssbbZx9/ESVja36RV6aE40MTHZRMiwVpaXbYHLali1wfFprPW0CJ4eFYhKNN+rX7H70VZglxeVnQs589kiraxIqrDuEeUxsni4TkHO4lU1p/KiXwoQYQIPtwb4x8uPrEqzHzTRZRpUfT5DD1gjkzVYV22yMgqqNFLKNCZotGNucj9eO3cRoaelmgiMSv88WdwO32zv3bs0+P1Rkf/uOx3tfLyRk4PCsy5SYXiPgCsMQhORVJ1BKOuqRtaxnuhTpuOUssiyUKVno2d7vTaBrg0tiFxr+pTjKvORINdawb3Ht3H19iV8ee1TfHHtM1y9e4kGYg+ppxqRjEaiPD+xgI1GnEBPgpDHycXPya+rydHwa8aflwWPECjLyGS8BdRJ3MW0wIvXyvTKSgQ/rxdTokqY4XcdrPr46dkjePPQfpw7eBCnNBMKy9wk728Q5Oos/za3pcCObKvrW3pKNou4uHiNspElklj7o8znd3Fxnj/i3Td/69+rSK9wp5XnJAqYxFfgaPuF/Ozsl1CjxzA1MYXpqRk06/QeiGt9FppSNyb4sKBWq/QI6aEQ9DQ340BjvVhW6fehQSVrHh2fscZpvY1CLms1ZUBWuJFYT7PtThur62tYXl7B1naHUfBRCycxN3uAnlyCTqeH0Oe5qixyvp/WaZUWs3ocavxOEXdZnW7r9gUh1mOVrNZ5HWvAvOPig/eG9HaXAwI8GQPuWHsyEPr0IGI0cHe5i99cfYSNmHH260yv1NpcXQcfAZ6KPi17Ap5msZFhMaAi3jdzAOfPvIGAgCfPWIAnj9uBXKFsXCz++dLI9+1865Ddr/UdtK1jmSDKF17jM+pMpdU/ZFjYHI4yKjToTWlPwyPJuuj2NpH05XEzD2oMV23rg56tUnDv4S1ctTXnNIHzdTxcum/tc9vRFkp1gqmAjuXCTQLumBGwmLnaGr7XqqndmFufHrzY63vQVH+q30itBkSG0sAmW66S6zIItUKBZjchcGkC50PNGk7un8b5I/M4d2Qfzh87gMP7ZNSyfIc0VlmONTRJK4nL+C1WZDd5yWXGUiaP35DH9KJoDHjPSOZZmEWYF+9cdm3jZBtxL6Lw01r1A8xOTGKewDfJ/WyrhdXVFXT4fOapM4QapQmGcar+0QgJWBoKoWMVWSsS/HEFO6+iUddRWrHVRg0BwbQbpXj4aBMPFpaxudVDp9vHoUPH+aC8uQojzBdoEGtJYUY2c/pAnT0IXvYCsvuOooi6b3OKTmAnkBPg5czrTsHtDSmNFQtF3AGei4/t5YCnxV+vPtjEx7eWsZUy3StVpFRsgZJDjTf6dj4lr3dAgyOgR83AmKwVHNx/BGdPXbBB587DcwlkAGm7dmDPv7yk9BM9+Q3Fd309688p9FGWVAxJNRi6VysHVJi2pYpnQwC0gohmMNL0Xim6zMgYfkiwqQ7QiTaxsHiH4HYVv/nkV7hx7xpuP7i5s+6c1qoLGwHq9P40obPrWi+2HLGtI9W8mPnDeKqDh2ubdSvZE2x5rMpQeXV+hUDIrQBL49/kmQnkygS8WYLy8ekJvEZP7o1jB/H60YM4e3AOx/ZN0HvjvXyfwA3qoclnStZ5RZ1JnHyOpolEyB24MqY0zE+M6QXRGPCekSTGRe9LI25MfCnJEnQBWD0IiDEs2BELEEFnjsC0r17DDBWAenXep1emge4avhrwekirl8XSrOKYzxTdpFVQDOyMVbgd2JUDn94hgZWeYGZtESEtZQ9b7QwPH29g4dEapmf348jhI7RiY3Tbm/TsAN8foKsJaOm9DKxaKld9+adYMc2VuSuuTnG4L3PVqoqXs+ztyRdOhTIROS9U6sOdUx5oNFRUauLzm0v44t4GWn2tYB8a4Pk0GHSffQf3mAUEPJ1nWhDw6AvgyMFjOHnsjAFeRV38ngA8p1hdGr2c9GS+Fd8xuhU7Y2K479jJuatCdvngJEU/Ln3cWXXI0ixAlFrE9KS0vFImg6RMw8vvIqzTAKGn1ku26M3dwGeXP8Jnlz7CtduXsNGRJ7eJbtpGWqKBVqEnVqHBR6NNU4eVaMDZdF3ute79FpsiEoqnrqrHo/a1db0edV/C52lr0uOvIJBXFvfgsVzUkgjNLLYxcxcO7MPFIwfwBr2603M0WusemqWEoNijlLAcEuDk1ZWzVC2aqMqAZdharcClwAjlaWLApziSXZzH9KJoDHjPSFKwuxWtxDevUuNWAu+XaV1Swapnl9r1Qhb7Bq3J2VoV+/fNImXhqPD5eGsLcYsAxGd9gSSfS+gZDgiMAsQ+L7hC4gqRql5Een+k4Q+ZembSEwkm+HINqC2h3c3w6PEK1je3rOzP8X0zM5PQvIMCPy/U4pduwLbC1j1D4nvyY1ctpG8r1Irbqt1rLwHP0lzpwIiq05C2Lp6MIZUb/TUDvI+uLODq47aNx4vNo6UlT4NBICdVqSDk4alXjwEedZhHgDt2+ASOHzlF679qVZqaaaWo0tTtSiP3Njt4SanIuye/ozgu8pxsAjg8Z+lvLChzV4x0q+04A023q43M1U4wbTUVWC1EtUmkCdpY3XyAK9e/wMef/xZfXv0Mdxdu8dyyjZ3LVG2viZxVA2rr3jB7mHdxP6ZDFaFKD91nvGi/MM8VJxc3xcoiwozSfhE3yztt7WxGw48SkPUw6LbQb20iJOAdalRtXsuLRw/jnRPHcGbfND28JuY1wQS9Sz/potRr23i7RpWyQQGqECxt0DnDp0NoIKwVC+RjFu12RvbyIjaiYdzG9GJoDHjPSFbUrSotl+OdYmYQYlt1PtG+phtTlYlNHEvLUYszVn0Phw8cwKSGPMRdZFGXwJWwkDPskCWcSoFFEYk6sUg7M0QVavXYVLuelLp6uJXotfg0kcv07jSXbS9iYdOAea3j5/lUIA/o6S2gRHCtN5vmEabqoRZO8HmGmisBkeLsvqz4lpxz0NORO6c/3aNeZyMF+gWSez/fzfgX8wm6c+5Ps9VEaOA3l+/hxlIPXc1Lnw9fsLkJ+SHWFskgHODR16VHPciUX1UDuyMHjxvgaaXzf56AV9CT35EfW9YW13azfq0jhtvjKVcW7JAksBPIaQXwhH9EBhsPp3bndm8b661F3H14CV9c+RC//fjXuHrjCja31whqqrlgICwCvZRlgt6ctc+pIZsYKees7LM8MS9raQUhWWPSHNiJ+SwzSFF3GM29otpT0EvAFJfpMdZKBM2MkkFv8UA9tCEF7548hvdOnbCFVjVAfNYr06OjscpyW2Y5rWhlA5ZjlWn5iQI3zbCqgeIybjWfrqrHKSx5xzCx4sCNyLbF+eHpMb0YGgPeM5KKugpYIboCtoILZexZN2sCEC1ATUNUUm8tFl4Bhbwjra4+Xa9itlFDNaggpufV7nUR8z7Nl6k2Bldl6pS4AE9sBVzVM2p0J+ipOUrrv+ntfAEVBItipUSFEVvPz43tFr688thmopibP4bJ2SPY7qQ2Hk2hF99SkFMdPGsKLd/ntvgud56F25TI3pDioTSUUrOplqhwHRAxpZgWhYf3my/v4tZKjF5ZC7FQi/ImTSutEQVWPUtdVHh4mvHeAV6NgHeSBskxa78bBTx7A28X50f2+3KS4l7Ef/Q7vum8I+05eHF7MjiUHsoPGw5A1jyVGdNaTKTgNqL3toQbd67g8vVP8He//I94vHzfhhv49Py8qmfPJX31XtbCrD6NNoYto495nVLQbX5LvkczIAWpa5NzbcqjcaB8kiUfelIGjlgTMXuMi6fJIgY9+J017PcH1ib3owtn8bPXzuH1IwcJdDQqOy00GIcqy62vdjnGUe1zFh6BTCtlaDUElSAzaBkHgZ8BHeOhsjWc2o+kdOHGQNjSzHby3zG9KHplx+E9LznV74q8LEcdaZUE2/KYB3aXS1wn1jsF0u6qsPCy+HgBOmUPq+kAtzfbuPR4BV8+XMK9zRbq+w+hRzBTXxYNQlUbny3aSAsySzV7Ogu5AI7BE/eoLBQT/dHzYuH2aR37Ay0tQuCjJVtJUsw1gffffB0/vngc754i0A62rB3DU0cYWtSZZmgvs6ATrKUwkii2uAeBZqQgVCRUXwm/T9Yrea+ER8pF3b2VpnHZTeGm2KjttMyESAh229iP/+f/8Nf4u+sdtMODiCoNlwex2jJ9KlDPOlBoWadSxvD0HA2BuYkD+Jd//G/w+pl36ZFX6RyETE91dHD5qFRW/iofi7x9OanIvad9A6FM8kWl7WalsTM851grdKgKWNe1xltCQMgoc6pf9KuUHfPQOgSuGK3OFhYeP8Cde7ew8OgBPblNpnsPJU+dWiTHFrzJr1LVtVWrJoL5q1QmoGgGFDfu0cl4OWU82n3U/LpN/qwh5l3KbtqPUaG7FYYUagJUid5YiV5ZRZ2S+Kym4puo1jDFe945cQgHmnXsn5pG0/fhU7ZLvYhlpY+qvtHG0KlM6/tlXCleAvOiFJsYjGztA+y4+NXWyjsPFYrkb5T2rgS9mjT28J6RpPzME6K8FrBnwsudosAaAPGi9osqHruWe3nqFF1Wwzu3Pq3KWhCgWa2yQFYx05jAxtIyAats4/EEdBk9DGv059PqUi8FrZoevduVOFrAVgXEwknQUtWPVIa6gKtDS5yV0eqWsboNLC2tI6KFOznZxOT0frS7MbZabSqKKqpUCAJUmwg7pVKS5UqlZ8dk9WoLQ02k7ABnL0hprWmVlLaZeXguHlI6ypkMIdoEq199fgf3NlLElQlk1MKmsPuRjXfs00uW0yZrXd9nkEnDox42cebkeRua4IBO3RB0tfjWnUTP+WWlr4s/v4+kWWWKtRd9X1NvUfXTyFCNhUBOBlAUa6ougRdQa2r1gQqNtC422mu4df86bt27hi+ufoJLVz+1TilbnVXKIj26qsKiJcfXu/Lhyovlo7b8sRW7DXR5TBF0zAPb0gsL64iYb+2oZ16iRxBTTUnI8uUR5DSfZUgQrDN+0ywTxyZquHBgjkB3BO+dPIID9O7mCYzTfK5OcAtiTeaQWAcWtbmL+XaXQnncHDtZcGWf100cdNdQQuwBknuaXFzI5bSg3Udj+n3TGPCekZzAO6E3oBNTeouCO8p2Tt5Qvi9WKSmpxxkL/UBVJ56HqVoNc7UG9pMPNidRpfIN6IH0trcR0fIsU+l49RpST7NOsIAyIE1zxNDJcgMJQDnYFUic0Fql78JnG3TjJpD0q1Q6Zaytb2JrYxHr6xt8MsDE9D4C3xzDYdj06jTVka28YJZ8xapx1JVf7RMC+DKvadHavQQ8V6WpNrwC8BzrT4C3TnD/9Zd38WCTnmqlaYBnCoeAp2onpcsQ8BSergHN2iTOnjqP2an5VxDwJE9uW6tVTQYEcFHURUzgUKucT1DRYHFNhadekxUea4HVFBGWNx7j+u2ruHrzS1y5+QXuLtzA/cd3sL69TG+vSzmUfCbmEaoXsquOVJlQjqqMFHFiivO6pTvl3AxMbQV0zO9SpULPnt49LT617/k+gZmg69HDK3VbKLU2baXwQwTrC/tm8O7Rw3j/6BG8dWAeZ6cncbQeYgoJJukIamUDL6YRRCPPF9gxJpphxTpFKS78V7zUZu86qqn1TjGj92fJJckYxtuxyO2rvBepqmO7yp/irjG9OBoD3jOShNWBnQ6cODtvTgVX22EhdlZdzrrArZ3S/eY9qVcnrMeZlh5qsFBN0Ks7Nn/AFpFMel2bVDbjvQactEilsDXVkQqdGuStqkX7jJS8GL1AE+KyDCNTL071NvTqvF5jrlftWhy1aXW3sbC4yHM1NKdmbeYLayfJv82qr/hOa+fKSVa2uverrdApqBdPStkh4EkJFba4Uz4CvKWtDL+9fB8P6dHGBLy05Dw8AzyCXAF4+jYpN7Xh8UFM1Kdw/sxrmJ6Y4/lX1MOjTHdtIuOUoKcqQt+YOEPPL+K1NmqTVQptH720hcerD3Dlxhc2pODyjS9x/9Ftenmr6CTbBIkYYd0zD9CtPE6Pm55UpaIZVpR3eqOA4Ym4GOBQNJkvquHQrENqKzMDjHFKSjTVghLLCEOhJzdobSGIOphj+ThSr+LtI4fx5sF5vHv8KPcP4fT0FOZUO0HwBu+t8XVl1WDQSy2xoFQoDB7jYIDKGOntFjeeE8t4tfQyeWG8GD8XY/3mrG8wpizl++4rHBUyZL9ud0wvkMaA94wkWaVIcysvZ3fBELktmYLvxgCpUVucdzqhsnWDvll4of7WDEftZWS1N4R8tE7LeWaygdnZKVqwZbTaW9jeWqP3FaFGc1ZthkUVqasu1Ztd2GWFW1bbkwov38PSmvEdNqsE46z7SxXfeoAubnRpjT/E4+VV1OpNzMztQxCGBsbSOWrDsepLhU5lw8Dd9/Hbiu990aR0V8WlklgenjquuAtSKbxSCvFgpYOPry3gcZteKwGPKpbfzOtZz3msJa18zkLAfe1IscqTmGzMEPBeJ/DN8PifK+DlcVemPi0PKR++z3Ql4CVpBK1KoKxX+5jABl6GdrqJh6v3CHCf4ZMvfo3Pr3yEe49uod3bpOfH1CYY2di5UoZ0QK+un9DAUO0DgcvXIrwuDjuAsBMNd0ZyZ2DH+5W7AiSbxYTnNS9lZSCvrGMDxH16ddM8d2pqEu8dO4YfnzyJ908cN5A7WA3QlBdHw7FMsPNVLc932fAelgmzE/lyrWhfVvU/r6mMuPKssqwY8T6Ln+Im2XOHNs5P5WB4xtgA3PZFxdbRjhwJNcf0QmkMeM9ITqzlXbkDE/CigEj4TYlItKkqedKAjqBjy3rQFdTgV7VClcuBNf7rGlgQxR4BzCMGxrRatapzvVFFo1lFSJBTZ42Mlnev04JPK1aTTtuEuIqU3iuAMwVNMEsUE4/h8TkVSlVJqg2GysdeRy8o8xoEvRDtboTHi1tYWV0yj25qchLNZh2Bz7jRKhb42YKY+i6ymvJ5o/v4PaC8QtLSPaMmdkNEFBexoDDA7Udb+OzGYyx3fcTlOv0Kpg2/rTygUcHbCsAT3hUenjqbTzamDfAa1SmXlv9cAU/Cqu2uT9C3OaWuKs0SZUsyY2tHMgW1Bpy8u1a0iV9+/Lf44vrHuHrjczxcuoe437GemWVfFZ+ud6bNa8nElpklo8lqBhh6WY1+xfv1tp04uHeLBBnKJ4GddfenV6itfMRA61G211GL6dEx68/OzOCDkyfwk9On8dbBgzgxOYEZPtugzGvmFBDo+nFk5UcrXQS+63mrdfAM6Gj8GdjJ86csaPFmvojHFpE8mq5NT6wksqrW/39739VcV5Zet+69J92EDBAEwGZodmD39GiCRiXbZZddpb/gBz+5yg/+Ya7ys17sB5XLZdnSaKSZzoHDbjZzAEmASDefcK/X+vY5ANijkTkj91AU9iI3Tt7pnPutvXZ0RzyoCM5ufGFfONlzOD7+7gWP7xWe8H5P6PMuv3ojD/1g3Y/W/UKcgXSqzq1d5YhO8/kFts+f8Yw/MhpT3aMnjEtkBwIaDJaMNdfghGpEJWMtJ7K+vIxz8/No0UhrgtshS9sTOvXQ1MKx6oQxo58iO0xJePydN/iDDvgj1nJGtTpLxSyZByH9bgQYo41+SkKLEnQXFulHhu3tHg72tq26s6H00dipR2ND3cNpBER6+jHLqRffq4IZw99CeMp3DfO//XAfX93exfNJRIXXthUtROYNKmRtRXhSvZXCs4IH/813FvHO1WtU0V2+3n/mCk/uOAlMl1AS3t7+Lt/xDElTiwzX0Osd4u692/jii8/wyZe/wvV7n+LJ/n0MJ0cI4hlaczGiROsCZBiRiDR9mBS1luLRhAqqRtfvgxxE4mSBhfmtb/4FsrPflNvXSuJGMCJcEp1cxG+5FUdYigNcYGHw7bk2frS5aYruR5tbuDw3j0V6EY3GwKDv2uX4aej7rUmxMh1anSS1JJa/lbomJHDfEKPGb4eXGG+LVxkXFbD0VYkwbauzvF9foSWCdziS0y631bParU67zfElReH4pMcfBJ7wfl/Yh1pWe9geT5RG9/gffzEsIJ+48jGzJ3qO11Vtow4iaodTlZF+Pzl/4BqTF1BdCTLIcdBAN4wwx3MrJL/lxXnc33nCnx5/cgWfpeG2Rn0LRR6RWG3+Qv6I6Z+6a081HZN0jjq36Mdaa/N6QqNf2IKYIrCkHaM/nuDGtwc2Se+EbNpszyNu0vjLMNDvGonC2sIYuqlcQ/WDV/gnTjESlGYZispw6O7T7W6/zVXPn0YVkoyxy3/lk1IuJaJ3UUdWa+HmwwGN8j6epzEyLYvE+5kdVi2mqlkjPLWfyjOSuQhPi2zOq0rzyntohl0ea0JpkZ7SIzAh9gIFF8ffF5VCOPGHzrw7dexOGKpQBXf29Jm/DyfP/iZ4zbyXH0oTvySpsbp6+eaY1jO05xNS1xiPdx7i0+uf4Bcf/Q0++uJj3Hl8B7v9Z2QVKuUIiBOSBjMxZSFsNBnz7fKNqGcnnX4TKiRpsm7VEuj96Hu2qvGSZEQscoqPqhn1fQT8ZtVbMtaW/sb8PbTpxwLZa62VYKsV4l+/dRE/2lrHe5vnsdFpl/Nf9m2KsITp0SThIQlOKlXp0+9KSlOkpsCmmoWBAStsxVlLPmmrDlkN/t5OhmPoezv9pSrP9Lciu1Nvwg51dHzGUN527MeL93v8oeDH4f2jcDrr3M/A4cWvWDalOlPtVz8h+6Hbk6UrDZAZAdvjD7MkBpkC52pUdzFujlJc397BjdsP8ORwgCLumBvQOPfSAmGLhMaSrWxLkY9RZEN6nyGhEYjCFkvhIY0RjRL9k0FwZJRRjaYIZyMamwEW4xxX1zv46btb+ON3L+DCShtBMUA63KVBGRl5qi1GtbGaA0rDJdJMvfpSdNothic/VQ2lqZY096H21a7TQFoj2UrS/hY4XnEpr7ZuX+A+oy2bVCNJSaTNaJwznsyCDibhJv78f93Df/vrW3g0jJA355HRkEtlTCcyvDK28oM+ZjnmmnymN2HRv4Z/9cf/Dn/6k3+DdrjEgoQmjlYeiZj0XhSga890JGgv8HeGkR2JVM/TV3dSsHS49L74bfCvyxCDnpJyd9XUvPf4kouPK5Aw1qZqSSHMKBVs5GxJJF6OqNy1Mr56CqvHsMgr57sfpn2MsoGtTrBzsIPHTx9jZ2+X38vE5RkJQZWUGsCvb8bFUN9p+XYsDXo3riDnJirg/XxWBQ4b3sDC0xFFWCNStTk/0GmGfMzvM0vR5GFbNRe9I7TpU5f3L1LVne/OY2t1FVtra1jrJujyGw3VK5P3GKcZWTrSrL4dcxafKj/dvhWl7ISc3Xx8vTo+2eout2+3lKddaCc4ef5FVM8aTu06zzz+kPAK7x8FfbGVe/HotDt9oB+Fc/zyX3Av3lz9k2lRD071CLPZ3EkYoUiD+91OF/MaRNvUgkI096MhxmMqNVqVJku86jaulb1VwpZaTKgMw5BGLqeiG4wQ0firNC1IKalVrCBp5bWI2wiNuI0+SXX3+QH2948w0dJGJLQ4YWm61cRo3CeBML68V6SX54wo46t10DTP4IwsWKm5alul1WhQM5jQ+L6I44wo/764rfYVkqqSlUO2/AvP1GoTa3PJ6hHG9Xl8cauPrx/00cuZO+okATeubKoVdkW0jIsRJRVyTGs/zZgDJM+Lm2/iwvnLiOpN3iKyO0VMfMZIz86cjtHvBvdmzTz/pg/VCeVVBds/OdYthSYml3E3AnNfi0VRiSJ4ya5rzJxrf+V3ROUi5RVQ/QzGR9beFmmF8GBqvSrvPvoWX339mY2d07I8j57dp9LfxVgrE/CemjqicCud5N6pguQ5kZ25snDG/TrDEtnqG1FPWFVR8gpJmt8klXU94Hem8Z5UZFrCd46k1o34LscsvB3uYb0V4435jk3irOm+PriwgctL81hm3DskuqYWYSVB6zdhC6xafKwEVDplRuXs/wvuxesubs656y/ce/qIaXHuO6dfPDx2L+C3XvD4Q8AT3msK/SwzGotOu4nVpUUszM0haLDMnaYYkfgmo5G1vekHbFUzNHgygLRE/E+jQgNf6Ub5dvwDLH/4avMrson1kAMN1OHhGI8e7+H5/nMjws7CEmKSnqoFtZCn2mRYKEcs9VjTnKFjkrSmXsrop6pQaaAZbs6SfWaEquouGmkZbIuHi8KJU7xOtoLO62+1df9VICCBKX0Md0o5m9VjjGZtfHnzAN8+PMSAJDYLaFIVL16fFiIIkoPSSj+qtfA06DxAhItbV7C5fhFhPeEtIjv6b6Eqn+SYn3ZG5+R+D9hjLu0nhZ8XDbXLF1dQsHzgM9W+1EnB+GraM60zZ4qVj1otHG9VftvMJPqvKjoWeAINVOPxhEptMOmhs6ROT2Ps93ZJdLdw/ZsvcP3rz3H7/k1sP32IwahnK4/b0AQ+pwkHVL5RQUYNxHWN+WR8racwzx9PrKC0cWskW6bSSNgKN4y9buC1NuPUpPLXhMx1hoX+IVr0d72T4MrqMn72zlW8vXEOV9fXcGFpAcvqvKVqSuZJoBqDmRQmE+vh8ZLwhPe6gjYjm4z5o6exphGYp6JbWpy3zi2aSX406Jth0owiNq8fS9QZFQF5CUEQkwwTKpoJvXFGyRlZ7ZTGlec1V2CLpNZut60NZv8ww+5Biv2BZrkfYGFpmaV0+tUk2ZIwVK85pfFCMaLdZqldS8Cw5G3Gj8bOqUcSnlYfIImoRG7KTMG/sHVG7LvGzKJncHs0rTTA9IeyzsYzivBI+hOS1rBo4ZMbu7i93cd4xvC0DiENpJarmanDkNG9Ok4wFiS8iKQhwosaMS5tvYmt8xcZN9d+970QHuFm0j/lSrJz6a6cQvhuHslJMYUkoZhpUC6K7MrCDXlFKn9+ccEGeA+Gfaq5vhvs3SCpx3UESQ2Pn9/FvW0R3ef47KtP8O2dG3y/z5hPmgmlThVPlW9tYApPaWb4zOeaFaAKntd7JeFxaxxWOosg79U3o13RnBSeBsg4pacxpyTrwQFa+Rhtvpf2NDPldnlpDj+6/AZ+fOUSrp5bIfk1MU+WDXP1tBxilvL7yrXOZGbk+91vxMPjH4InvNcUMteLnRZtfIpMa9sVGTpJhKVu28burSzMY+/ZUzPVZnD4r2HtZVRZNOy5Bv7SWDlCko80Zi8YWxJjSIIqplQEmvO+QSMZIW8E2N4f4etbT2lIRyzFhyS+NSPFKeMyzYaISHQdNcQUIlSn7qS8RHaqLpWjuaPRk680nAr773GColY5nT05EtQmRLVmI/5l6BlWEGDCOPXSGB//egd3nw6trXBKhaf2KameGe83tcv0WhVnSXiy6UnQxOULV23i6Dr9+f4ITxGo/CudpfBk63z/7rZ0JPk685H0wSuO6ExRMSsamgeV5HHYP0CqHqlhDS2qpjBh3lB5P32+jftPbuMvf/EXuHH7C1tgde9oh+94TDKsmXOzAGUoSCxSeNbhRCTFsDXpgaopC7XfHpOd0mARM6fdShFaRxTGT4O71dOSTIiIpJUM97EwnWC9HeGDNzbxL669jZ9dvYwrS/NYoh91Kr6Eha4mv5OE30mkqnzGR2SnWge1YSocD4+XhSe81xSyK7EMC42IlJUMisr5MY1Mt900wquWJRr2BhgN1WGFRoiGX4pAc2I2rErKmV5nOZz5qIyIGVJe1JAH69HQSEgoTaSgiuS5Rw93rG2vP1QPUKDD0rjm5lTPOI0hFIMY2VEiOKfZ8NVZROpOawSqg4wIT2HKSL7o+Ef/zZ3slY7/NT5MadGYRhl8DbsQ4Y2mAY7SCB999QwPdifWQ1PKL9cMK5oqRARJD9QSJcJUW5gIT9WESegI7/zaFg2rG5LAhJfhKk5y/z8Ij7DHXTpPe3Xax2r/u7cZ0TEa6mWqNjr+NfVWV72yGn2ZF0WNii5goSDSCLqJkdo3t67jlx//Lf7u45/jaLyL3mifpDh2vS2p6OqaIozvZZSOmV8kO70Lfkea1s6qSEl2bpwac49ZY2UNBunaEEtyo9Oky5qLssHvs54xHiQuDfhWz0uKS8xRjf9gfQHX1pfwwysX8f6FTVxaWsQKw2mz4JTwmThNSXZTIzo3r6Xa6eg3ww0CvhPLE4Xq4fFy8IT3mkImO6XCUqlbVZYhDbmMgZEf1Z46sayvLFNpsWQvY1SWrDVbhcy3Zm6ROXNzfJqpkrcvQLOyhKHGVrV4T8PW28vUQSSZQ6e7hMFgiKPeCI+e7GB374AGUtVlIsYa72VJXB1kSHSuI4OMpXSm7LE64BR0jCuvOIJzMaj2hZMt9xhHd6Xcald+0PrVtESMqtlE4CS8IUnqcBziwy+f4fFeRsJrSa4y7ikJTySm+5VupxCl8DQkQYTXijq20vn66hbzyw3i/34IT+G7beWqXoOn/bX90rCf/qczrkBC9aXekiQ2dUDRkII0H2KY9pC0A+wePMH1m1/gV5/8DT767O+sI8qz/cc2HVh7IaYapE9KHkmyYP5pCEumTiVUbjUNayEB6T1qPUX1wuRXVDqGxbxWQUbxUS9JOc3tGomk+D4i+hPzW2zRz3m+3FV+ixcW53BlbRlvrS7gZ1c2cG1zDZdXl7HAcBr9PmZHR2iSJOcYnobgJPw+VSjKSZhauFj5ryWDAoVd6Etw+eTh8TLwhPeaQj/0KGCJXMpJ8ookIvKjWaLhoWNJWupuaX4O51dXMd+m0dc4qd4hitGABp9GhEbDLatTmtDSdrgNDZdN/6S5E2kAaYzrkgH1kGQ2xXA4RqfVZWk7xigrsP1sH/efPMHRaILm3DyW1jeQUgVWa9XJV5X8XS/T3MhOJrsKzcK3Y+GE2qrrtjXSc37ZaRGeKTyNp3OER4bGoKjjYEKFd/0ZtvcLTMM21UiArJg4ZWCyRMrzRcJTW6BWSri0dQXnVja/Z8ITlJbKOb/sn2PC0mt3zqHalxMhqToxI8lR3WlV4EZOshphMDnE4eA5/vLn/5Nk9zlu3vk1Hj+9h4Phrk3gXAuZXsqs4bhHgsudd4yC1JyUm2YbqRmZUc3R6VzK/NUsKTquM4+DSG2ieht1E5SMCkKRHb+rmAWrJgteHfoot0jVudFp4erqEpXcBv7o8hv44cUNLDHOTZJibTxGwAJSh+9yjgWSWFlMdVeMRtYObe1/fG/qfDPjN216lmEpn07yxsPj/w1PeK8pRAkzKRUZY5KKllJxplBGSL0NuZXxUnUSS9lzLF1vnVvB+eUF1LMxDvb3MUvamNBOqkNLSENjxl9KkIZN+6outMYu/hexyvCo6sp1POC+2FDno8Q6hRyOUzx6fkh3hEe7R3jnBz+xgetZqm7jAE0kgnyMKB/RSNLQkixnUlYMS5FXPKzziYEpVBi2R3BfVtnSbft8XJ7qmVxL2EQYpQNbLqbWnMf9ZyP8zScPSX5tDPI6ucvN6K8lb6w3pqSN2qFoUEWaCZVsRgm7OLeE997+wObT/H6rNJWX9F/vToWOaqv3aMd8F8x7EbnGzikP9E7U6cZVKVKfN6h4wowExHtqE+tteef+N/j0y1+Zu//4Fg4GO3wHVE68txYyz0mKWp5nSlUo/6o0KEVq0VXeWgcj7tvq+DwvRUdpTDXolB5fvs7YRa3Ar0nPI3oQMZ4J8zOhEtMg8JWwYe1xPybB/ezqJby/dc6W6FlkwaTJ7yBIh04NMqyIYWkydJtEgWlmzlj1s02JxzgVcozvlM5qDXi9ir2Hx8vCE95rCpsKiWZB7WInRldmgPbISt40JDQOUnmq0kxoqFphgG4coBPW0el28WD3wJ6PVW3EZ2dUgEXqVhJIotiqkFRVqLFc3JFFNJuvmTJsomWFJsNMwtDSO2MqpH4GHI0LPDsY4MNPvkTS7OLiGxctLvlwgISEkQSSE1MSUY3KwRkvGXjrPs/71FZUDZxW2ixVdqx7RY7al3lWuw7/UtHJOGbTiXVOKcIWHu/lVHg76GWJtelR2jHiVHs0ymrzczN9iEbcRm14BZXqQncRVy+98wcgPKk6+att6QfTVc1ZKpfnJA8WVFrtJhVOHeN0bPNYkt5Qj0El3UBvvId7j+7g1zak4DN8KzW38wD7RzskJxJlncRIp+2sJrJzTukQZVj4CpdHrqjkCE/buMlwRXQiOH4D+haO10hMMyRSxHxGiq6hiZlHAyzw3sssVF3bPI+fvHkJ76yv4sryHM41I3RJ0nE6QkiiC7KRFcpUvW0KkVlqTY9yygrLEL5lxs2RnOLliE/HuqN6Kx4eLwtPeK8pZJxyqiMZAdWA0U7QbjmzJSOgrUhK5GczzNNMitjaUQPzzdAmh55FLasOHe7vIaexini9laiatIacSkgeOb/oStLT4GH1vNOCtVIemmTXBh8wLiDR5LUYQyqq/niKJ88GODzqIRtTYbbaWJ6fs3X/ZCwzKoMiVlVjRM9JWyQgU3T0W6RknVB4XrAU6UJpkN2+coA6hIZSbXgiSakejbdTJ5WHOxOr0uxlsQ1TmKntriQ86+HI8FSFZ2EyaVoSZlbMGMcVvHX5XXSoEr9vhTdVO2JVfVnFhe+AUVGWoNlJbKWCw8EBhpO+VWGqY0mQ8Aaqtc9v/Ao3717H9Ruf4+tb1/Fg+x7v3eM3kVJ1iyAqghPxqaqSjnGXU9xPp83aWRl+Vf2sf/oGVKUoUtKkcvQS1OT2HTV5LuF7DQYDkt0QK3GIt7UUz5U38P4bG7iysoBLSyS6VoilgPcXfAuaDD2bIJ5m5p/LO9KuslR7ZRWl3rFTmqWqs30VrNw5F3vF3L4GD4+Xhie81xSmb2gMnL2U1TczYdecGeF19eAUaJxmuchBPTml/CR4QmxcuERlQ7Mx6mM2UYlbVZn0Re17MnSqUlIPPYZj1Y4WqjMytDsWkNboU2cVm+eiFqIeNlEP6ChB5rrzePL4Ke7deYqM/s/PzVOxdGjQY0zp8iAhQTINjHZRSElSQTCOJcW5MCzE6kCEZ6cI3sN4SuHNtN6fCE+D3KMIaT3BvadD66U5LNol4TGdUjZWCFBVGY+VZ/KX8ecZ+lfD2vI6Fd7baEaM5/dMeJqdX8Er97RsjmbFUfFBc1qKrPqjIyrgMcJmA3OLHZucuTc8wA2S24ef/S0++uJvcX/7Dnb3n2Gcj0iI5MFmQELUIPO6ddLhm7d0WqFIcXdBE0wv88HytExCVVWsN205ze9HvSON5Bg/dUAJqe5CEmGLboHvaoXK8+LyPD64vIWfXL2Ea1vncJ5EPU9yjVV9nal9jnGjOtXacxo0rp7EWq1A300VnstL5QXfJ3f5Ksxp+Sq3785bXOm0scKcPeXh8XLwhPe6wn78Jm+4S2dbZwicRRCR1IzYZFjtHA2MCII2xIxMTBJY67awRYM1RyOZpyMbsK5lXNRek9Ggqf3GzIpIj0bHORpQGkLxgJtBg4ZJpJeryov3Wze+GiZDluZJalrZ4fH2AR4/3UURtBDMrSKXGmQkzCDzdkFteLK56nEahuqF54jFYMa4pFvb51NUdEoP2dPatjRaEGGE0SzErUc9fPzrXYymbaRUnaoK08TIilydEbc2MvphXe2VL8wr0ibOr563Ks2Y8fy+hyVomRqXerXRUSebEqtUGePaIEE0meZ6ip2DbRsgLqL79KsPcev+1yTDAVUt1Rz9CZOQ70yat6CAnmCoKkbmifLKkUgZV1NwctJHCl/ndEnxsDup6KbQNHZz/AaaIiiSW0AVF42GaPNdLvDeVSr891YX8UeXNvAn197Eu1urWAxnvK9nSq4lwpvlrt22DMlIVPGwvNdXK1RUR3BHpHxMdjqm/NO+y3cXR5eC49h7eLw0POG9pnCldjILnSO76sfPv2bUaiQhGgZV5dVp3GQraGzMwGtXKq5/iG5YwwpJb2mug06rabPci+hGqnbkQ6qu1NYK4yI9KUDuqvt6mk/ovzoXuA4vqha0BXZIHjUtrJlTGUQJRVcTkyzH9vMB7jx5jqe9nMprhsWFJjTbVZLoHo0PlIVjpOkUis0T6WJrcddWYbt9GTwSs4hdU4WpHVGEF8UYFA3cfHCIz77eI9l1bSozp4ZFkHpOeo5PMxypWM2laesVMv4b5zaN8BrUNd/vwHP1fk35lwTMIFguMVWmvjR1NWgFOQaTAzzcvouPP/slfvHhX+Oz6x/ZunOTYkgVx5DVS5OEoG9BY/EyEozSpDyyji9618wrRxGKp1N0FdnplPuOXLp0p8hOTr0tZ1pzMWWhhfGc5/VzSYQ3V5bwgwtb+OGFDXxw4RwuL3ew2g6RaIzjpI9GOoDmt2QRw3rkuuV0FA0GpnZTvSfGTZ+HoDDdW3XxMLKzNDnS0/Z0/NxUdEqF80D7Hh4vC094rytEchpYTCOusrKMiuGYGOrIpLRoxG01A9oHMxGqyhN7saSuKZ2ifIwZiSukSpjXnJzLK6jFCUmiDq0fp/kvU6k3PUtjZb0E6ZMIT2PA3MTA/JAYRsTrrSBCxGfrJJG5dpeKccx4zJDMLZJ8Qtzf7eHe8yH2eke06YcoxofkqAgxndoG1SlipqpYGUum5digHaeLW9tnCpkHIjwpPBn3QgUAKrx+VsfX9/bwxbcHmJDw8oZTeDNNHs2/gZEY80T7alNUfjCOQS3AhfMXceWNqzTU0fdMeFLEmuib7069NRj3cdq3npZPdx5he+chfv53/xs3vvkCdx7cxGH/OepRQcXHFNQzmzElIylZL065kuhUwAmYBxHfoZS6qMHFv3Qz5pUIj9GeHrfrnZCJ3q7UnQZ7B+kYyyyIvLE4j7fOreL9jfN4f2sD1zbW8dbaElaTGdVcH8XgADWSnVRdN2FRQflTpCiyzFVTy3d+G1PGTW29OfNZMROxqlpSWeiIjc4UHV1J5EZ2ihu3uldUbc/xrOH3zX6PMwlPeK8tZATUeVzGSv9Uucd/MytP01BIzYmgnGmwnpaEtcfRSNRILO2Ipk98IYNJ46g7m1Rki3NdnFtaRn//wIzLNFVvTV7nDVJ4bn29wubZlH9afSHlPVY1SL+nBdUhlcF4ohk8Ahu3NcqmNnF02FlEVm/aQPVbt3ZxONgnwXYQJV36FVk8ZZoDRtIZNqVPKXJ/jbAVU13nsSk8pY2nChnKoImjLCbhHeGr2wdI61R4DNdVi2kqMz6jqko+oOo/q/Yj2TFJJOwEW+sXcfmNt5g/IbQWnsb4KU8t/CoK2tiW52mlT45dXH8b3BsowXfXaDIlCfO3kdsK4updqc4nn1//GF9c/wSPnlLN5SMSHe+n6htnQzeDDUlSBQTlt4aTqD1MwzL0LjRkIeW7HfN9VIPFXWeUU06xZF6578cpOq3Aod6WGlbQylO0WQi6tr6Kd0l0P7pIRbe1hTeXSXIMRwPD66M+MD4iufF+vmP1Aq6zAKRVyflBmeIPSbzWG5bOahmYXyI/jd9TwUaTwig2IjpF6Zj0LH/0x23kyH96xYp2dalMR3ng4fES8OvhvbagoaD5sB/8sYU47YRqexrV65ahMd1GiFac0/p0GZXYpBFiHMb46uFj/OrWHXz7fA+HNFp5q4VM7XskAQpEGqKSQOmLbWWYtMMzpiS5dbEoDa2Fw3M1rTp+iHp+gHPdEH/6/iX82R+/i/c25xGPdzHavY/1eaqU8YD2k2oj1pg9Emeu0j8NPI/zyQhTqqR2q0EDryEJMUaYw162iv/63z/GX372DP3GAkaNiGQr0qIhntIQ5+oZCqbREXI6yNAJu2jw/L/90z/DT9//U0RokZNosK0dTxRsppkbpal0IlrtElK6lgE8IbWlTjgaWiAFqZUKVKVXjakTpkGGRjfFNtP57a1buHf/Dp7v7bKQMOQ9OWPq2vX0jqsVCUSS8l9BigAiKG6qOZ5aNXRK0tLA7DpVmaYCG2pycRKRqm0VvgoGKtxYoUKFiZS5FYc2m8lsNEBt2MdyEuGt9XO4srqCaxe2kLAwk/A9xvzUYuaBxtyp0KDCQz7rM15U44RSfpwZx6jOaqv8E9xWZ1TdWZ09Jrpy/wXwgvPBQWn2ZOfx+8ArvNcWIhIZ8cpV/06Mg4zJbzp3TUZFJFDNtKJaNc1grymhYhKMBhHPaDCX5+awub6Obrdjy8oc9Y7MaCdUgvlEJqruqh5lNOWvLDH5wWoazVjzpM7bViExPJIkqccGvmc028NRgd2dAzx78gyTEcNcXMSbl69gjyQrzlQHmoDk61QCBZmIg05tcWrvC0L1Kp1Q/EVISVTDfB6f3tjGvR0qIvUGVVwYvuWUpkazakqea5BAeG2WqTo24bUQb158F+srW/Q5tntl3itlwQNLgZIiF6nhjVDnGkdkzF8RjsYTBnU024m1c/b6h6bO1IsyKIcLjIsj/NWH/wNf3/0Kdx/ews7eEwzUK5PpKGzSNxJJnSxTVjkqAopCBcUim4gQlTCGSYJrkOhqWkxV6TWyZYoVUeaVxs5VQww0NKRLImyqDbF3aG4tCfHjK2/gX157G390YcMGiHenGdoFc5TfQ0Iy1VyYIkq9BVUIWxwZr5Mc+fvdi0euaGVfi53gH4uky9l/6F/5QPkMdz08fkd4wjujsM4BdCKiqiOA2VW7qpM09TToagtKmm102h2r7iQNYDacYHTYp3JQD9CGzVqvGfSlIjSuL89yZFqNoeGIxflpdpj7Iiq1P7oODRrTFdAgDw972Hk6wP7hHnJGbEKyaS+tYhpGmikSExpsqZ4wJDlpHquCZ1WdSWMcaqYRHmsG5KLWRD9t4qOv7uLR3hhpXeqOoTKOgghXqk17mu3fep1S9QW1mOovwjtX38Pq4jq1k5YGUjWdKxBYKsqEOALif1WvapfxUHUib2UcNUwjM+LSyuHTeo5mJ0R7Pmb8MjzZuY9Pv/oV/u7jv7KVCp48e4SDgz1MqFbVNmgdK+mP9YRVOCUxOLqTI3jexqgxf3Pmf0GC1UwyNu0WIyTyLUhmCa9LlSV8xFSaplBLUwSTFOFwgBWGd3GujQ8uXcBPr17GeyK6xS6WSH5zzOdGNrFhBHWr+pR6dGpT/9Re6noJl3Hy8HgN4AnvjEJE5wjPlbZly51z+7S4Vh03Go2QjjS8IMS5xWWstucQZSzdj1OaQJpBEo46wFiHBxpcW4mBNtCUWUl48tD8poE0/0uyU9tgRALS4qthQEKgoe+PC9x/8hxffHMHneU1TAOSbKdt8VX71WxKg02iChr0iaSo6rkgmCJTHEh4GZrojSP88rNbeHqU2dp7UrJKK5NUklgZL8paVbsGNVVdhghrCd5/+wMsza3aPdZ2Z4QnBnKPKL8c+Sh8qU1XTaiOFnzI/FQb24yuEZN8ahMc9Hdw58E3+IxE9+mXv8S3d3+NZ3uPqI9GJKiMZEnlF6rNi+qTqkxhiVRMFRuhiPwsSHN6QzYbCgsgGR+WztM8l7oitR3wGTX7aZWCkAUPkZza5tp8VwskxpU4xvk4wgfrK/jplYu20Opb66voarq3wSHq4z7VnMbM5W5sJp2rTlWRQ2l1zvKTZz08Xhd4wjuroKUyI03n7Ki2pxwv2Zp5tLSqAmuTlJo0st1agNWkiY2VVQzSifmTDfsoxmPt2oTT6qwQRjGVhgylhUbjKMOt4xN1F9HvOpmokJN5rTcxpiY5SGt41ktx88E2BtkMra4GrCckRXVm4bMaYC5G5TMy8gEVXpqnqFtvzCYOhyF+8ck32B3OqPBi5CRHKSLBKTzJKOo2ErRWateir7WiQbXZxA+v/cimFRPhaYC6kV2ZCGtL4z9ljhEQz2t7PIuJxtHVMuS1lFQxxv3t2zaf5fWbn+Lz6x/ixp0v8OzgEZXlyMbXqYqTItG1/6l2lv5qAHpGclJBQsvyGMEqLIVz7JhffBe22iD3dU6dj7Q6fUTS1UBxzYSi6b5UbalOKAu8Z6vdwrvr5/Djyxfxx5rf8uImNpohEqrLxrCHJBujzfzV2nNaukmrForspMrVlqiekyK6osyHSvt6eLwu8IR3RlEZT+EFoquOuaOqShlR6aFGUaBBZReTaBbDEIsat7c4h047QaDlZNKxtRNp5QGzzvJF9X3aL8OxKrrSgItGQvpMXUM1FNnKCmMSzIRqq9BMLUkHz3tDPHu+h+0nT6z9UL1HV1aWbUmg8SSnMmoypvTLFF5OAqGprrWx12/g5x9+TeJsnKrSdJGQYnNE5iKRM01J1ILETdJo4Ucf/BTtuEOiFwPRVZFXvGX4j9PghmZo7FzUDGyF8LQY4unzR1RwN3Dj1pckuY/w7b0bePDkDon7kCQHxO0G6Xpis6io3U15oYKH+c5CgLZqkzPZZ2RWOouHc/amSHjqrCJVrZlzrOqR7wDjIerDAV0fa0mE863EVil4b+Mc3t86j3fPr+Hy0jy2Ok0sk6SjyRAY9RHkExZopmiS1NRjEyTJhtIqGVum1+KqGCp+jJPLRcXJw+P1gCe8MwuRmplOM1nmZOCE0oZpKR1NT5ZPqCVIZKom06KzARWIumjOL3ew0I2x3EnQiQKqpAIp79VyQlJ3Nl+lGWtVKTqVJeOt6kWxjciHVh9TEp5cqh6iGqdFU6rxfx0qO83Fub3dw/7zXec3g57WYpJci/7HFtdGUMOYarSu2VvQxm6vjr/68CZ6U00zFlu1n8K29jqFa51RXLtbluZokVynqVvt/Mc//Cm3Ca+fMudm7GnqSQauxyRJoFRzaTEgee3j6e5D3H34LYnuC/z6G7ckT16b4IjX+hNNETYCuRw1ilIpQeWBLYbLeIhALG9KIlHnHI2n0xAD9zLkVEQ4eVv6KwWW0IeYBY6Q6iwk4XW4vxoH2GBB5AcXzuOd9RX8YGsd186v4tJiF8vMqyglKQ4OMRscUWUzx3h/S9Wp3J9mEyu41Eoy1XeiaLgvRSTsiJiJ94Tn8drBE94ZhZlNiYlq307yhDkzc9Z+ZysnxLEtMktLiFmR2fipMJyh13uOTlzD6lwbc02Sz7RAkVLlTVVtSUNe+nysUugc6TnDqeVx0nxK9WOzYKIexVRMasujeiGp9nt9NJME8yTU8XiCG18f4eatbTJxhI0LV6n0RMEMRYRHQq4FbZIMlSEV3l99+A2Gs9gUXnZcpUmao6E2hcetqjTTSYZ2wudSqhsS5o9/+BOEDXXNqXJGikZ5IpIqO26IsOqa9muK3YPHtkqBFlf94tef4OHTe7a4qrh4MOmRhGdUdRHv1SRbmp0mtSpLMhr9I6lpmhXllfKGoammtho+IIIWjsf60SlWylnNcZlIldElmpB5MsIC4/kGSU1rzf34zYu4trGGrbkWVviO2mT0MBvYSgVJORNKzHwRsRWMU04/tBqC2gA1tVsjYNys2vjkfZlj6FZooFNeuhh6eLwe8OPwzigcr7lyu/5ZtVrl+M/usftkFKXGKrJwx1I5RSPDmAZzImUWtdGj5ri3N8Bndx/jxvYO9vIZxlrzLm6iiCKMqQAnIsw4QqfVxuHeAQmVBp/GP9e4NRpcjRlTZxn1rqe0tLakoJigUYwQ0FC3wxo6SYD5MMN//vd/hjc3Wpibm+LocJdkwfg11/CLT5/iv/z5X+NZ2sKASnAcaNYYZ5wbVHdBEZnKyxiXKEzQbc5jcDDG+vIG/uN/+E+YTmArn6utz9TqrLDOJA2tp1ekOOwdoj/Yw//5+V9gSPXW7/cworpS3tWMXJk3qiOt2uVEkmUeW8bbvpRdbHGWhNL0ZoJpT8ZVA+81OFvUoqwwmUUCsnfCP03GKe7vk8BGJOoAW+trePfyJVw6v4Z5KraQeWdzWU6ZRq1OYI7p4HNqPxVxFZpyjVuh+hYUN7fVnuCuWEGluovhW1LKqx4erwu8wjujkLGq1h/Tvv3lf7O/OiEDZ648Z9V51XV1ZMiR1DWbPgmJRi+m0W1SCUoNtlotLC4sUAH2bIjCZJJa9WEcJ0ZmKY8Pjo4QtVtUQCE0+bHGjEWhhgeQHKg2ivHIZu/QcIc6CVUL1BSzBJNpglHBbV7DrZtf02AXmFtaQCNpoajrWoybD47w2dePMUTTdVpRlSGdkqVhBiIyEYmITOP5plSZGeO0ML+ED97/gFdcb1OpzDBuIG4FVI9Tm/br69s38OGnv6T7BYbZc6q4PSq6gbSb3UNWNhUo0hMrqOrS8lGhi+SMYDSSTev38RqPtcipzZZC5ar0qtdrMSHZc1/VltWyPIG1o6ZoUJElVGpvdRN8cH4Vf/Lu21R0l3BhoYMO30s0GdhKBW2pOJG61J3eEwmQqRLV2XvU1HFay9Apb8WlfOe847Q7UXX6J2VX7R9/PB4erwU84Z1RyE7ZFI7cysgZwWnfnAwfd+yqM4bOcHNL4tMs9upXqWmlajTO6tpvZpyGO6GSa5PwlhbmMT/XNSLU0kApCWw21VTUMp9US1RyBR9KaZCnNOCun+bUdYPnfWpPUqcJZ1pV7RdD/QbTWYzxNCbhzfD06T56VFpHwyFGJK2ku4RGcxW3Hvbwt59+iyKYK4cluLhb2NzRbCvmK5WQluQxM864X9jcMsLTCujqLakM6o96eLh9H9dvfoFPv/zItg+f3MPB4BkJo28rFhSqkOW9dRK2MoI0x3/KXIXoUuyqA6XfnNO5BglOeac5J6dqg2RcNOGyyE0L5WqlAi3HUx9rzbkRWsyTlVaMCyT4t5YX8C8ub+KDzXN4e+Mc1toJmiI2Kj5SPxUwlSzz1ZGcVJ16t6qgoniJkEm4dfWMdUM2TsjuJL5ubTzmldpaed7907G2ytPyA/LweE3gCe+MQnbK2Sr+laEz4+yOK8PmrrlzJ4rPnVPZP5jRyJtKIV3JcOdUDmp70jUqqo21Vcx1WmhpBhAa3Gw8thXVpViSWG1aNLwiFpJeXd3wafDrVDGqzov5jDqUqBnJdY4nFarNq6Z5Md0MJ3NzCQ76R7j74ACPnz1GmlO/1Nu4/2SEG3eeIWtQ79Coi1hF5IqzGyZXEp6WRFIVa9lOeWHzAjY3N63tcm9/D3fu3cK3d27iqxuf40u6e49vozc+5HM1dBYSZLMRPWV6G/SvdMobq8BUFaQRBk+IJI63bt/+lfkoJac2OXUG0ioFmtVEPSa1sGqLJDZPfjzXinBpeR7vkODef2MTP9gi0S13saDwJ0Nb2aCh9jySkKYKixi+5kuVlqxoTFA+iMhEeLnlpeJIKDK6q9qedva//Cd/ddbITo4HHh6vCXwb3llFaatO2yszhuUZK73zyFQdnR3ZObevKaqa3NGQBR2rR6Fm+VCFWabqwEaItBFhEsQ4oAePD/v49skO7tI9OehhL83QWF3F0BiNocrQkuxyTUxMMx1Fse1rTT/TeSS7WtXBg8dSLKHWjUv3UM+fqyYRCx3g/PoGiXQJd7cHOMgZB4sL76tpTCGfIUGHPC/MImBE9alOKknQwjuXr+HS1hWM+mPsPN3F7dt3rFpT1aYKoMaHp3USN0ko0wreMYmYJG1zhkr1MA1VZx0mydxJLle0IyfMSOhjKuIQLa0UQaKbkbi0UGpEP0V43aCOlU4TW8uL2KBb7nbQTSJbQFUqUEMKVMWpUkEUBOa0ooBmWck12FwTTPM+C+3U+9Se2uTUE/akDc/9LV+xiyn33ba8ascuFfTJvovjBzw8XgN4wjuj0Et3pktGrDJm3Ku25R1mIOnIE2Y0reOFoYZpRnVCQglpgN2YLRptKhUjP6qMMU9NowSTsIn+rI7n4wwPnh/g5oNHuL27jx0a2548puGNkxb9r9u6eeq/oeo+TW1ms5iQCJ2hVduai5tUX29INRQD880RavkBsv6IRr+BIDqHfkqybbRtSELe0DRkrnovEDkY4TGOUYGxps9Sr8xpgM3VC1SXEcaDFDvPdqDFczUAXL0yKRTRiEUSOSb5mPGcII6bJDv6RD+l6ER4qhpVz1YNychJ2A6n89WBuWekbYujMgzQPym0TlDDSivBUjPCtYubNs3XaruJRR5rjJyqe2daCUHPMF/UozJquEmkRXzq/GIdXxgH6wjDOLl3pxzU1qJi2/IKnVBRGfeqU0QVY9sqjbav/NeT5omHx2sDT3hnFKp4rNrlZOCqHpjO6Vh3lYaN94joXE9OHZMsaKon05D2lATDI1IDnVQUFZo6nsgchg1M+MCQxCYn4huS+HaO+rjfG+FX9x7j4eEQvdEE9biNoNWhnzWkssNUc1plQO1+szylEc8R1gpuGXOGkU0jDGvL1sV/rjlAJ5wA6RBqsCuKDg6HJJ32Ig9JUg2qIBKeOm6EBQmvEM1MkUf0h37FjYR8M8VSdxn9gyFJM0HvsIe1tTUbRjCcDDDOhyQ7EieJL4hd+1s6kSJyZOMG2TvSC1S9KaIhIdk5uoosHEhyPG5pfIdmq8kzJMzwZam5pQVcWJ63NrnLa0tIqCRjpT8b0Y1tkL+qhGskummY8J2I2JgaFhSmGcmO7yOJE8RhjNFozLBKxWnv271zsZTaTNXr1Tqw6BRRxVDvuIK7xr8Ks0yjc7pJbZGnbvbw+CcO34Z3RmFGULZKhsw2Tn9U+0J1XBnA6rz2pMbi1hw0SNpkDhWHxnRJNTjzOsNkPHZqR49Qfag9SQOc5ynLFrodtFptGvspldmACm1KQtPMK6om5NP0UtWZIo0ZlZCax4IG/bBGw0yLqaMWLZCQuDNNTQmOqSBzLefT6NggdNGjS6PUFMmABruhXpozxYRB8w4t5aNeoJmqWCnj1LO0RYWlQfc7u0+Zmsx6aTbbMRqa0oSkW0wzG1ivCae1aKwtv8N00iOGJyot2ybFhAxbeaIOIxY+Q9YCq7GGCwx6mGdaNtotvLe1gZ+9fRU/efMS3jq3gg2SX61/aGSXkBBtiAGfs4VZLX8ZFpVdmhXkSxIx/yWRxkuGVJYFyU6r0bs7GQnbuvZX9w5tMVX6JToU3OmTN3yyQ9i+0lLeZ8f6ozdrBx4erwW8wjuj0Et3xtDh2I45u2a22sCDate25U2O1lyXCO1ZD0Aa0YrwqqdkZM3ka0uStKvc13p7xcIqbm7v4PrNu7j1eAe7oxzjeoKUymVC8pE+maq6NCahkOzybIiMrk7Si+MO0rTFAEhwJCUpFqkVF0GZcdc+5eLs4iQj79Sr4gFkDZIpE2pPkBnFpWbCLR3uvFKqYRgaS6ehBm6mFV5Tz8W0TjUYk1w1OwwJl+SUkZQ0Fk/j9tSOplXgqYOt7bBBIqrTNbjf5r1XqBSvbZzDW1cuY3VxwdrxpuORqeUm1V+dRCdlK2JyhQlV7bq0KA8dobsXVFGV/a1KKOU5bd3bIOx2987UkejUFTtvqB4r4XT+b5wmfvOMh8c/ZXjC83glUMeWfS3mmrQxKuq48+Q5Pr/9ELefHaCvAQqtLnIqlsN0wus0+kmAqBnSlhe2akKRaXxam0Zb3WZk46vPWNsXP2mZZXfZ0YI6YahqryD7OfJypv6Y6Mqtgdcd6bl7rVpXp/lvngp3PByjPxqSfKj4kogqUNV8vKfI0Gb8Z1S508EIQZqjS5Jfm1vA5soazjdjvLfUwjLv77bbVIok90lK8ZpS6aoTSt16rZbFBecnydbSxuuKQRlrOg8Pj5eBJzyPVwLN8j+ROtEsLFRJvQx48Fw9OZ/j9s4hng4m2CdJTBpUTyGJLgqR0+CnJAFVHarqDhOREw2/EUBJc7b/4ietU0YL5dbpG5IXVVt152nacJQmlLRHD9z2xGeRjfV0VC9OKrOQBJVE6iVJP1OS3FDD3meYI7Evk/jOtdo4353HxsIi1haWrBPKUpvkmGmoRoaZJr+WPwxD5EfvSJrfJTyFTmdbT3geHr8rPOF5vBKIQuJWgt54hCHVGpIOaq15ktwMN5/s4dbTPdzQ8kC1Bkb1ACmJb6R2Oo3zCwObtWU6nlg1pWrw5F9Vk6f9CiVlOWooTzu+sCfKrbtL1YNuKz/sCTqh2up2t6/NKJsgIhHHYcOqH4vxEAG3HZJVt1HHfL2OzW4HV9fWcHllBetUcm1Vcao35TTDKKdS5bbGY409jDStmua3ZBrVJuq6m5Sqju6E8NwZF6+TuHl4ePzD8ITn8UpghryYQBM4FyQ0Wy2hEWOECP1ZYNWaNx4+xTdPdvH142c4zGn8210b5jAmQaRZikTVfvTHqhvppxGVkcHpT7qiLp7jf9svt3Zn+axrY5Rz+8eEN1MYZRsfnT1rz5OkIl6bFU7RjUh2jNNKM8bl1WVszXfx5toqFinXlkiIWni1qU4nVKh1KjeNWRwo7jyvjj02hZrio046GopBxReR2C3SpVPaqji7FNgJDw+Pl4QnPI9XAo1BQzZCrGrAMEJGaz4kqU3QQBG1MW3O4SCv4ZvtXXxJpXfv+RH2qP4GJCDNnqIhD7NZ5giLdl86SGSl/YrEKlT0VQ0NcEMuKqhdriI854f2HRzJaZFazdAScGsdW+hPg+Q1nZDkNKyB9y+3m9hcXMAlkt2FxTksxYENLYjzCYlwjLoWWSXBB6q2lIpTD0s6rZZgg+5JciK6mSk7F74NZC9TohQ5wnPpc6ji6eHh8TLwhOfxSqC2rljsoTYwM+fUN/WGTWg8mTUw5jaL26b0dsZT3H62j+v3n+Duzh5SEl7Y6aKn9i9xxbHTlFluvyJB4TcJrzwur1T3F0aeRi3lfSI5Kjl6pGrIkByt6dTUyzJivDXTyWq7hc3VVVw6v46tlSUstxI0QVIrUhTjHqJagaiu/pR8mARtPS3psebTnE5J2gzcBq1bbDSwXv1EHdzYPl1RjFwchWrr4eHxu8ETnscrgXpCaoYRzdAigaPZSjTmzqoTNX4sjDEgGeQkvZSKb4+k983jHVy/+wAPdw9wmBdImy2qJHXNd51gChKecxXpVfqoorbSVSfLFc2dSqzIznX/l4KTC6m64mJKgpMj6fFYJJiQ8H54YRNvLC/hjfPnsdRpu4VxJxocnpHMReG5jRuskfBs/J4NXihUS0q6qyOahuRAhkg/bVkkpYX5kmUZJmlq7ZSOCi2a3Hc4Oed/uh4evws84Xm8Ihgj2Z7UnkbGuXXa+DnytNGOqv2o5oYkpqEmjo47OEqn+Jqk9znd7f4E4zAhe0gZ1pDy0YzbKY8RhshIiiI+hxPCU/UhQyURRahTSVJY8jypiKoM6kQyS0lsOdqNurXLBalmO8kwFwTYWCLBrZ/H5sIcrqwskgw1Tk4qUM6RpJylhSTnxu3RbxJgQfKTilScVEXazFw1qYudUO7bxmJqMa7OO5y+n/Ck5+Hx0vCE5/FK4ChOc1qqncxV+dm0ZCQKkY/dY+QVYsJ7R+o80uwAUYKDwRj3D4f4m7vb2B6m2DvqYZiRXDTujW5CAhpqTk6qvTpJKohCciKfF1fQf5u9hUFMi7opKk1fptXc6zbN2MwWmW1xO9h5gqUkwvm5DtbNdXFufg7nFhawoBXex30jRlV9OtJzRFYn+VkHFAVXEpwR3qmtJrtuZk4tnhCc1GZFZo7wtHWuwsm+XfeE5+Hx0vCE5/FKIIrLaxENfN0Uka1/J+IrnUhPH6YWiNVoNBHYLCBxaZJpKr+DWYBHeYSbz/bxzZ17eLizi0ExQx5EKDTPJf2dqTMMVVdOgtMUYprYWnNPW69ItRdSLYb0Kw7rNti7ToVXz8Y2b2VUTPDuhQ2sdhJsLs4Z4S2S/JoaA6h7qd6yfMgYKu70k6xWbUV8rkenI6eKyFTtKvLTvtoRtQK5CN6R3IuE5x7VH9sxXrM9d8Fg4tATnofHS8MTnscrgQhPi7NKC8non5Ad93lVU2kVeWbqDCSoSa6ZLwn1bCRxDEh49dULeDrM8Hh3D3e2n+Le01087Q3Q540TVVUmTTfpGMnFCI+Pi/Dq/KMVBbpNXh+PkQ4HNolzO6hRwXVxYWUBa50W3n1jA50GbAUDLdcjQtSqBjONuZtlqDedjnNDFirSo+O2GsZQDWsQXSn8itR0ZD1V5YNu0x7T5bbuPoeypyb/qzvLCYmW93nC8/B4aXjC83glUJVmTpWlCkyb45Im3LYkJu07wsuNnGTp9ZFqhYA6nRab1Vi8gXo5xi1bNeAoLXD/+T5uPnyCW092jPiGakaLY4QkviCKza+qUwjlHabjgS2Y2uG1hVYLG0sLuLy+hsvn1rDWbSPvH9m4OatqnZLgRNO8XxNZK04TnnOE4wivUnXf3RdlVbx0mp+MsMqtKb9T5/iIAw+0a518bCvnSLNavcLDw+Pl4AnP45VArXRTIzJHBs6on2yFOIqQU03Jiag0IbNWJdC+ejZqnJqWEsrqAYqoiXEjxrPBGLef7eHh/hFuPdrGiNdHmdawy22dPq0TF4WhDYlozsY4T5K7srmFy+c3sNrtoK0q1skEjVTLCU0RWPWqq2LVcAJFjrdYe+CUpGuMYxEumadMQ5my8tjpNZc+kbmuG8Xzcddi6QjMbrcbbatd3aXnFHduK9LTdVvJ3QXm4eHxEvCE5/GKoLYrV6XnzLiIzMx5eVwjMcUoNFSBpKP5KsGt2uGM/Ki61rotjIdDHE0yW2EB7XmkUQuHswYOyIS3tp9ifzjG88Mj7Pf6yIrMVlLvUM114zo+uHQO3YCPhRHaJM0m2SPMp4jogmKGJNDgAVWFSkmps8kUOeOQ0h9kU8wHbau+dErLdU6ZkpWOx/Pxfgel0TkRmKDVFmpTddpxwypOFJ5CsxuM5CrCc+rXTlvu6J7cE56Hx+8ET3gerwgig9xZ8GOyc6qnIrzxeIIg0LyZkW2n09za9UR4EcmkNjyyFb/VOSVvhBiRPMb1EJMgRk4n5aeKSE3VmfMZKTypQnVaUVeYMDuytrmo1kBE5tDyPY2MjoSnweVTHotwFLU6yQ90GrpH2kWD/Ks1ZzWswIiO6RDZaauemBUJGn1ZGk87naqjboSnNDviqrYv3MetI7pyq3O6xB319vSE5+Hx8vCE5/EKUSkgZ7WlZ6r9k22F05+pjL+cWgBFLKoWdIPO3RRhbl/+uWnCSl3FXeMK/tEZa5vjk+6Y1MOtIxd3bMTinvwOIblr6qCibXXOXTeKsxtsayj37D494A5qInbtl35XqPZLbwmXM7pwcq687/QJDw+PfxCe8Dw8PDw8zgRUd+Th4eHh4fHPHp7wPDw8PDzOBDzheXh4eHicCXjC8/Dw8PA4E/CE5+Hh4eFxJuAJz8PDw8PjTMATnoeHh4fHmYAnPA8PDw+PMwFPeB4eHh4eZwKe8Dw8PDw8zgQ84Xl4eHh4nAl4wvPw8PDwOBPwhOfh4eHhcSbgCc/Dw8PD40zAE56Hh4eHx5mAJzwPDw8PjzMBT3geHh4eHmcCnvA8PDw8PM4EPOF5eHh4eJwJeMLz8PDw8DgT8ITn4eHh4XEm4AnPw8PDw+NMwBOeh4eHh8eZgCc8Dw8PD48zAU94Hh4eHh5nAp7wPDw8PDzOBDzheXh4eHicCXjC8/Dw8PA4E/CE5+Hh4eFxJuAJz8PDw8PjTMATnoeHh4fHmYAnPA8PDw+PMwFPeB4eHh4eZwKe8Dw8PDw8zgQ84Xl4eHh4nAl4wvPw8PDwOBPwhOfh4eHhcSbgCc/Dw8PD40zAE56Hh4eHx5mAJzwPDw8PjzMBT3geHh4eHmcCnvA8PDw8PM4EPOF5eHh4eJwJeMLz8PDw8DgDAP4vtkUJztM6cdAAAAAASUVORK5CYII=";
            IFormFile teste1 = ConverterBase64ParaIFormFile(base64);
            byte[] teste2 = await ConverterIFormFileParaBytes(teste1);
            IFormFile teste3 = ConverterBytesParaIFormFile(teste2);
            string teste4 = ConverterIFormFileParaBase64(teste3);

            return Ok(teste4);
        }
        #endregion
    }
}