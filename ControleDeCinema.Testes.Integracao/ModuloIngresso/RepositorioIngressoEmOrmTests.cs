using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Infraestrutura.Orm.ModuloSessao;
using FizzWare.NBuilder;

namespace ControleDeCinema.Testes.Integracao.Repositorios;

[TestClass]
[TestCategory("Testes de Integração de Ingressos")]
public class IngressoRepositorioTestes : TestFixture
{
    [TestMethod]
    public void Deve_Selecionar_Ingressos_Corretamente()
    {
        // Arrange
        var genero = Builder<GeneroFilme>.CreateNew().Persist();

        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = genero).Persist();

        var sala = Builder<Sala>.CreateNew().Persist();

        var idUsuario = Guid.NewGuid();

        List<Sessao> sessoesUsuarioA = new List<Sessao>
        {
            new(new DateTime(), 25, filme, sala)
            { UsuarioId = idUsuario },
            new(new DateTime(), 25, filme, sala)
            { UsuarioId = idUsuario },
            new(new DateTime(), 25, filme, sala)
            { UsuarioId = idUsuario }
        };

        List<Ingresso> ingressosUsuarioA = new();

        foreach (var sessao in sessoesUsuarioA)
        {
            var novoIngresso = sessao.GerarIngresso(1, true);
            novoIngresso.UsuarioId = idUsuario;
            ingressosUsuarioA.Add(novoIngresso);
        }

        repositorioSessao.CadastrarEntidades(sessoesUsuarioA);
        dbContext.SaveChanges();

        // Act
        List<Ingresso> ingressosSelecionados = repositorioIngresso.SelecionarRegistros(idUsuario);
        List<Ingresso> ingressosEsperados = sessoesUsuarioA.SelectMany(s => s.Ingressos).ToList();

        // Assert
        Assert.AreEqual(ingressosEsperados.Count, ingressosSelecionados.Count);
        CollectionAssert.AreEquivalent(ingressosEsperados, ingressosSelecionados);
    }
}