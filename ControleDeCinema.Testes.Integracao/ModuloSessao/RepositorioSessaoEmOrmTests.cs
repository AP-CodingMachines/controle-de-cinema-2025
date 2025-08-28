using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;

namespace ControleDeCinema.Testes.Integracao;

[TestClass]
[TestCategory("Testes de Integração de Sessão")]
public class RepositorioSessaoEmOrmTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Sessao_Corretamente()
    {
        // Arrange
        var filme = Builder<Filme>.CreateNew().Persist();
        var sala = Builder<Sala>.CreateNew().Persist();

        var sessao = new Sessao(new DateTime(), 100, filme, sala);

        // Act
        repositorioSessao?.Cadastrar(sessao);
        dbContext?.SaveChanges();

        // Assert
        var sessaoSelecionada = repositorioSessao?.SelecionarRegistroPorId(sessao.Id);

        Assert.AreEqual(sessao, sessaoSelecionada);
    }
}
