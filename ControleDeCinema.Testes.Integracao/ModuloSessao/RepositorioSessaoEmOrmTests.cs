using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using FizzWare.NBuilder;

namespace ControleDeCinema.Testes.Integracao.ModuloSessao
{
    [TestClass]
    [TestCategory ("Teste de Integração de Sessão")]
    public class RepositorioSessaoEmOrmTests : TestFixture
    {
        [TestMethod]
        public void Deve_Cadastrar_Sessao_Corretamente()
        {
            // Arrange
            var genero = Builder<GeneroFilme>.CreateNew().Persist();
            var sala = Builder<Sala>.CreateNew().Persist();
            var filme = Builder<Filme>
                .CreateNew()
                .With(f => f.Genero = genero)
                .Persist();

            var sessao = new Sessao(DateTime.UtcNow, 100, filme, sala);
            // Act
            repositorioSessao.Cadastrar(sessao);
            dbContext.SaveChanges();

            // Assert
            var sessaoSelecionada = repositorioSessao?.SelecionarRegistroPorId(sessao.Id);

            Assert.AreEqual(sessao, sessaoSelecionada);
        }

        [TestMethod]
        public void Deve_Editar_Sessao_Corretamente()
        {
            // Arrange
            var genero = Builder<GeneroFilme>.CreateNew().Persist();
            var sala = Builder<Sala>.CreateNew().Persist();
            var filme = Builder<Filme>
                .CreateNew()
                .With(f => f.Genero = genero)
                .Persist();

            var sessao = new Sessao(DateTime.UtcNow, 100, filme, sala);
            repositorioSessao?.Cadastrar(sessao);
            dbContext?.SaveChanges();

            var sessaoEditada = new Sessao(DateTime.UtcNow, 50, filme, sala);

            // Act
            var conseguiuEditar = repositorioSessao?.Editar(sessao.Id, sessaoEditada);
            dbContext?.SaveChanges();

            // Assert
            var sessaoEncontrada = repositorioSessao?.SelecionarRegistroPorId(sessao.Id);

            Assert.IsTrue(conseguiuEditar);
            Assert.AreEqual(sessao, sessaoEncontrada);
        }

        [TestMethod]
        public void Deve_Excluir_Sessao_Corretamente()
        {
            // Arrange
            var genero = Builder<GeneroFilme>.CreateNew().Persist();
            var sala = Builder<Sala>.CreateNew().Persist();
            var filme = Builder<Filme>
                .CreateNew()
                .With(f => f.Genero = genero)
                .Persist();

            var sessao = new Sessao(DateTime.UtcNow, 100, filme, sala);
            repositorioSessao?.Cadastrar(sessao);
            dbContext?.SaveChanges();

            
            // Act
            var conseguiuExcluir = repositorioSessao?.Excluir(sessao.Id);
            dbContext?.SaveChanges();

            // Assert
            var sessaoSelecionada = repositorioSessao?.SelecionarRegistroPorId(sessao.Id);

            Assert.IsTrue(conseguiuExcluir);
            Assert.IsNull(sessaoSelecionada);
        }

        [TestMethod]
        public void Deve_Selecionar_Sessao_Corretamente()
        {
            // Arrange
            var genero = Builder<GeneroFilme>.CreateNew().Persist();
            var sala = Builder<Sala>.CreateNew().Persist();
            var filme = Builder<Filme>
                .CreateNew()
                .With(f => f.Genero = genero)
                .Persist();

            var sessao = new Sessao(DateTime.UtcNow, 100, filme, sala);
            var sessao1 = new Sessao(DateTime.UtcNow, 50, filme, sala);
            var sessao2 = new Sessao(DateTime.UtcNow, 75, filme, sala);

            List<Sessao> sessoesEsperadas = [sessao, sessao1, sessao2];

            repositorioSessao?.CadastrarEntidades(sessoesEsperadas);
            dbContext?.SaveChanges();

            var sessoesEsperadasOrdenadas = sessoesEsperadas
                .OrderBy(s => s.Id)
                .ToList();

            // Act
            var sessoesRecebidas = repositorioSessao?.SelecionarRegistros();

            var sessoesRecebidasOrdenadas = sessoesRecebidas!
                .OrderBy(s => s.Id)
                .ToList();

            // Assert
            CollectionAssert.AreEqual(sessoesEsperadasOrdenadas, sessoesRecebidasOrdenadas);
        }
    }
}
