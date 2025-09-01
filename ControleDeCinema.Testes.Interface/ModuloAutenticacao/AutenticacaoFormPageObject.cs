using ControleDeCinema.Testes.Interface.ModuloGeneroFilme;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloAutenticacao;

public class AutenticacaoFormPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public AutenticacaoFormPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        wait.Until(d => d.FindElement(By.CssSelector("form")).Displayed);
    }

    public AutenticacaoFormPageObject PreencherEmail(string email)
    {
        var inputEmail = driver?.FindElement(By.Id("Email"));
        inputEmail?.Clear();
        inputEmail?.SendKeys(email);

        return this;
    }

    public AutenticacaoFormPageObject PreencherSenha(string senha)
    {
        var inputSenha = driver?.FindElement(By.Id("Senha"));
        inputSenha?.Clear();
        inputSenha?.SendKeys(senha);


        return this;
    }

    public AutenticacaoFormPageObject PreencherConfirmarSenha(string senha)
    {
        var inputConfirmarSenha = driver?.FindElement(By.Id("ConfirmarSenha"));
        inputConfirmarSenha?.Clear();
        inputConfirmarSenha?.SendKeys(senha);

        return this;
    }

    public AutenticacaoFormPageObject SelecionarTipoUsuario(int tipo)
    {
        var selectTipoUsuario = driver?.FindElement(By.Id("Tipo"));
        var selectElement = new SelectElement(selectTipoUsuario!);

        selectElement.SelectByValue(tipo.ToString());

        return this;
    }

    public AutenticacaoIndexPageObject Confirmar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']"))).Click();

        return new AutenticacaoIndexPageObject(driver!);
    }
}
