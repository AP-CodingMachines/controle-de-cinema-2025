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
            sala = new Sala(1, 100);

            //Act
            var salaAdicionada = sala;

            //Assert
            Assert.AreEqual(1, sala.Numero);
            
        }
    }
}
