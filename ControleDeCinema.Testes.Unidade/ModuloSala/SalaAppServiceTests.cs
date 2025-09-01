using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloSala;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloSala;
using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidades.ModuloSala;

[TestClass]
[TestCategory("Unidade - SalaAppService")]
public class SalaAppServiceTestes
{
    private SalaAppService salaAppService;
    private Mock<ITenantProvider> tenantProviderMock;
    private Mock<IRepositorioSala> repositorioSalaMock;
    private Mock<IUnitOfWork> unitOfWorkMock;
    private Mock<ILogger<SalaAppService>> loggerMock;

    [TestInitialize]
    public void Setup()
    {
        tenantProviderMock = new Mock<ITenantProvider>();
        repositorioSalaMock = new Mock<IRepositorioSala>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<SalaAppService>>();

        salaAppService = new SalaAppService(
            tenantProviderMock.Object,
            repositorioSalaMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object
        );
    }

    [TestMethod]
    public void Cadastrar_SalaValida_DeveRetornarSucesso()
    {
        // Arrange
        var sala = new Sala(1, 30);

        repositorioSalaMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala>());

        // Act
        Result resultado = salaAppService.Cadastrar(sala);

        // Assert
        repositorioSalaMock.Verify(r => r.Cadastrar(sala), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Cadastrar_SalaDuplicada_DeveRetornarFalha()
    {
        // Arrange
        var sala = new Sala(1, 30);
        var salaDuplicada = new Sala(1, 30);

        repositorioSalaMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala> { salaDuplicada });

        // Act
        var resultado = salaAppService.Cadastrar(sala);

        // Assert
        repositorioSalaMock.Verify(r => r.Cadastrar(It.IsAny<Sala>()), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Editar_SalaValida_DeveRetornarSucesso()
    {
        // Arrange
        var sala = new Sala(1, 15);
        var salaEditada = new Sala(1, 30);

        repositorioSalaMock.Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala> { sala });

        repositorioSalaMock.Setup(r => r.SelecionarRegistroPorId(sala.Id))
            .Returns(sala);

        // Act
        var resultado = salaAppService.Editar(sala.Id, salaEditada);

        // Assert
        repositorioSalaMock.Verify(r => r.Editar(sala.Id, salaEditada), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Editar_SalaDuplicada_DeveRetornarFalha()
    {
        // Arrange
        var sala1 = new Sala(1, 15);
        var sala2 = new Sala(2, 30);
        var salaEditada = new Sala(2, 15);

        var salasExistentes = new List<Sala> { sala1, sala2 };

        repositorioSalaMock.Setup(r => r.SelecionarRegistros())
            .Returns(salasExistentes);

        // Act
        var resultado = salaAppService.Editar(sala1.Id, salaEditada);

        // Assert
        repositorioSalaMock.Verify(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Sala>()), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Registro duplicado", resultado.Errors[0].Message);
    }

    [TestMethod]
    public void Excluir_SalaExistente_DeveRetornarSucesso()
    {
        // Arrange
        var sala = new Sala(1, 15);

        repositorioSalaMock.Setup(r => r.SelecionarRegistroPorId(sala.Id))
            .Returns(sala);

        repositorioSalaMock.Setup(r => r.Excluir(sala.Id))
            .Returns(true);

        // Act
        var resultado = salaAppService.Excluir(sala.Id);

        // Assert
        repositorioSalaMock.Verify(r => r.Excluir(sala.Id), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Excluir_SalaInexistente_DeveRetornarFalha()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        repositorioSalaMock.Setup(r => r.Excluir(idInexistente))
            .Returns(false);

        // Act
        var resultado = salaAppService.Excluir(idInexistente);

        // Assert
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Registro não encontrado", resultado.Errors[0].Message);
    }

    [TestMethod]
    public void Selecionar_PorId_Existente_DeveRetornarSucesso()
    {
        // Arrange
        var sala = new Sala(1, 15);

        repositorioSalaMock.Setup(r => r.SelecionarRegistroPorId(sala.Id))
            .Returns(sala);

        // Act
        var resultado = salaAppService.SelecionarPorId(sala.Id);

        // Assert
        repositorioSalaMock.Verify(r => r.SelecionarRegistroPorId(sala.Id), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(sala, resultado.Value);
    }

    [TestMethod]
    public void Selecionar_PorId_Inexistente_DeveRetornarFalha()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        repositorioSalaMock.Setup(r => r.SelecionarRegistroPorId(idInexistente))
            .Returns((Sala)null);

        // Act
        var resultado = salaAppService.SelecionarPorId(idInexistente);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Registro não encontrado", resultado.Errors[0].Message);
        Assert.IsNull(resultado.ValueOrDefault);
    }

    [TestMethod]
    public void Selecionar_Todos_DeveRetornarSucesso()
    {
        // Arrange
        var sala1 = new Sala(1, 15);
        var sala2 = new Sala(2, 30);

        var salas = new List<Sala> { sala1, sala2 };

        repositorioSalaMock.Setup(r => r.SelecionarRegistros())
            .Returns(salas);

        // Act
        var resultado = salaAppService.SelecionarTodos();

        // Assert
        repositorioSalaMock.Verify(r => r.SelecionarRegistros(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
        CollectionAssert.AreEquivalent(salas, resultado.Value);
    }
}
