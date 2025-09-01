using ControleDeCinema.Aplicacao.ModuloSessao;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidades.ModuloIngresso;

[TestClass]
[TestCategory("Testes de Unidade de Ingresso")]
public class IngressoAppServiceTestes
{
    private IngressoAppService ingressoAppService;

    private Mock<ITenantProvider> tenantProviderMock;
    private Mock<IRepositorioIngresso> repositorioIngressoMock;
    private Mock<ILogger<IngressoAppService>> loggerMock;

    [TestInitialize]
    public void Setup()
    {
        tenantProviderMock = new Mock<ITenantProvider>();
        repositorioIngressoMock = new Mock<IRepositorioIngresso>();
        loggerMock = new Mock<ILogger<IngressoAppService>>();

        ingressoAppService = new IngressoAppService(
            tenantProviderMock.Object,
            repositorioIngressoMock.Object,
            loggerMock.Object
        );
    }

    [TestMethod]
    public void Deve_RetornarSucesso_QuandoSelecionarTodos()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        var genero = new GeneroFilme("Comédia") { Id = Guid.NewGuid() };
        var filme = new Filme("Esposa de Mentirinha", 117, true, genero);
        var sala = new Sala(1, 30) { Id = Guid.NewGuid() };
        var inicio = new DateTime(2025, 08, 09, 14, 30, 00);
        var sessao = new Sessao(inicio, 30, filme, sala);

        var ingressos = new List<Ingresso>
        {
            sessao.GerarIngresso(1, true),
            sessao.GerarIngresso(3, true)
        };

        var outroIngresso = sessao.GerarIngresso(5, true);

        tenantProviderMock
            .SetupGet(t => t.UsuarioId)
            .Returns(usuarioId);

        repositorioIngressoMock
            .Setup(r => r.SelecionarRegistros(usuarioId))
            .Returns(ingressos);

        // Act
        var resultado = ingressoAppService.SelecionarTodos();
        var selecionados = resultado.ValueOrDefault;

        // Assert
        repositorioIngressoMock.Verify(r => r.SelecionarRegistros(usuarioId), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsSuccess);
        CollectionAssert.AreEquivalent(ingressos, selecionados);
        CollectionAssert.DoesNotContain(ingressos, outroIngresso);
    }

    [TestMethod]
    public void Deve_RetornarFalha_QuandoOcorrerExcecaoAoSelecionarTodos()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        tenantProviderMock
            .SetupGet(t => t.UsuarioId)
            .Returns(usuarioId);

        repositorioIngressoMock
            .Setup(r => r.SelecionarRegistros(usuarioId))
            .Throws(new Exception());

        // Act
        var resultado = ingressoAppService.SelecionarTodos();
        var selecionados = resultado.ValueOrDefault;

        // Assert
        repositorioIngressoMock.Verify(r => r.SelecionarRegistros(usuarioId), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsNull(selecionados);
    }
}
