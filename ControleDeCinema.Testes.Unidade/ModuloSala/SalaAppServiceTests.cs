using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloSala;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloSala;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidade.ModuloSala
{
    [TestClass]
    [TestCategory("Teste de Unidade de Salas")]
    public sealed class SalaAppServiceTests
    {
        private Mock<IRepositorioSala>? repositorioSalaMock;
        private Mock<ITenantProvider>? tenantProviderMock;
        private Mock<IUnitOfWork>? unitOfWorkMock;
        private Mock<ILogger<SalaAppService>>? loogerMock;

        private SalaAppService? salaAppService;

        [TestInitialize]
        public void Setup() 
        {
            repositorioSalaMock = new Mock<IRepositorioSala>();
            tenantProviderMock = new Mock<ITenantProvider>();
            unitOfWorkMock = new Mock<IUnitOfWork>();
            loogerMock = new Mock<ILogger<SalaAppService>>();

            salaAppService = new SalaAppService(
                tenantProviderMock.Object,
                repositorioSalaMock.Object,
                unitOfWorkMock.Object,
                loogerMock.Object
            );
        }

        [TestMethod]
        public void Cadastrar_DeveRetornarOk_QuandoSalaForValida()
        {
            // Arrange
            var sala = new Sala(1, 100);

            var salaTeste = new Sala(2, 150);
            
            repositorioSalaMock?
                .Setup(r => r.SelecionarRegistros())
                .Returns(new List<Sala>() { salaTeste });
            
            // Act
            var resultado = salaAppService?.Cadastrar(sala);

            // Assert
            repositorioSalaMock?.Verify(r => r.Cadastrar(sala), Times.Once);

            unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado?.IsSuccess);
        }

        [TestMethod]
        public void Cadastrar_DeveRetornarErro_QuandoSalaForDuplicada()
        {
            // Arrange
            var sala = new Sala(1, 100);

            var salaTeste = new Sala(1, 150);
            
            repositorioSalaMock?
                .Setup(r => r.SelecionarRegistros())
                .Returns(new List<Sala>() { salaTeste });
            
            // Act
            var resultado = salaAppService?.Cadastrar(sala);

            // Assert
            repositorioSalaMock?.Verify(r => r.Cadastrar(sala), Times.Never);

            unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado?.IsFailed);
            
        }
    }
}
