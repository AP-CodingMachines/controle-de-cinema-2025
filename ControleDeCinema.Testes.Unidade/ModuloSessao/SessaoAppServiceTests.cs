using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloSessao;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidade.ModuloSessao
{
    [TestClass]
    [TestCategory("Testes Unitarios de Sessão")]
    public sealed class SessaoAppServiceTests
    {
        private Mock<IRepositorioSessao>? repositorioSessaoMock;
        private Mock<ITenantProvider>? tenantProviderMock;
        private Mock<IUnitOfWork>? unitOfWorkMock;
        private Mock<ILogger<SessaoAppService>>? loggerMock;

        private SessaoAppService? sessaoAppService;

        [TestInitialize]
        public void Setup()
        {
            repositorioSessaoMock = new Mock<IRepositorioSessao>();
            tenantProviderMock = new Mock<ITenantProvider>();
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
        public void Cadastrar_DeveRetornarFalha_QuandoHouverConflitoDeHorario_NaMesmaSala()
        {
            // Arrange
            var sala = new Sala { Id = Guid.NewGuid(), Capacidade = 100 };
            var sessaoExistente = new Sessao
            {
                Id = Guid.NewGuid(),
                Sala = sala,
                Inicio = new DateTime(2024, 10, 10, 14, 0, 0),
                NumeroMaximoIngressos = 50,
                UsuarioId = Guid.NewGuid()
            };
            var novaSessao = new Sessao
            {
                Id = Guid.NewGuid(),
                Sala = sala,
                Inicio = new DateTime(2024, 10, 10, 14, 0, 0), // Mesmo horário da sessão existente
                NumeroMaximoIngressos = 50,
                UsuarioId = Guid.NewGuid()
            };
            repositorioSessaoMock!
                .Setup(r => r.SelecionarRegistros())
                .Returns(new List<Sessao> { sessaoExistente });
            // Act
            var resultado = sessaoAppService!.Cadastrar(novaSessao);
            // Assert
            repositorioSessaoMock.Verify(r => r.Cadastrar(sessaoExistente), Times.Never);

            unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
            
            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.IsFailed);



        }

        



    }
}
