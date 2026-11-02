namespace NuclearQuiz.Exceptions;

public class ValidacaoException(IReadOnlyList<string> erros)
    : Exception("Dados invalidos: " + string.Join(" | ", erros))
{
    public IReadOnlyList<string> Erros { get; } = erros;
}
