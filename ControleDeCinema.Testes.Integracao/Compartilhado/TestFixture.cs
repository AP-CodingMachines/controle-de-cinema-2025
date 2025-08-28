using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloSessao;
using DotNet.Testcontainers.Containers;
using FizzWare.NBuilder;
using Testcontainers.PostgreSql;

namespace ControleDeCinema.Testes.Integracao;

[TestClass]
public abstract class TestFixture
{
    protected ControleDeCinemaDbContext? dbContext;

    protected RepositorioGeneroFilmeEmOrm? repositorioGeneroFilme;
    protected RepositorioSessaoEmOrm? repositorioSessao;

    private static IDatabaseContainer? container;

    [AssemblyInitialize]
    public static async Task Setup(TestContext _)
    {
        container = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithName("controle-de-cinema-testdb")
            .WithDatabase("ControleDeCinemaTestDb")
            .WithUsername("postgres")
            .WithPassword("root")
            .WithCleanUp(true)
            .Build();

        await InicializarBancoDadosAsync(container);
    }

    [AssemblyCleanup]
    public static async Task Teardown()
    {
        await EncerrarBancoDadosAsync();
    }

    [TestInitialize]
    public void ConfigurarTestes()
    {
        if (container is null)
            throw new ArgumentNullException("O banco de dados n�o foi inicializado.");

        dbContext = ControleDeCinemaDbContextFactory.CriarDbContext(container.GetConnectionString());

        ConfigurarTabelas(dbContext);

        repositorioGeneroFilme = new RepositorioGeneroFilmeEmOrm(dbContext);
        repositorioSessao = new RepositorioSessaoEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<GeneroFilme>(repositorioGeneroFilme.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<GeneroFilme>>(repositorioGeneroFilme.CadastrarEntidades);
        BuilderSetup.SetCreatePersistenceMethod<Sessao>(repositorioSessao.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<Sessao>>(repositorioSessao.CadastrarEntidades);
    }

    private static void ConfigurarTabelas(ControleDeCinemaDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();

        dbContext.GenerosFilme.RemoveRange(dbContext.GenerosFilme);
        dbContext.Sessoes.RemoveRange(dbContext.Sessoes);

        dbContext.SaveChanges();
    }

    private static async Task InicializarBancoDadosAsync(IDatabaseContainer container)
    {
        await container.StartAsync();
    }

    private static async Task EncerrarBancoDadosAsync()
    {
        if (container is null)
            throw new ArgumentNullException("O Banco de dados n�o foi inicializado.");

        await container.StopAsync();
        await container.DisposeAsync();
    }
}
