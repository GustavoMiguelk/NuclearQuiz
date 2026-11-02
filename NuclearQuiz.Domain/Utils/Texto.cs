using System.Globalization;
using System.Text;

namespace NuclearQuiz.Utils;

public static class Texto
{
    public static string Normalizar(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return string.Empty;
        }

        var textoNormalizado = valor.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var textoSemAcentos = new StringBuilder();

        foreach (var caractere in textoNormalizado)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(caractere) != UnicodeCategory.NonSpacingMark)
            {
                textoSemAcentos.Append(caractere);
            }
        }

        return textoSemAcentos.ToString().Normalize(NormalizationForm.FormC);
    }
}
