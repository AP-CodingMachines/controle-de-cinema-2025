using ControleDeCinema.Testes.Interface.ModuloAutenticacao;
using ControleDeCinema.Testes.Interface.ModuloGeneroFilme;

namespace ControleDeCinema.Testes.Interface;

[TestClass]
[TestCategory("Testes de Interface de G�nero de Filme")]
public class GeneroInterfaceTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Genero_Corretamente()
    {
        // Arrange
        var indexPageObject = new AutenticacaoIndexPageObject(driver!)
            .IrParaRegistro(enderecoBase!);

        var formPageObject = indexPageObject
            .PreencherEmail("empresa@gmail.com")
            .PreencherSenha("123Abcde")
            .PreencherConfirmarSenha("123Abcde")
            .SelecionarTipoUsuario(1)
            .Confirmar();

        // Act
        var generoIndexPageObject = formPageObject
            .IrParaGeneroFilme(enderecoBase!);

        var generoFormPageObject = generoIndexPageObject
            .
    }
}