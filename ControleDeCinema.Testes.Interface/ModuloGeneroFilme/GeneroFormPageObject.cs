using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloGeneroFilme;

public class GeneroFormPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public GeneroFormPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        wait.Until(d => d.FindElement(By.CssSelector("form")).Displayed);
    }

    public GeneroFormPageObject PreencherNome(string nome)
    {
        var inputNome = driver?.FindElement(By.Id("Nome"));
        inputNome?.Clear();
        inputNome?.SendKeys(nome);

        return this;
    }

    public GeneroIndexPageObject Confirmar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']"))).Click();

        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return new GeneroIndexPageObject(driver!);
    }
}