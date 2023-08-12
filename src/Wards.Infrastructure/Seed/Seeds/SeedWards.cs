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
                await context.Wards.AddAsync(new Ward()
                {
                    WardId = 1,
                    Titulo = "Como utilizar Response Compression no .NET",
                    Conteudo = @"<section>
<h1>Utilizando Response Compression no .NET</h1>

<br/>
<h2>Introdução</h2>
<p>A compactação de resposta (Response Compression) é uma técnica importante para melhorar o desempenho e a eficiência de um aplicativo da web. Ao comprimir os dados antes de enviá-los ao cliente, você pode reduzir o tamanho das respostas HTTP, resultando em tempos de carregamento mais rápidos e economia de largura de banda. O ASP.NET oferece uma maneira fácil de habilitar a compactação de resposta para o seu aplicativo.</p>

<br/>
<h2>Passo 1: Adicionar o Pacote NuGet</h2>
<p>Para começar, adicione o pacote NuGet <b>Microsoft.AspNetCore.ResponseCompression</b> ao seu projeto ASP.NET. Isso fornecerá as classes e funcionalidades necessárias para habilitar a compactação de resposta.</p>

<br/>
<h2>Passo 2: Configurar a Compactação</h2>
<p>No arquivo <b>Startup.cs</b>, configure a compactação de resposta no método <b>ConfigureServices</b>:</p>

<code>using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

        // ...
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseResponseCompression();
        // ...
    }
}</code>

<br/>
<h2>Passo 3: Testar a Compactação</h2>
<p>Agora, quando você executa o aplicativo, a compactação de resposta será aplicada automaticamente às respostas HTTP. Você pode verificar isso usando as ferramentas de desenvolvedor do navegador, que mostrarão o cabeçalho <b>Content-Encoding: br</b> para Brotli ou <b>Content-Encoding: gzip</b> para Gzip nas respostas compactadas.</p>

<br/>
<h2>Observações Importantes</h2>
<p>A compactação de resposta geralmente é mais eficaz para conteúdo de texto, como HTML, CSS e JavaScript. Para arquivos binários, como imagens ou vídeos, a compactação pode não resultar em uma economia significativa.</p>

