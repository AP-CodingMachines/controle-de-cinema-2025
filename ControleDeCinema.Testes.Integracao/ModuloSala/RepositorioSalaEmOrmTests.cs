using ControleDeCinema.Dominio.ModuloSala;

namespace ControleDeCinema.Testes.Integracao.ModuloSala
{
    [TestClass]
    [TestCategory("Testes de Integração de Salas")]
    public sealed class RepositorioGeneroSalaEmOrmTests : TestFixture
    {
        [TestMethod]
        public void Deve_Cadastrar_Sala_Corretamente()
        {
            // Arrange
            var sala = new Sala
            {
                Numero = 1,
                Capacidade = 100
            };
            // Act
            repositorioSala?.Cadastrar(sala);
            dbContext?.SaveChanges();

            // Assert
            var salaSelecionada = repositorioSala?.SelecionarRegistroPorId(sala.Id);

            Assert.AreEqual(sala, salaSelecionada);
        }

        [TestMethod]
        public void Deve_Editar_Sala_Corretamente()
        {
            // Arrange
            var sala = new Sala 
            {
                Numero = 2,
                Capacidade = 50
            };
            repositorioSala?.Cadastrar(sala);
            dbContext?.SaveChanges();

            var salaEditada = new Sala
            {
                Numero = 1,
                Capacidade = 100
            };

            // Act
            var conseguiuEditar = repositorioSala?.Editar(sala.Id, salaEditada);
            dbContext?.SaveChanges();

            // Assert
            var salaEncontrada = repositorioSala?.SelecionarRegistroPorId(sala.Id);

            Assert.IsTrue(conseguiuEditar);
            Assert.AreEqual(sala, salaEncontrada);
        }

        [TestMethod]
        public void Deve_Excluir_Sala_Corretamente()
        {
            // Arrange
            var sala = new Sala
            {
                Numero = 1
            };
            repositorioSala?.Cadastrar(sala);
            dbContext?.SaveChanges();

            // Act
            var conseguiuExcluir = repositorioSala?.Excluir(sala.Id);
            dbContext?.SaveChanges();

            // Assert
            var salaSelecionada = repositorioSala?.SelecionarRegistroPorId(sala.Id);

            Assert.IsTrue(conseguiuExcluir);
            Assert.IsNull(salaSelecionada);
        }

        [TestMethod]
        public void Deve_Selecionar_Sala_Corretamente()
        {
            // Arrange
            var sala = new Sala(1, 100);
            var sala1 = new Sala(2, 50);
            var sala2 = new Sala(3, 75);

            List<Sala> salasEsperadas = [sala, sala1, sala2];

            repositorioSala?.CadastrarEntidades(salasEsperadas);
            dbContext?.SaveChanges();

            var salasEsperadasOrdenadas = salasEsperadas
                .OrderBy(s => s.Numero)
                .ToList();

            // Act
            var salasRecebidas = repositorioSala?.SelecionarRegistros();

            var salasRecebidasOrdenadas = salasRecebidas!
                .OrderBy(s => s.Numero)
                .ToList();

            // Assert
            CollectionAssert.AreEqual(salasEsperadasOrdenadas, salasRecebidasOrdenadas);
        }
    }

}
