using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;

namespace ControleDeCinema.Testes.Unidade.ModuloGeneroFilme
{
    [TestClass]
    [TestCategory("Testes de Unidade de Gênero de Filmes")]
    public sealed class GeneroFilmeTests
    {
        private GeneroFilme? genero;

        [TestMethod]
        public void Deve_AdicionarFilme_AoGeneroFilme_Corretamente()
        {
            // Arrange
            genero = new GeneroFilme("Terror");

            var filme = new Filme("A Freira", 50, false, genero);

            // Act
            genero.AdicionarFilme(filme);

            // Assert
            var generoContemFilme = genero.Filmes.Contains(filme);

            Assert.IsTrue(generoContemFilme);
        }
    }
}
