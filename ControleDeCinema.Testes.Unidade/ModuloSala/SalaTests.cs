using ControleDeCinema.Dominio.ModuloSala;

namespace ControleDeCinema.Testes.Unidade.ModuloSala
{
    [TestClass]
    [TestCategory("Teste de Unidade de Salas")]
    public sealed class SalaTests
    {
        private Sala? sala;

        [TestMethod]
        public void Deve_Adicionar_Sala_Corretamente()
        {
            // Arrange
            var numeroEsperado = 1;
            var capacidadeEsperada = 100;

            //Act
            var salaTeste = new Sala(numeroEsperado, capacidadeEsperada);

            //Assert
            Assert.AreEqual(numeroEsperado, salaTeste.Numero);
            Assert.AreEqual(capacidadeEsperada, salaTeste.Capacidade);

        }
    }
}
