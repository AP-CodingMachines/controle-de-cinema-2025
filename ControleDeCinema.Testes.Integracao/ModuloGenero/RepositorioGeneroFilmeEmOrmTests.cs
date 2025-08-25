using ControleDeCinema.Dominio.ModuloGeneroFilme;

namespace ControleDeCinema.Testes.Integracao;

[TestClass]
[TestCategory("Testes de Integra��o de G�nero de Filme")]
public sealed class RepositorioGeneroFilmeEmOrmTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_GeneroFilme_Corretamente()
    {
        // Arrange
        var genero = new GeneroFilme("A��o");

        // Act
        repositorioGeneroFilme?.Cadastrar(genero);
        dbContext?.SaveChanges();

        // Assert
        var generoSelecionado = repositorioGeneroFilme?.SelecionarRegistroPorId(genero.Id);

        Assert.AreEqual(genero, generoSelecionado);
    }

    [TestMethod]
    public void Deve_Editar_GeneroFilme_Corretamente()
    {
        // Arrange
        var genero = new GeneroFilme("Com�dia");
        repositorioGeneroFilme?.Cadastrar(genero);
        dbContext?.SaveChanges();

        var generoEditado = new GeneroFilme("Com�dia Rom�ntica");

        // Act
        var conseguiuEditar = repositorioGeneroFilme?.Editar(genero.Id, generoEditado);
        dbContext?.SaveChanges();

        // Assert
        var generoEncontrado = repositorioGeneroFilme?.SelecionarRegistroPorId(genero.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual(genero, generoEncontrado);
    }

    [TestMethod]
    public void Deve_Excluir_GeneroFilme_Corretamente()
    {
        // Arrange
        var genero = new GeneroFilme("Terror");
        repositorioGeneroFilme?.Cadastrar(genero);
        dbContext?.SaveChanges();

        // Act
        var conseguiuExcluir = repositorioGeneroFilme?.Excluir(genero.Id);
        dbContext?.SaveChanges();

        // Assert
        var generoSelecionado = repositorioGeneroFilme?.SelecionarRegistroPorId(genero.Id);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(generoSelecionado);
    }
}
