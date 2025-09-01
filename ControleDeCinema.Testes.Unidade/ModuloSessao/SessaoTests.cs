using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;

namespace ControleDeCinema.Testes.Unidade.ModuloSessao
{
    [TestClass]
    [TestCategory("Teste de Unidade de Sessão")]
    public sealed class SessaoTests
    {
        private Sessao? sessao;

        [TestMethod]
        public void Deve_Adicionar_Sessao_Corretamente()
        {
            // Arrange
            var genero = new GeneroFilme("Comedia");
            var filme = new Filme("Debi & Loide", 50, false, genero);
            var sala = new Sala(1, 100);
            var dataHoraInicio = new DateTime(2024, 06, 20, 14, 0, 0);
            

            //Act
            sessao = new Sessao(dataHoraInicio, 100, filme, sala);

            //Assert
            Assert.AreEqual(new DateTime(2024, 06, 20, 14, 0, 0), sessao.Inicio);
        }
    }
}
