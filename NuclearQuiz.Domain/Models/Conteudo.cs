namespace NuclearQuiz.Models;

public class Conteudo
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;
}
