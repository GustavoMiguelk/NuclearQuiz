namespace NuclearQuiz.Models;

public class Pergunta
{
    public int Id { get; set; }
    public string Enunciado { get; set; } = string.Empty;
    public string OpcaoA { get; set; } = string.Empty;
    public string OpcaoB { get; set; } = string.Empty;
    public string OpcaoC { get; set; } = string.Empty;
    public string? OpcaoD { get; set; }
    public string RespostaCorreta { get; set; } = string.Empty;
    public int ConteudoId { get; set; }
    public Conteudo Conteudo { get; set; } = null!;
}
