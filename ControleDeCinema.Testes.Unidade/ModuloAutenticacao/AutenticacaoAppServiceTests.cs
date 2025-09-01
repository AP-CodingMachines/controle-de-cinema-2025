using ControleDeCinema.Aplicacao.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ControleDeCinema.Testes.Unidades.ModuloAutenticacao;

[TestClass]
[TestCategory("Testes de Unidade de Autenticação")]
public class AutenticacaoAppServiceTestes
{
    private AutenticacaoAppService autenticacaoAppService;

    private Mock<UserManager<Usuario>> userManagerMock;
    private Mock<SignInManager<Usuario>> signInManagerMock;
    private Mock<RoleManager<Cargo>> roleManagerMock;

    [TestInitialize]
    public void Setup()
    {
        userManagerMock = new Mock<UserManager<Usuario>>(
            new Mock<IUserStore<Usuario>>().Object, null!, null!, null!,
            null!, null!, null!, null!, null!
        );

        signInManagerMock = new Mock<SignInManager<Usuario>>(
            userManagerMock.Object, new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<Usuario>>().Object, null!,
            null!, null!
        );

        roleManagerMock = new Mock<RoleManager<Cargo>>(
            new Mock<IRoleStore<Cargo>>().Object, null!, null!, null!, null!
        );

        autenticacaoAppService = new AutenticacaoAppService(
            userManagerMock.Object,
            signInManagerMock.Object,
            roleManagerMock.Object
        );
    }

