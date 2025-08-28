using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Dominio.ModuloFilme;
using System.Diagnostics.CodeAnalysis;

namespace ControleDeCinema.Dominio.ModuloGeneroFilme;

public class GeneroFilme : EntidadeBase<GeneroFilme>
{
    public string Descricao { get; set; }
    public List<Filme> Filmes { get; set; }


    [ExcludeFromCodeCoverage]
    protected GeneroFilme()
    {
        Filmes = new List<Filme>();
    }

    public GeneroFilme(string descricao) : this()
    {
        Id = Guid.NewGuid();
        Descricao = descricao;
    }

    public void AdicionarFilme(Filme filme)
    {
        if (Filmes.Contains(filme))
            return;

        Filmes.Add(filme);
    }

    public override void AtualizarRegistro(GeneroFilme registroEditado)
    {
        Descricao = registroEditado.Descricao;
    }
}
