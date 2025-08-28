using ControleDeCinema.Dominio.ModuloGeneroFilme;

namespace ControleDeCinema.Testes.Integracao;

[TestClass]
[TestCategory("Testes de Integração de Gênero de Filme")]
public sealed class RepositorioGeneroFilmeEmOrmTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_GeneroFilme_Corretamente()
    {
        // Arrange
        var genero = new GeneroFilme("Ação");

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
        var genero = new GeneroFilme("Comédia");
        repositorioGeneroFilme?.Cadastrar(genero);
        dbContext?.SaveChanges();

        var generoEditado = new GeneroFilme("Comédia Romântica");

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

    [TestMethod]
    public void Deve_Selecionar_GenerosFilme_Corretamente()
    {
        // Arrange
        var genero = new GeneroFilme("Ficção Científica");
        var genero2 = new GeneroFilme("Drama");
        var genero3 = new GeneroFilme("Animação");
        
        List<GeneroFilme> generosEsperados = [genero, genero2, genero3];

        repositorioGeneroFilme?.CadastrarEntidades(generosEsperados);
        dbContext?.SaveChanges();

        var generosEsperadosOrdenados = generosEsperados
            .OrderBy(g => g.Descricao)
            .ToList();

        // Act
        var generosRecebidos = repositorioGeneroFilme?.SelecionarRegistros();

        var generosRecebidosOrdenados = generosRecebidos?
            .OrderBy(g => g.Descricao)
            .ToList();

        // Assert
        CollectionAssert.AreEqual(generosEsperadosOrdenados, generosRecebidosOrdenados);
    }
}
