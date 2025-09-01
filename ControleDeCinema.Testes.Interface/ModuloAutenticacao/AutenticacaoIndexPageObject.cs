using ControleDeCinema.Testes.Interface.ModuloGeneroFilme;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloAutenticacao;

public class AutenticacaoIndexPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public AutenticacaoIndexPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    public AutenticacaoFormPageObject IrParaRegistro(string enderecoBase)
    {
        driver.Navigate().GoToUrl(Path.Combine(enderecoBase, "autenticacao/registro"));

        return new AutenticacaoFormPageObject(driver!);
    }

    public AutenticacaoFormPageObject IrParaLogin(string enderecoBase)
    {
        driver.Navigate().GoToUrl(Path.Combine(enderecoBase, "autenticacao/login"));

        return new AutenticacaoFormPageObject(driver!);
    }

    public GeneroFormPageObject IrParaGeneroFilme(string enderecoBase)
    {
        driver.Navigate().GoToUrl(Path.Combine(enderecoBase, "generos"));

        return new GeneroFormPageObject(driver!);
    }

    public AutenticacaoFormPageObject ClickLogout()
    {
        var dropdownToggle = wait.Until(d => d.FindElement(By.CssSelector(".dropdown-toggle")));
        dropdownToggle.Click();

        var logoutButton = wait.Until(d => d.FindElement(By.CssSelector("form[action='/autenticacao/logout'] button[type='submit']")));
        logoutButton.Click();

        return new AutenticacaoFormPageObject(driver!);
    }

    public bool ContemTituloPaginaInicial()
    {
        wait.Until(d => d.FindElement(By.CssSelector("header h2")).Displayed);

        var titulo = driver.FindElement(By.CssSelector("header h2")).Text;

        return titulo.Contains("Página Inicial");
    }

    public bool EstaNaPaginaDeLogin()
    {
        return driver.Url.Contains("/autenticacao/login");
    }
}
