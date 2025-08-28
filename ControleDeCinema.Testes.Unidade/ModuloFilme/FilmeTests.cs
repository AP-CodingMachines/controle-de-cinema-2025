using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;

namespace ControleDeCinema.Testes.Unidade.ModuloFilme
{
    [TestClass]
    [TestCategory("Teste de Unidade de Filmes")]
    public sealed class FilmeTests
    {
        private Filme? filme;

        [TestMethod]
        public void Deve_Adicionar_Filme_Corretamente() 
        {
            // Arrange
            var genero = new GeneroFilme("Comedia");

            filme = new Filme("Debi & Loide", 50, false, genero);

            //Act
            var filmeAdicionado = filme;

            //Assert

            var contemFilme = filme.Titulo.Contains("Debi & Loide");

            Assert.IsTrue(contemFilme);
        }

    }
}
