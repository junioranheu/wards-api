﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
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
using Wards.Infrastructure.UnitOfWork.Generic;
using Wards.Utils.Entities.Output;
using static Wards.Utils.Fixtures.Convert;
using static Wards.Utils.Fixtures.Get;
using static Wards.Utils.Fixtures.Post;

/// <summary>
/// Sim, este Controller está propositalmente uma bagunça;
/// NÃO LEVE EM CONSIDERAÇÃO ESSE CONTROLLER EM GERAL COMO PARÂMETRO DE QUALIDADE: ELE ESTÁ RUIM PROPOSITALMENTE;
/// </summary>
namespace Wards.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExemplosController : BaseController<ExemplosController>
    {
        #region constructor
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
            IWebHostEnvironment webHostEnvironment,
            Infrastructure.Factory.ConnectionFactory.IConnectionFactory connectionFactory,
            IListarLogUseCase listarLogUseCase,
            IListarUsuarioUseCase listarUsuarioUseCase,
            IMigrateDatabaseService migrateDatabaseService,
            IBulkCopyCriarWardUseCase bulkCopyCriarWardUseCase,
            IGenericRepository<Usuario> genericUsuarioRepository)
        {
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
            _rabbitMQChannel.Close();
            _rabbitMQConnection.Close();
        }
        #endregion

        #region IFormFile
        [HttpGet("iFormFile")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IFormFile))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(StatusCodes))]
        public  ActionResult<IFormFile> ExemploIFormFile()
        {
            string filePath = $"{_webHostEnvironment.ContentRootPath}/Uploads/Usuarios/Perfil/Imagem/1AAAAA.jpg";
            string fileName = Path.GetFileName(filePath);
            FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read);

            return File(fileStream, "application/octet-stream", fileName);
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
        #endregion
    }
}