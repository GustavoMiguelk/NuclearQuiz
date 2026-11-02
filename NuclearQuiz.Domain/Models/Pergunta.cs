using NuclearQuiz.Exceptions;
using NuclearQuiz.Utils;

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

    public List<(string Letra, string Texto)> ObterAlternativas()
    {
        var alternativas = new List<(string Letra, string Texto)>();

        if (!string.IsNullOrWhiteSpace(OpcaoA))
        {
            alternativas.Add(("A", OpcaoA.Trim()));
        }

        if (!string.IsNullOrWhiteSpace(OpcaoB))
        {
            alternativas.Add(("B", OpcaoB.Trim()));
        }

        if (!string.IsNullOrWhiteSpace(OpcaoC))
        {
            alternativas.Add(("C", OpcaoC.Trim()));
        }

        if (!string.IsNullOrWhiteSpace(OpcaoD))
        {
            alternativas.Add(("D", OpcaoD.Trim()));
        }

        return alternativas;
    }

    public void Normalizar()
    {
        Enunciado = Enunciado?.Trim() ?? string.Empty;
        OpcaoA = OpcaoA?.Trim() ?? string.Empty;
        OpcaoB = OpcaoB?.Trim() ?? string.Empty;
        OpcaoC = OpcaoC?.Trim() ?? string.Empty;
        OpcaoD = string.IsNullOrWhiteSpace(OpcaoD) ? null : OpcaoD.Trim();
        RespostaCorreta = RespostaCorreta?.Trim().ToUpperInvariant() ?? string.Empty;
    }

    public void Validar()
    {
        var erros = new List<string>();
        var enunciado = Enunciado?.Trim() ?? string.Empty;
        var opcaoA = OpcaoA?.Trim() ?? string.Empty;
        var opcaoB = OpcaoB?.Trim() ?? string.Empty;
        var opcaoC = OpcaoC?.Trim() ?? string.Empty;
        var opcaoD = string.IsNullOrWhiteSpace(OpcaoD) ? null : OpcaoD.Trim();
        var respostaCorreta = RespostaCorreta?.Trim().ToUpperInvariant() ?? string.Empty;

        if (enunciado.Length is < 10 or > 250)
        {
            erros.Add("O enunciado deve ter entre 10 e 250 caracteres.");
        }

        ValidarAlternativa("A", opcaoA, true, erros);
        ValidarAlternativa("B", opcaoB, true, erros);
        ValidarAlternativa("C", opcaoC, true, erros);
        ValidarAlternativa("D", opcaoD, false, erros);

        var alternativas = ObterAlternativas();
        if (alternativas
            .Select(alternativa => Texto.Normalizar(alternativa.Texto))
            .GroupBy(texto => texto)
            .Any(grupo => grupo.Count() > 1))
        {
            erros.Add("As alternativas nao podem ser repetidas.");
        }

        if (respostaCorreta is not ("A" or "B" or "C" or "D"))
        {
            erros.Add("A resposta correta deve ser A, B, C ou D.");
        }
        else if (respostaCorreta == "D" && opcaoD is null)
        {
            erros.Add("A resposta correta e D, mas a alternativa D nao foi informada.");
        }

        if (ConteudoId <= 0)
        {
            erros.Add("Informe um conteudo valido.");
        }

        if (erros.Count > 0)
        {
            throw new ValidacaoException(erros);
        }
    }

    private static void ValidarAlternativa(
        string letra,
        string? texto,
        bool obrigatoria,
        List<string> erros)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            if (obrigatoria)
            {
                erros.Add($"A alternativa {letra} e obrigatoria.");
            }

            return;
        }

        if (texto.Length > 120)
        {
            erros.Add($"A alternativa {letra} deve ter no maximo 120 caracteres.");
        }
    }
}
