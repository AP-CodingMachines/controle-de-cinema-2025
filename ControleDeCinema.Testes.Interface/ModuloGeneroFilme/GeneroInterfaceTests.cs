using ControleDeCinema.Testes.Interface.Compartilhado;
using ControleDeCinema.Testes.Interface.ModuloGeneroFilme;
using Docker.DotNet.Models;

namespace ControleDeCinema.Testes.Interface;

[TestClass]
[TestCategory("Testes de Interface de G�nero de Filme")]
public class GeneroInterfaceTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Disciplina_Corretamente()
    {
        // Arange
        var indexPageObject = new GeneroIndexPageObject(driver!)
            .IrPara(enderecoBase!);

        // Act
        indexPageObject
            .ClickCadastrar()
            .PreencherNome("Terror")
            .Confirmar();

        // Assert
        Assert.IsTrue(indexPageObject.ContemDisciplina("Terror"));
    }
}
