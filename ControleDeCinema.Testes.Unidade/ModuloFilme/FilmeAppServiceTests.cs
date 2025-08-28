using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloFilme;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidade.ModuloFilme
{
    [TestClass]
    [TestCategory("Teste de Unidade de Filmes")]
    public sealed class FilmeAppServiceTests
    {
        private Mock<IRepositorioFilme>? repositorioFilmeMock;
        private Mock<ITenantProvider>? tenantProviderMock;
        private Mock<IUnitOfWork>? unitOfWorkMock;
        private Mock<ILogger<FilmeAppService>>? loggerMock;

        private FilmeAppService? filmeAppService;

        [TestInitialize]
        public void Setup() 
        {
            repositorioFilmeMock = new Mock<IRepositorioFilme>();
            tenantProviderMock = new Mock<ITenantProvider>();
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
        public void Cadastrar_DeveRetornarOk_QuandoFilmeForValido()
        {
            // Arrange
            var genero = new GeneroFilme("Comedia");

            var filme = new Filme("Debi & Loide", 90, false, genero);

            var filmeTeste = new Filme("Teste", 120, false, genero);

            repositorioFilmeMock?
                .Setup(r => r.SelecionarRegistros())
                .Returns(new List<Filme>() { filmeTeste });
            // Act
            var resultado = filmeAppService?.Cadastrar(filme);

            // Assert

            repositorioFilmeMock?.Verify(r => r.Cadastrar(filme), Times.Once);

            unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.IsSuccess);

        }

        [TestMethod]
        public void Cadastrar_DeveRetornarFalha_QuandoCampoObrigatorioForVazio()
        {
            // Arrange
            var genero = new GeneroFilme("Comedia");
            var filme = new Filme("", 90, false, genero);

            repositorioFilmeMock?.Setup(r => r.SelecionarRegistros())
                .Returns(new List<Filme>() { filme });

            // Act
            var resultado = filmeAppService?.Cadastrar(filme);

            // Assert
            repositorioFilmeMock?.Verify(r => r.Cadastrar(filme), Times.Never);

            unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.IsFailed);
        }

        [TestMethod]
        public void Cadastrar_DeveRetornarFalha_QuandoFilmeJaExistir()
        {
            // Arrange
            var genero = new GeneroFilme("Comedia");

            var filme = new Filme("Debi & Loide", 90, false, genero);

            var filmeTeste = new Filme("Debi & Loide", 90, false, genero);

            repositorioFilmeMock?
                .Setup(r => r.SelecionarRegistros())
                .Returns(new List<Filme>() { filmeTeste });
            
            // Act
            var resultado = filmeAppService?.Cadastrar(filme);

            // Assert
            repositorioFilmeMock?.Verify(r => r.Cadastrar(filme), Times.Never);

            unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.IsFailed);
        }
    }
}
