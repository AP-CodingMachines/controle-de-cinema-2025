using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidade;

[TestClass]
[TestCategory("Testes de Unidade de Gênero de Filmes")]
public sealed class GeneroFilmeAppServiceTests
{
    private Mock<IRepositorioGeneroFilme>? repositorioGeneroFilmeMock;
    private Mock<ITenantProvider>? tenantProviderMock;
    private Mock<IUnitOfWork>? unitOfWorkMock;
    private Mock<ILogger<GeneroFilmeAppService>>? loggerMock;

    private GeneroFilmeAppService? generoFilmeAppService;

    [TestInitialize]
    public void Setup()
    {
        repositorioGeneroFilmeMock = new Mock<IRepositorioGeneroFilme>();
        tenantProviderMock = new Mock<ITenantProvider>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<GeneroFilmeAppService>>();

        generoFilmeAppService = new GeneroFilmeAppService(
            tenantProviderMock.Object,
            repositorioGeneroFilmeMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object
        );
    }

    [TestMethod]
    public void Cadastrar_DeveRetornarOk_QuandoGeneroForvalido()
    {
        // Arrange
        var genero = new GeneroFilme("Terror");

        var generoTeste = new GeneroFilme("Teste");

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>() { generoTeste });

        // Act
        var resultado = generoFilmeAppService?.Cadastrar(genero);

        // Assert
        repositorioGeneroFilmeMock?.Verify(r => r.Cadastrar(genero), Times.Once);
        
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Cadastrar_DeveRetornarFalha_QuandoCampoObrigatorioForVazio()
    {
        // Arrange
        var genero = new GeneroFilme("");

        var generoTeste = new GeneroFilme("Terror");

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>() { generoTeste });

        // Act
        var resultado = generoFilmeAppService?.Cadastrar(genero);

        // Assert
        repositorioGeneroFilmeMock?.Verify(r => r.Cadastrar(genero), Times.Never);
        
        unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsFailed);
    }


    [TestMethod]
    public void Cadastrar_DeveRetornarFalha_QuandoGeneroForDuplicado()
    {
        // Arrange
        var genero = new GeneroFilme("Terror");

        var generoTeste = new GeneroFilme("Terror");

        repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>() { generoTeste });

        // Act
        var resultado = generoFilmeAppService?.Cadastrar(genero);

        // Assert
        repositorioGeneroFilmeMock?.Verify(r => r.Cadastrar(genero), Times.Never);

        unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsFailed);
    }
}
