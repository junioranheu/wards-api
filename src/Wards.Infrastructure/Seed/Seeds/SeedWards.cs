using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Infrastructure.Seed.Seeds
{
    public sealed class SeedWards
    {
        public static async Task Seed(WardsContext context)
        {
            #region seed_wards
            if (!await context.Wards.AnyAsync())
            {
                await context.Wards.AddAsync(new Ward() { WardId = 1, Titulo = "Como utilizar Response Compression no .NET", Conteudo = @"<p><strong>Compacta&ccedil;&atilde;o Gzip e Brotli</strong></p>

                <p>Quando algu&eacute;m entra em seu site, um pedido &eacute; enviado para o servidor para entregar o arquivo solicitado. Quanto maiores forem esses arquivos, mais tempo levar&aacute; para carregar. Ao comprimir suas p&aacute;ginas da web e CSS antes de envi&aacute;-los para o navegador voc&ecirc; ir&aacute; reduzir significativamente o tempo de carregamento como os arquivos ser&atilde;o menores. Essa &eacute; a fun&ccedil;&atilde;o do Gzip.</p>

                <p>&nbsp;</p>

                <p>Como implementar essa compacta&ccedil;&atilde;o no .NET 6/7:</p>

                <p><em>Sess&atilde;o app:</em></p>

                <p>app.UseResponseCompression();</p>

                <p>&nbsp;</p>

                <p><em>Sess&atilde;o builder:</em>&nbsp; &nbsp;</p>

                <pre>
                <code class=""language-cs"">builder.Services.AddResponseCompression(o =&gt;
                            {
                                o.EnableForHttps = true;
                                o.Providers.Add&lt;BrotliCompressionProvider&gt;();
                                o.Providers.Add&lt;GzipCompressionProvider&gt;();
                            });

                            builder.Services.Configure&lt;BrotliCompressionProviderOptions&gt;(o =&gt;
                            {
                                o.Level = CompressionLevel.Optimal;
                            });

                            builder.Services.Configure&lt;GzipCompressionProviderOptions&gt;(o =&gt;
                            {
                                o.Level = CompressionLevel.Optimal;
                            });</code></pre>

                <p>&nbsp;</p>
                ", QtdCurtidas = 0, UsuarioId = 1 });

                await context.Wards.AddAsync(new Ward() { WardId = 2, Titulo = "Como checar o environment em .NET", Conteudo = @"<p><strong>Checar se o ambiente &eacute; de produ&ccedil;&atilde;o</strong></p>

                <p>Como implementar essa valida&ccedil;&atilde;o no .NET 6/7:</p>

                <p>&nbsp;</p>

                <p><em>Program.cs</em></p>

                <pre>
                <code class=""language-cs"">app.Environment.IsProduction()</code></pre>

                <p>&nbsp;</p>

                <p><em>Em outros arquivos:</em></p>

                <pre>
                <code class=""language-cs"">#if DEBUG
                    return true;
                #else
                    return false;
                #endif</code></pre>

                <p>&nbsp;</p>
                ", QtdCurtidas = 0, UsuarioId = 1 });

                await context.Wards.AddAsync(new Ward() { WardId = 3, Titulo = "Remover referências não utilizadas", Conteudo = "Clique com o botão direito em qualquer <i>solution</i> de seu projeto no VS 2022, e clique na opção 'Remover Referências Não Usadas...'", QtdCurtidas = 0, UsuarioId = 1 });

                await context.Wards.AddAsync(new Ward() { WardId = 4, Titulo = "Instalar certificação SSL para ambiente localhost", Conteudo = "Abra o prompt de comando e digite <i>dotnet dev-certs https--trust</i>'", QtdCurtidas = 0, UsuarioId = 1 });

                await context.Wards.AddAsync(new Ward()
                {
                    WardId = 5,
                    Titulo = "Migrations",
                    Conteudo = @"Para utilizar migrations basta seguir os passos:
                <br/>1 - Instale o pacote <i>dotnet add package Microsoft.EntityFrameworkCore.Tools</i>;
                <br/>2 - Abra o prompt de comando no projeto e aponte para a camada de infraestrutura;
                <br/>3 - Crie uma migration: <i>Add-Migration NomeDaMigration</i>;
                <br/>4 - Para finalizar, rode o comando <i>Update-Database</i>.",
                    QtdCurtidas = 0,
                    UsuarioId = 1
                });

                //for (int i = 0; i < 20000; i++)
                //{
                //    await context.Wards.AddAsync(new Ward()
                //    {
                //        WardId = 6 + i,
                //        Titulo = "ABC",
                //        Conteudo = "teste",
                //        QtdCurtidas = 0,
                //        UsuarioId = 1
                //    });
                //}
            }
            #endregion
        }
    }
}