    [TestMethod]
    public async Task Deve_RetornarSucesso_QuandoRegistrarUsuarioValido()
    {
        var usuario = new Usuario { UserName = "teste@teste.com", Email = "teste@teste.com" };
        var senha = "Teste123!";
        var tipoUsuario = TipoUsuario.Cliente;
        var cargo = new Cargo { Name = tipoUsuario.ToString(), NormalizedName = tipoUsuario.ToString().ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString() };

        userManagerMock.Setup(u => u.CreateAsync(usuario, senha)).ReturnsAsync(IdentityResult.Success);
        roleManagerMock.Setup(r => r.FindByNameAsync(tipoUsuario.ToString())).ReturnsAsync(cargo);
        userManagerMock.Setup(u => u.AddToRoleAsync(usuario, tipoUsuario.ToString())).ReturnsAsync(IdentityResult.Success);
        signInManagerMock.Setup(s => s.PasswordSignInAsync(usuario.Email, senha, true, false)).ReturnsAsync(SignInResult.Success);

        var resultado = await autenticacaoAppService.RegistrarAsync(usuario, senha, tipoUsuario);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public async Task Deve_RetornarFalha_QuandoUsuarioForDuplicado()
    {
        var usuario = new Usuario { UserName = "teste@teste.com", Email = "outro@teste.com" };
        var senha = "Teste123!";
        var tipoUsuario = TipoUsuario.Cliente;

        userManagerMock.Setup(u => u.CreateAsync(usuario, senha))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "DuplicateUserName" }));

        var resultado = await autenticacaoAppService.RegistrarAsync(usuario, senha, tipoUsuario);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public async Task Deve_RetornarFalha_QuandoEmailForDuplicado()
    {
        var usuario = new Usuario { UserName = "outroUser", Email = "teste@teste.com" };
        var senha = "Teste123!";
        var tipoUsuario = TipoUsuario.Cliente;

        userManagerMock.Setup(u => u.CreateAsync(usuario, senha))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "DuplicateEmail" }));

        var resultado = await autenticacaoAppService.RegistrarAsync(usuario, senha, tipoUsuario);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public async Task Deve_RetornarFalha_QuandoSenhaForInvalida()
    {
        var usuario = new Usuario { UserName = "outroUser", Email = "teste@teste.com" };
        var senha = "123";
        var tipoUsuario = TipoUsuario.Cliente;

        var erros = new[]
        {
            new IdentityError { Code = "PasswordTooShort" },
            new IdentityError { Code = "PasswordRequiresDigit" }
        };

        userManagerMock.Setup(u => u.CreateAsync(usuario, senha))
            .ReturnsAsync(IdentityResult.Failed(erros));

        var resultado = await autenticacaoAppService.RegistrarAsync(usuario, senha, tipoUsuario);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public async Task Deve_RetornarSucesso_QuandoCriarCargoEAtribuirAoUsuario()
    {
        var usuario = new Usuario { UserName = "teste@teste.com", Email = "teste@teste.com" };
        var senha = "Teste123!";
        var tipoUsuario = TipoUsuario.Cliente;

        userManagerMock.Setup(u => u.CreateAsync(usuario, senha)).ReturnsAsync(IdentityResult.Success);
        roleManagerMock.Setup(r => r.FindByNameAsync(tipoUsuario.ToString())).ReturnsAsync((Cargo?)null!);
        roleManagerMock.Setup(r => r.CreateAsync(It.IsAny<Cargo>())).ReturnsAsync(IdentityResult.Success);
        userManagerMock.Setup(u => u.AddToRoleAsync(usuario, tipoUsuario.ToString())).ReturnsAsync(IdentityResult.Success);
        signInManagerMock.Setup(s => s.PasswordSignInAsync(usuario.Email, senha, true, false)).ReturnsAsync(SignInResult.Success);

        var resultado = await autenticacaoAppService.RegistrarAsync(usuario, senha, tipoUsuario);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public async Task Deve_RetornarSucesso_QuandoChamarLoginAposRegistrar()
    {
        var usuario = new Usuario { UserName = "teste@teste.com", Email = "teste@teste.com" };
        var senha = "Teste123!";
        var tipoUsuario = TipoUsuario.Cliente;
        var cargo = new Cargo { Name = tipoUsuario.ToString(), NormalizedName = tipoUsuario.ToString().ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString() };

        userManagerMock.Setup(u => u.CreateAsync(usuario, senha)).ReturnsAsync(IdentityResult.Success);
        roleManagerMock.Setup(r => r.FindByNameAsync(tipoUsuario.ToString())).ReturnsAsync(cargo);
        userManagerMock.Setup(u => u.AddToRoleAsync(usuario, tipoUsuario.ToString())).ReturnsAsync(IdentityResult.Success);
        signInManagerMock.Setup(s => s.PasswordSignInAsync(usuario.Email, senha, true, false)).ReturnsAsync(SignInResult.Success);

        var resultado = await autenticacaoAppService.RegistrarAsync(usuario, senha, tipoUsuario);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public async Task Deve_RetornarSucesso_QuandoLoginForValido()
    {
        var email = "teste@teste.com";
        var senha = "Teste123!";

        signInManagerMock.Setup(p => p.PasswordSignInAsync(email, senha, true, false)).ReturnsAsync(SignInResult.Success);

        var resultado = await autenticacaoAppService.LoginAsync(email, senha);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public async Task Deve_RetornarFalha_QuandoContaEstiverBloqueada()
    {
        var email = "teste@teste.com";
        var senha = "Teste123!";

        signInManagerMock.Setup(p => p.PasswordSignInAsync(email, senha, true, false)).ReturnsAsync(SignInResult.LockedOut);

        var resultado = await autenticacaoAppService.LoginAsync(email, senha);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public async Task Deve_RetornarFalha_QuandoLoginNaoForPermitido()
    {
        var email = "teste@teste.com";
        var senha = "Teste123!";

        signInManagerMock.Setup(p => p.PasswordSignInAsync(email, senha, true, false)).ReturnsAsync(SignInResult.NotAllowed);

        var resultado = await autenticacaoAppService.LoginAsync(email, senha);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public async Task Deve_RetornarFalha_QuandoRequererDoisFatores()
    {
        var email = "teste@teste.com";
        var senha = "Teste123!";

        signInManagerMock.Setup(p => p.PasswordSignInAsync(email, senha, true, false)).ReturnsAsync(SignInResult.TwoFactorRequired);

        var resultado = await autenticacaoAppService.LoginAsync(email, senha);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public async Task Deve_RetornarFalha_QuandoCredenciaisForemInvalidas()
    {
        var email = "teste@teste.com";
        var senha = "Teste123!";

        signInManagerMock.Setup(p => p.PasswordSignInAsync(email, senha, true, false)).ReturnsAsync(SignInResult.Failed);

        var resultado = await autenticacaoAppService.LoginAsync(email, senha);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public async Task Deve_RetornarSucesso_QuandoLogoutForExecutado()
    {
        signInManagerMock.Setup(p => p.SignOutAsync()).Returns(Task.CompletedTask);

        var resultado = await autenticacaoAppService.LogoutAsync();

        signInManagerMock.Verify(s => s.SignOutAsync(), Times.Once);
        Assert.IsTrue(resultado.IsSuccess);
    }
}