<br/>
<h2>Conclusão</h2>
<p>A utilização da compactação de resposta no .NET é uma estratégia eficaz para otimizar o desempenho do seu aplicativo da web. Ao seguir os passos acima e configurar a compactação de acordo com suas preferências, você pode habilitar facilmente a compactação de resposta, proporcionando uma experiência mais rápida e eficiente para os usuários.</p>
</section>",
                    QtdCurtidas = 0,
                    UsuarioId = 1
                });

                await context.Wards.AddAsync(new Ward()
                {
                    WardId = 2,
                    Titulo = "Como checar o environment em .NET",
                    Conteudo = @"<section>
<h1>Verificando o ambiente em .NET: Debug ou produção</h1>

<br/>
<h2>Introdução</h2>
<p>Ao desenvolver aplicativos em .NET, é comum a necessidade de executar ações específicas com base no ambiente em que o aplicativo está sendo executado. Uma maneira prática de realizar essa verificação é usando o método <b>IsProduction()</b> fornecido pela classe <b>IWebHostEnvironment</b> do ASP.NET Core.</p>

<br/>
<h2>Passo 1: Usar o método IsProduction()</h2>
<p>No ASP.NET Core, você pode acessar o ambiente atual do aplicativo usando a injeção de dependência. Primeiro, adicione uma dependência para <b>IWebHostEnvironment</b> ao construtor da sua classe:</p>

<code>private readonly IWebHostEnvironment _environment;

public MinhaClasse(IWebHostEnvironment environment)
{
    _environment = environment;
}</code>

<br/>
<p>Em seguida, você pode usar o método <b>IsProduction()</b> para verificar se o aplicativo está sendo executado em ambiente de Produção:</p>

<code>if (_environment.IsProduction())
{
    Console.WriteLine(""O aplicativo está sendo executado em ambiente de Produção."");
}
else
{
    Console.WriteLine(""O aplicativo não está sendo executado em ambiente de Produção."");
}</code>

<br/>
<h2>Conclusão</h2>
<p>O uso do método <b>IsProduction()</b> da classe <b>IWebHostEnvironment</b> no ASP.NET Core é uma maneira conveniente de verificar se o aplicativo está sendo executado em ambiente de Produção. Ao utilizar essa abordagem, você pode ajustar o comportamento do aplicativo de acordo com o ambiente atual, garantindo uma experiência otimizada tanto para o desenvolvimento quanto para a produção.</p>
</section>",
                    QtdCurtidas = 0,
                    UsuarioId = 1
                });

                await context.Wards.AddAsync(new Ward()
                {
                    WardId = 3,
                    Titulo = "Remover referências não utilizadas",
                    Conteudo = @"<section>
<h1>Remover referências não utilizadas no VS 2022</h1>

<br/>
<h2>Introdução</h2>
<p>Ao desenvolver projetos no Visual Studio 2022, é comum acumular referências a bibliotecas e assemblies que, com o tempo, podem se tornar não utilizadas. Isso pode afetar negativamente o desempenho e a organização do seu projeto. Felizmente, o Visual Studio oferece uma maneira fácil de identificar e remover referências não utilizadas de forma eficiente.</p>

<br/>
<h2>Passo 1: Abrir a Opção de Remover Referências Não Utilizadas</h2>
<p>Para começar, clique com o botão direito em qualquer <b>solution</b> do seu projeto no Visual Studio 2022. No menu de contexto que aparece, clique na opção <b>Remover Referências Não Usadas...</b></p>

<br/>
<h2>Passo 2: Revisar e Confirmar</h2>
<p>Uma janela pop-up será exibida, mostrando uma lista de referências que podem ser removidas por estarem sem uso. Revise cuidadosamente essa lista para garantir que as referências a serem removidas são realmente não utilizadas. Depois de revisar, clique no botão de confirmação para remover as referências selecionadas.</p>

<br/>
<h2>Passo 3: Compilar e Testar</h2>
<p>Após a remoção das referências não utilizadas, é importante compilar e testar o seu projeto para garantir que tudo continue funcionando corretamente. Certifique-se de verificar se não há erros ou problemas de compilação.</p>

<br/>
<h2>Observações Importantes</h2>
<p>Embora a remoção de referências não utilizadas seja uma prática benéfica, é importante ter cautela. Algumas referências podem ser usadas em partes do código que não foram analisadas. Sempre faça uma revisão completa do seu projeto após remover referências não utilizadas.</p>

<br/>
<h2>Conclusão</h2>
<p>A opção de <b>Remover Referências Não Usadas</b> no Visual Studio 2022 é uma ferramenta valiosa para manter a organização e otimização do seu projeto. Usando essa opção, você pode facilmente identificar e eliminar referências desnecessárias, contribuindo para um código mais limpo e uma experiência de desenvolvimento aprimorada.</p>
</section>",
                    QtdCurtidas = 0,
                    UsuarioId = 1
                });

                await context.Wards.AddAsync(new Ward()
                {
                    WardId = 4,
                    Titulo = "Certificação SSL para ambiente localhost",
                    Conteudo = @"<section>
<h1>Certificado SSL para ambiente localhost no Visual Studio</h1>

<br/>
<h2>Introdução</h2>
<p>Ao desenvolver aplicativos da web no Visual Studio, é comum precisar de um ambiente seguro usando HTTPS, mesmo em um ambiente localhost. Isso garante que você possa testar recursos que requerem conexões seguras, como autenticação ou acesso a APIs, sem problemas. Aqui estão os passos para instalar um certificado SSL para uso em um ambiente localhost no Visual Studio.</p>

<br/>
<h2>Passo 1: Geração do Certificado</h2>
<p>O primeiro passo é gerar um certificado SSL autoassinado. Você pode fazer isso usando a ferramenta <b>openssl</b> ou uma ferramenta como o <b>dotnet dev-certs</b>. Abra um terminal e execute o seguinte comando:</p>
<code>dotnet dev-certs https --trust</code>

<br/>
<h2>Passo 2: Configuração no Visual Studio</h2>
<p>Abra seu projeto no Visual Studio e navegue até as configurações do projeto. No menu suspenso do projeto, clique com o botão direito em seu projeto e selecione ""Propriedades"". Na guia ""Debug"", marque a opção ""Enable SSL"" e selecione o certificado que você gerou anteriormente.</p>

<br/>
<h2>Passo 3: Atualização da URL</h2>
<p>Agora, atualize a URL do projeto para usar HTTPS. No arquivo <b>launchSettings.json</b>, altere a URL para começar com ""https://localhost"". Por exemplo:</p>
<code>""applicationUrl"": ""https://localhost:5001""</code>

<br/>
<h2>Passo 4: Execução do Projeto</h2>
<p>Agora, ao executar seu projeto, ele usará o certificado SSL configurado para o ambiente localhost. Isso permitirá que você acesse sua aplicação via HTTPS.</p>

<br/>
<h2>Observações Importantes</h2>
<p>Lembre-se de que o certificado SSL autoassinado não é confiável em um ambiente de produção. Ele é adequado apenas para ambiente de desenvolvimento. Além disso, ao acessar sua aplicação, seu navegador pode exibir um aviso de segurança devido à natureza do certificado autoassinado.</p>

<br/>
<h2>Conclusão</h2>
<p>Instalar um certificado SSL para ambiente localhost no Visual Studio é fundamental para testar recursos seguros durante o desenvolvimento de aplicativos da web. Seguindo esses passos, você pode configurar seu ambiente de desenvolvimento para usar HTTPS, permitindo testes mais realistas e precisos.</p>
</section>",
                    QtdCurtidas = 0,
                    UsuarioId = 1
                });

                await context.Wards.AddAsync(new Ward()
                {
                    WardId = 5,
                    Titulo = "Migrations",
                    Conteudo = @"<section>
<h1>Entity Framework Core Migration no .NET 7</h1>

<br/>
<h2>Introdução</h2>
<p>O Entity Framework Core Migration é uma ferramenta poderosa para gerenciar as mudanças no esquema do banco de dados em uma aplicação .NET. Com os comandos <b>add-migration</b> e <b>update-database</b>, você pode criar e aplicar migrações para manter o esquema do banco de dados sincronizado com o seu modelo de dados.</p>

<br/>
<h2>Passo 1: Instalação do Pacote</h2>
<p>Antes de começar, certifique-se de que você tem o Entity Framework Core instalado no seu projeto .NET 7. Se não estiver instalado, você pode adicionar o pacote NuGet através do seguinte comando:</p>
<blockquote>
    <p><i>dotnet add package Microsoft.EntityFrameworkCore.Design</i></p>
</blockquote>

<br/>
<h2>Passo 2: Criar uma Migração</h2>
<p>Para criar uma nova migração, você deve usar o comando <b>add-migration</b>. Abra um terminal na pasta do seu projeto e execute o seguinte comando:</p>
<code>Add-Migration NomeDaMigracao</code>
<br/>
<p>Substitua <i>NomeDaMigracao</i> pelo nome descritivo da sua migração.</p>

<br/>
<h2>Passo 3: Aplicar Migrações</h2>
<p>Depois de criar uma ou mais migrações, você pode aplicá-las ao banco de dados usando o comando <b>update-database</b>:</p>
<code>Update-Database</code>
<br/>
<p>Isso aplicará todas as migrações pendentes no banco de dados e atualizará o esquema de acordo com as alterações feitas no seu modelo.</p>

<br/>
<h2>Conclusão</h2>
<p>O Entity Framework Core Migration é uma ferramenta essencial para gerenciar o esquema do banco de dados em uma aplicação .NET. Usando os comandos <b>add-migration</b> e <b>update-database</b>, você pode criar e aplicar migrações de forma eficiente, mantendo o banco de dados sincronizado com as alterações no seu modelo de dados.</p>
</section>",
                    QtdCurtidas = 0,
                    UsuarioId = 1
                });

                await context.Wards.AddAsync(new Ward()
                {
                    WardId = 6,
                    Titulo = "AddDbContext vs AddDbContextPool",
                    Conteudo = @"<section>
<h1>AddDbContext vs AddDbContextPool no .NET 7</h1>

<br/>
<h2>Introdução</h2>
<p>Ao trabalhar com o Entity Framework Core no .NET 7, você frequentemente precisa configurar o contexto do banco de dados em sua aplicação. Duas opções comuns são o uso dos métodos <b>AddDbContext</b> e <b>AddDbContextPool</b>. Ambos são usados para registrar um contexto do Entity Framework Core no contêiner de injeção de dependência, mas eles têm algumas diferenças importantes que você deve considerar ao escolher a abordagem adequada para o seu cenário.</p>

<br/>
<h2>AddDbContext</h2>
<p>O método <b>AddDbContext</b> é usado para registrar um contexto do Entity Framework Core no contêiner de injeção de dependência. Ele cria uma nova instância do contexto para cada solicitação de serviço, o que significa que cada solicitação obtém seu próprio contexto exclusivo. Isso é adequado para a maioria dos cenários, especialmente quando a aplicação tem uma carga moderada de solicitações e o contexto não mantém muitos recursos em memória.</p>

<br/>
<h2>AddDbContextPool</h2>
<p>O método <b>AddDbContextPool</b> também é usado para registrar um contexto do Entity Framework Core no contêiner de injeção de dependência. No entanto, ele cria um pool de contextos reutilizáveis, em vez de criar uma nova instância para cada solicitação. Isso é particularmente útil em cenários de alta carga, onde criar um novo contexto para cada solicitação pode ser caro em termos de recursos. Com o <b>AddDbContextPool</b>, os contextos são emprestados do pool e devolvidos após o uso, reduzindo a sobrecarga de criação e destruição de objetos.</p>

<br/>
<h2>Escolhendo entre AddDbContext e AddDbContextPool</h2>
<p>A escolha entre <b>AddDbContext</b> e <b>AddDbContextPool</b> depende das necessidades específicas da sua aplicação. Se sua aplicação tiver uma carga moderada e recursos não forem uma preocupação crítica, <b>AddDbContext</b> pode ser a escolha adequada. Por outro lado, se você espera uma alta carga de solicitações e deseja otimizar o uso de recursos, <b>AddDbContextPool</b> pode ser a melhor opção.</p>

<br/>
<h2>Conclusão</h2>
<p>Ao configurar o contexto do Entity Framework Core no .NET 7, a escolha entre <b>AddDbContext</b> e <b>AddDbContextPool</b> é crucial para o desempenho e eficiência da sua aplicação. Considere cuidadosamente as demandas da sua aplicação e escolha a opção que melhor atenda às suas necessidades.</p>
</section>",
                    QtdCurtidas = 0,
                    UsuarioId = 1
                });

                await context.Wards.AddAsync(new Ward()
                {
                    WardId = 7,
                    Titulo = "Biblioteca react-hot-toast",
                    Conteudo = @"<section>
<h1>Utilizando a biblioteca react-hot-toast</h1>

<br/>
<h2>Introdução</h2>
<p>O react-hot-toast é uma biblioteca leve e simples que permite exibir notificações elegantes e interativas em aplicativos React. Com o react-hot-toast, você pode criar facilmente avisos, mensagens de sucesso, erros e muito mais, fornecendo uma experiência amigável para o usuário.</p>

<br/>
<h2>Passo 1: Instalar o pacote</h2>
<p>Para começar a usar o react-hot-toast, primeiro você precisa instalar o pacote em seu projeto. Use o seguinte comando para adicionar o react-hot-toast:</p>
<code>npm install react-hot-toast</code>

<br/>
<h2>Passo 2: Criar um Componente para Avisos</h2>
<p>Você pode criar um componente personalizado para gerar e exibir avisos usando o react-hot-toast. Aqui está um exemplo de como criar um componente chamado <b>Aviso</b>:</p>
<code>import { toast } from 'react-hot-toast';

export const Aviso = {
    toast(texto: string, ms: number, icone: string | null, isDark: boolean) {
        const styleGlass = {
            background: 'rgba(255, 255, 255, 0.08)',
            borderRadius: '1rem',
            boxShadow: '0 4px 30px rgba(0, 0, 0, 0.1)',
            backdropFilter: 'blur(5px)',
            color: 'var(--branco)',
            userSelect: 'none'
        } as any;

        const styleDark = {
            background: 'rgb(59, 59, 59)',
            borderRadius: '1rem',
            boxShadow: '0 4px 30px rgba(0, 0, 0, 0.25)',
            color: 'var(--branco)',
            userSelect: 'none'
        } as any;

        const style = isDark ? styleDark : styleGlass;

        toast(
            (t) => (
                <span onClick={() => toast.dismiss(t.id)}>
                    {texto}
                </span>
            ),
            {
                duration: ms,
                position: 'top-center',
                icon: icone,
                style: style
            }
        );
    }
}
</code>

<br/>
<h2>Passo 3: Usar o Componente de Avisos</h2>
<p>Agora você pode usar o componente <b>Aviso</b> para exibir avisos em seu aplicativo. Basta chamar a função <b>toast</b> e passar os parâmetros desejados para personalizar a notificação. Por exemplo:</p>

<code>
import { Aviso } from './caminho-para-o-componente/Aviso';

// ...

Aviso.toast('Esta é uma mensagem de aviso', 3000, '🔔', true);</code>

<br/>
<h2>Conclusão</h2>
<p>O react-hot-toast é uma ferramenta útil para adicionar notificações visuais atraentes ao seu aplicativo React. Ao seguir os passos acima e criar um componente personalizado de aviso, você pode facilmente implementar mensagens de aviso interativas em seu aplicativo, melhorando a experiência do usuário.</p>
</section>",
                    QtdCurtidas = 0,
                    UsuarioId = 1
                });

                //for (int i = 0; i < 22; i++)
                //{
                //    await context.Wards.AddAsync(new Ward()
                //    {
                //        WardId = 8 + i,
                //        Titulo = GerarLoremIpsum(3, 10, 2, 4, 1, false),
                //        Conteudo = GerarLoremIpsum(5, 15, 2, 5, 2, true),
                //        QtdCurtidas = GerarNumeroAleatorio(0, 500),
                //        UsuarioId = 1
                //    });
                //}
            }
            #endregion
        }
    }
}