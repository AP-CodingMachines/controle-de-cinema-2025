using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;

namespace ControleDeCinema.Testes.Integracao.ModuloFilme
{
    [TestClass]
    [TestCategory ("Teste de Integração de Filmes")]
    public sealed class RepositorioFilmeEmOrmTests : TestFixture
    {
        [TestMethod]
        public void Deve_Cadastrar_Filme_Corretamente()
        {
            // Arrange
            var genero = Builder<GeneroFilme>.CreateNew().Persist();
            
            var filme = new Filme
            {
                Titulo = "O poderoso Chefão",
                Duracao = 90,
                Lancamento = false,
                Genero = genero,

            };
            // Act
            repositorioFilme.Cadastrar(filme);
            dbContext.SaveChanges();

            // Assert
            var filmeSelecionado = repositorioFilme?.SelecionarRegistroPorId(filme.Id);

            Assert.AreEqual(filme, filmeSelecionado);
        }

        [TestMethod]
        public void Deve_Editar_Filme_Corretamente()
        {
            // Arrange
            var genero = Builder<GeneroFilme>.CreateNew().Persist();

            var filme = new Filme("Jumper", 90, true, genero);
            
            repositorioFilme?.Cadastrar(filme);
            dbContext?.SaveChanges();

            var filmeEditado = new Filme("Jumper", 120, false, genero);

            // Act
            var conseguiuEditar = repositorioFilme?.Editar(filme.Id, filmeEditado);
            dbContext?.SaveChanges();

            // Assert
            var filmeEncontrado = repositorioFilme?.SelecionarRegistroPorId(filme.Id);

            Assert.IsTrue(conseguiuEditar);
            Assert.AreEqual(filme, filmeEncontrado);
        }

        [TestMethod]
        public void Deve_Excluir_Filme_Corretamente()
        {
            // Arrange
            var genero = Builder<GeneroFilme>.CreateNew().Persist();

            var filme = new Filme("Jumper", 85, false, genero);
            
            repositorioFilme?.Cadastrar(filme);
            dbContext?.SaveChanges();

            // Act
            var conseguiuExcluir = repositorioFilme?.Excluir(filme.Id);
            dbContext?.SaveChanges();

            // Assert
            var filmeSelecionado = repositorioFilme?.SelecionarRegistroPorId(filme.Id);

            Assert.IsTrue(conseguiuExcluir);
            Assert.IsNull(filmeSelecionado);
        }

        [TestMethod]
        public void Deve_Selecionar_Filme_Corretamente()
        {
            // Arrange
            var genero = Builder<GeneroFilme>.CreateNew().Persist();

            var filme = new Filme("As aventuras de PI", 90, false, genero);
            var filme1 = new Filme("Jumper", 120, true, genero);
            var filme2 = new Filme("Vidro", 160, false, genero);

            List<Filme> filmesEsperados = [filme, filme1, filme2];

            repositorioFilme?.CadastrarEntidades(filmesEsperados);
            dbContext?.SaveChanges();

            var filmesEsperadasOrdenados = filmesEsperados
                .OrderBy(f => f.Titulo)
                .ToList();

            // Act
            var filmesRecebidos = repositorioFilme?.SelecionarRegistros();

            var filmesRecebidosOrdenados = filmesRecebidos!
                .OrderBy(f => f.Titulo)
                .ToList();

            // Assert
            CollectionAssert.AreEqual(filmesEsperadasOrdenados, filmesRecebidosOrdenados);
        }
    }
}
