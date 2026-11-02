using NuclearQuiz.Models;

namespace NuclearQuiz.Services;

public static class QuizJogo
{
    public static void Jogar(List<Pergunta> perguntas)
    {
        if (perguntas.Count == 0)
        {
            Console.WriteLine("Nao ha perguntas cadastradas.");
            return;
        }

        var acertos = 0;

        for (var i = 0; i < perguntas.Count; i++)
        {
            var pergunta = perguntas[i];
            var alternativas = pergunta.ObterAlternativas();

            Console.WriteLine();
            Console.WriteLine(
                $"Pergunta {i + 1}/{perguntas.Count} [{pergunta.Conteudo.Categoria.Nome}]");
            Console.WriteLine(pergunta.Enunciado);

            foreach (var alternativa in alternativas)
            {
                Console.WriteLine($"  {alternativa.Letra}) {alternativa.Texto}");
            }

            string resposta;
            do
            {
                Console.Write("Sua resposta: ");
                resposta = (Console.ReadLine() ?? string.Empty).Trim().ToUpperInvariant();

                if (!alternativas.Any(alternativa => alternativa.Letra == resposta))
                {
                    Console.WriteLine("Digite uma das opcoes: A, B, C");
                }
            }
            while (!alternativas.Any(alternativa => alternativa.Letra == resposta));

            if (resposta == pergunta.RespostaCorreta)
            {
                acertos++;
                Console.WriteLine("Correto!");
            }
            else
            {
                Console.WriteLine($"Errado. A resposta certa era {pergunta.RespostaCorreta}.");
            }
        }

        Console.WriteLine();
        Console.WriteLine($"Resultado: {acertos}/{perguntas.Count} acertos.");
    }
}
