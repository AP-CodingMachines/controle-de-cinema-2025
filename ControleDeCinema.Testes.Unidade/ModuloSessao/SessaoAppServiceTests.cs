using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloSessao;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidades.ModuloSessao;

[TestClass]
[TestCategory("Testes de Unidade de Sessão")]
public class SessaoAppServiceTestes
{
    private SessaoAppService sessaoAppService;
    private Mock<ITenantProvider> tenantProviderMock;
    private Mock<IRepositorioSessao> repositorioSessaoMock;
    private Mock<IUnitOfWork> unitOfWorkMock;
    private Mock<ILogger<SessaoAppService>> loggerMock;

    [TestInitialize]
    public void Setup()
    {
        tenantProviderMock = new Mock<ITenantProvider>();
        repositorioSessaoMock = new Mock<IRepositorioSessao>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<SessaoAppService>>();

        sessaoAppService = new SessaoAppService(
            tenantProviderMock.Object,
            repositorioSessaoMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object
        );
    }

    [TestMethod]
    public void Deve_Cadastrar_Sessao_QuandoForValido()
    {
        var genero = new GeneroFilme("Ação");
        var filme = new Filme("Matrix", 136, true, genero);
        var sala = new Sala(1, 50);
        var sessao = new Sessao(new DateTime(), 30, filme, sala);

        repositorioSessaoMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>());

        var resultado = sessaoAppService.Cadastrar(sessao);

        repositorioSessaoMock.Verify(r => r.Cadastrar(sessao), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Deve_Falhar_Cadastro_QuandoCapacidadeForExcedida()
    {
        var genero = new GeneroFilme("Drama");
        var filme = new Filme("Clube da Luta", 139, true, genero);
        var sala = new Sala(2, 40);
        var sessao = new Sessao(new DateTime(), 100, filme, sala);

        repositorioSessaoMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>());

        var resultado = sessaoAppService.Cadastrar(sessao);

        repositorioSessaoMock.Verify(r => r.Cadastrar(sessao), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Deve_Falhar_Cadastro_QuandoForDuplicado()
    {
        var genero = new GeneroFilme("Suspense");
        var filme = new Filme("Seven", 127, true, genero);
        var sala = new Sala(3, 60);

        var sessao = new Sessao(new DateTime(), 40, filme, sala);

        repositorioSessaoMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao> { sessao });

        var outraSessao = new Sessao(new DateTime(), 40, filme, sala);

        var resultado = sessaoAppService.Cadastrar(outraSessao);

        repositorioSessaoMock.Verify(r => r.Cadastrar(It.IsAny<Sessao>()), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Deve_Editar_Sessao_QuandoForValido()
    {
        var genero = new GeneroFilme("Comédia");
        var filme = new Filme("O Máskara", 101, true, genero);
        var sala = new Sala(4, 80);

        var sessao = new Sessao(new DateTime(), 20, filme, sala);

        repositorioSessaoMock.Setup(r => r.SelecionarRegistroPorId(sessao.Id))
            .Returns(sessao);

        repositorioSessaoMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>());

        var editada = new Sessao(new DateTime(), 30, filme, sala);

        repositorioSessaoMock.Setup(r => r.Editar(sessao.Id, editada))
            .Returns(true);

        var resultado = sessaoAppService.Editar(sessao.Id, editada);

        repositorioSessaoMock.Verify(r => r.Editar(sessao.Id, editada), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }


    [TestMethod]
    public void Deve_RetornarFalha_Edicao_QuandoCapacidadeForExcedida()
    {
        var genero = new GeneroFilme("Romance");
        var filme = new Filme("Titanic", 195, true, genero);
        var sala = new Sala(5, 100);

        var sessao = new Sessao(new DateTime(), 80, filme, sala);
        var editada = new Sessao(new DateTime(), 200, filme, sala);

        repositorioSessaoMock.Setup(r => r.SelecionarRegistroPorId(sessao.Id))
            .Returns(sessao);

        repositorioSessaoMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>());

        var resultado = sessaoAppService.Editar(sessao.Id, editada);

        repositorioSessaoMock.Verify(r => r.Editar(sessao.Id, editada), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        Assert.IsTrue(resultado.IsFailed);
    }


    [TestMethod]
    public void Deve_Falhar_Edicao_QuandoForDuplicado()
    {
        var genero = new GeneroFilme("Fantasia");
        var filme = new Filme("O Senhor dos Anéis", 178, true, genero);
        var sala = new Sala(6, 120);

        var sessao = new Sessao(new DateTime(), 60, filme, sala);
        var duplicada = new Sessao(new DateTime(), 60, filme, sala);

        repositorioSessaoMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao> { sessao, duplicada });

        var resultado = sessaoAppService.Editar(sessao.Id, duplicada);

        repositorioSessaoMock.Verify(r => r.Editar(It.IsAny<Guid>(), duplicada), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Deve_Excluir_Sessao_QuandoForValido()
    {
        var genero = new GeneroFilme("Terror");
        var filme = new Filme("O Exorcista", 122, true, genero);
        var sala = new Sala(7, 90);

        var sessao = new Sessao(new DateTime(), 30, filme, sala);

        repositorioSessaoMock.Setup(r => r.SelecionarRegistroPorId(sessao.Id))
            .Returns(sessao);

        repositorioSessaoMock.Setup(r => r.Excluir(sessao.Id))
            .Returns(true);

        var resultado = sessaoAppService.Excluir(sessao.Id);

        repositorioSessaoMock.Verify(r => r.Excluir(sessao.Id), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Deve_Falhar_Exclusao_QuandoNaoExistir()
    {
        var resultado = sessaoAppService.Excluir(Guid.NewGuid());

        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Deve_Selecionar_PorId_QuandoForValido()
    {
        var genero = new GeneroFilme("Aventura");
        var filme = new Filme("Jurassic Park", 127, true, genero);
        var sala = new Sala(8, 70);

        var sessao = new Sessao(new DateTime(), 40, filme, sala);

        repositorioSessaoMock.Setup(r => r.SelecionarRegistroPorId(sessao.Id))
            .Returns(sessao);

        var resultado = sessaoAppService.SelecionarPorId(sessao.Id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(sessao, resultado.Value);
    }

    [TestMethod]
    public void Deve_Falhar_Selecao_PorId_QuandoNaoExistir()
    {
        var resultado = sessaoAppService.SelecionarPorId(Guid.NewGuid());

        Assert.IsTrue(resultado.IsFailed);
    }
}
