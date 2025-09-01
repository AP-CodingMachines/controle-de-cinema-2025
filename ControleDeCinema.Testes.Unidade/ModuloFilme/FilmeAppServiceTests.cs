using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloFilme;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidades.ModuloFilme;

[TestClass]
[TestCategory("Unidade - FilmeAppService")]
public class FilmeAppServiceTestes
{
    private FilmeAppService filmeAppService;
    private Mock<ITenantProvider> tenantProviderMock;
    private Mock<IRepositorioFilme> repositorioFilmeMock;
    private Mock<IUnitOfWork> unitOfWorkMock;
    private Mock<ILogger<FilmeAppService>> loggerMock;

    [TestInitialize]
    public void Setup()
    {
        tenantProviderMock = new Mock<ITenantProvider>();
        repositorioFilmeMock = new Mock<IRepositorioFilme>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<FilmeAppService>>();

        filmeAppService = new FilmeAppService(
            tenantProviderMock.Object,
            repositorioFilmeMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object
        );
    }

    [TestMethod]
    public void Cadastrar_DeveRetornarSucesso_QuandoForValido()
    {
        var genero = new GeneroFilme("Terror");
        var filme = new Filme("A freira", 117, true, genero);

        repositorioFilmeMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>());

        var resultado = filmeAppService.Cadastrar(filme);

        repositorioFilmeMock.Verify(r => r.Cadastrar(filme), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Cadastrar_DeveRetornarFalha_QuandoForDuplicado()
    {
        var genero = new GeneroFilme("Terror");
        var filme = new Filme("A freira", 117, true, genero);

        repositorioFilmeMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme> { filme });

        var resultado = filmeAppService.Cadastrar(filme);

        repositorioFilmeMock.Verify(r => r.Cadastrar(It.IsAny<Filme>()), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Editar_DeveRetornarSucesso_QuandoForValido()
    {
        var genero = new GeneroFilme("Terror");
        var filme = new Filme("A freira", 117, true, genero);
        var filmeEditado = new Filme("Invocação do mal", 95, true, genero);

        repositorioFilmeMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme> { filme });

        repositorioFilmeMock.Setup(r => r.SelecionarRegistroPorId(filme.Id))
            .Returns(filme);

        var resultado = filmeAppService.Editar(filme.Id, filmeEditado);

        repositorioFilmeMock.Verify(r => r.Editar(filme.Id, filmeEditado), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Editar_DeveRetornarFalha_QuandoForDuplicado()
    {
        var genero = new GeneroFilme("Terror");
        var filmeOriginal = new Filme("A freira", 117, true, genero);
        var filmeDuplicado = new Filme("Invocação do mal", 95, true, genero);

        repositorioFilmeMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme> { filmeOriginal, filmeDuplicado });

        var resultado = filmeAppService.Editar(filmeOriginal.Id, filmeDuplicado);

        repositorioFilmeMock.Verify(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Filme>()), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Excluir_DeveRetornarSucesso_QuandoForValido()
    {
        var genero = new GeneroFilme("Terror");
        var filme = new Filme("A freira", 117, true, genero);

        repositorioFilmeMock.Setup(r => r.SelecionarRegistroPorId(filme.Id))
            .Returns(filme);

        repositorioFilmeMock.Setup(r => r.Excluir(filme.Id))
            .Returns(true);

        var resultado = filmeAppService.Excluir(filme.Id);

        repositorioFilmeMock.Verify(r => r.Excluir(filme.Id), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Excluir_DeveRetornarFalha_QuandoNaoForEncontrado()
    {
        var id = Guid.NewGuid();

        repositorioFilmeMock.Setup(r => r.Excluir(id))
            .Returns(false);

        var resultado = filmeAppService.Excluir(id);

        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void SelecionarPorId_DeveRetornarSucesso_QuandoForValido()
    {
        var genero = new GeneroFilme("Terror");
        var filme = new Filme("A freira", 117, true, genero);

        repositorioFilmeMock.Setup(r => r.SelecionarRegistroPorId(filme.Id))
            .Returns(filme);

        var resultado = filmeAppService.SelecionarPorId(filme.Id);

        repositorioFilmeMock.Verify(r => r.SelecionarRegistroPorId(filme.Id), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(filme, resultado.Value);
    }

    [TestMethod]
    public void SelecionarPorId_DeveRetornarFalha_QuandoNaoForEncontrado()
    {
        var id = Guid.NewGuid();

        repositorioFilmeMock.Setup(r => r.SelecionarRegistroPorId(id))
            .Returns((Filme)null);

        var resultado = filmeAppService.SelecionarPorId(id);

        Assert.IsTrue(resultado.IsFailed);
        Assert.IsNull(resultado.ValueOrDefault);
    }

    [TestMethod]
    public void SelecionarTodos_DeveRetornarSucesso_QuandoForValido()
    {
        var genero = new GeneroFilme("Terror");
        var filme1 = new Filme("A freira", 117, true, genero);
        var filme2 = new Filme("Invocação do mal", 95, true, genero);

        var filmes = new List<Filme> { filme1, filme2 };

        repositorioFilmeMock.Setup(r => r.SelecionarRegistros())
            .Returns(filmes);

        var resultado = filmeAppService.SelecionarTodos();

        repositorioFilmeMock.Verify(r => r.SelecionarRegistros(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
        CollectionAssert.AreEquivalent(filmes, resultado.Value);
    }
}
