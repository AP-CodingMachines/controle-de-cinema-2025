using ControleDeCinema.Testes.Interface.ModuloAutenticacao;

namespace ControleDeCinema.Testes.Interface;

[TestClass]
[TestCategory("Testes de Interface de Autenticação")]
public class AutenticacaoInterfaceTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Cliente_Corretamente()
    {
        // Arange
        var indexPageObject = new AutenticacaoIndexPageObject(driver!)
            .IrParaRegistro(enderecoBase!);

        // Act
        var formPageObject = indexPageObject
            .PreencherEmail("teste@gmail.com")
            .PreencherSenha("123Abcde")
            .PreencherConfirmarSenha("123Abcde")
            .SelecionarTipoUsuario(0)
            .Confirmar();

        // Assert
        Assert.IsTrue(formPageObject.ContemTituloPaginaInicial());
    }

    [TestMethod]
    public void Deve_Cadastrar_Empresa_Corretamente()
    {
        // Arrange
        var indexPageObject = new AutenticacaoIndexPageObject(driver!)
            .IrParaRegistro(enderecoBase!);

        // Act
        var formPageObject = indexPageObject
            .PreencherEmail("empresa@gmail.com")
            .PreencherSenha("123Abcde")
            .PreencherConfirmarSenha("123Abcde")
            .SelecionarTipoUsuario(1)
            .Confirmar();

        // Assert
        Assert.IsTrue(formPageObject.ContemTituloPaginaInicial());
    }

    [TestMethod]
    public void Deve_Realizar_Login_Corretamente()
    {
        // Arrange
        var indexPageObject = new AutenticacaoIndexPageObject(driver!)
            .IrParaRegistro(enderecoBase!);

        var formPageObject = indexPageObject
            .PreencherEmail("teste@gmail.com")
            .PreencherSenha("123Abcde")
            .PreencherConfirmarSenha("123Abcde")
            .SelecionarTipoUsuario(0)
            .Confirmar();

        var loginPageObject = new AutenticacaoIndexPageObject(driver!)
            .IrParaLogin(enderecoBase!);

        // Act
        var paginaInicial = loginPageObject
            .PreencherEmail("teste@gmail.com")
            .PreencherSenha("123Abcde")
            .Confirmar();

        // Assert
        Assert.IsTrue(paginaInicial.ContemTituloPaginaInicial());
    }

    public void Deve_Realizar_Logout_Corretamente()
    {
        // Arrange
        var indexPageObject = new AutenticacaoIndexPageObject(driver!)
            .IrParaRegistro(enderecoBase!);

        var formPageObject = indexPageObject
            .PreencherEmail("teste@gmail.com")
            .PreencherSenha("123Abcde")
            .PreencherConfirmarSenha("123Abcde")
            .SelecionarTipoUsuario(0)
            .Confirmar();

        // Act
        formPageObject.ClickLogout();

        // Assert
        Assert.IsTrue(formPageObject.EstaNaPaginaDeLogin());
    }
}