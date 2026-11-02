using Microsoft.EntityFrameworkCore;
using NuclearQuiz.Data;
using NuclearQuiz.Exceptions;
using NuclearQuiz.Models;
using NuclearQuiz.Repositories;
using NuclearQuiz.Services;

var connectionString = Environment.GetEnvironmentVariable("ORACLE_CONNECTION_STRING");
if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("Defina a variavel de ambiente ORACLE_CONNECTION_STRING (veja o README).");
    return;
}

var options = new DbContextOptionsBuilder<QuizDbContext>()
    .UseOracle(connectionString)
    .Options;

using var db = new QuizDbContext(options);
var service = new PerguntaService(new PerguntaRepository(db));

while (true)
{
    Console.WriteLine("=== Nuclear no Dia a Dia - Banco de Perguntas ===");
    Console.WriteLine("1 - Cadastrar pergunta");
    Console.WriteLine("2 - Listar todas as perguntas");
    Console.WriteLine("3 - Buscar por categoria");
    Console.WriteLine("4 - Buscar por trecho do enunciado");
    Console.WriteLine("0 - Sair");
    Console.Write("Escolha: ");

    var opcao = Console.ReadLine()?.Trim();

    try
    {
        switch (opcao)
        {
            case "1":
                Cadastrar(service);
                break;
            case "2":
                Exibir(service.Listar());
                break;
            case "3":
                Exibir(service.BuscarPorCategoria(Ler("Categoria (ex.: saude): ")));
                break;
            case "4":
                Exibir(service.BuscarPorTexto(Ler("Trecho do enunciado: ")));
                break;
            case "0":
                Console.WriteLine("Ate logo!");
                return;
            default:
                Console.WriteLine("Opcao invalida.");
                break;
        }
    }
    catch (ValidacaoException exception)
    {
        Console.WriteLine("Nao foi possivel concluir:");
        foreach (var erro in exception.Erros)
        {
            Console.WriteLine($" - {erro}");
        }
    }
    catch (RegraNegocioException exception)
    {
        Console.WriteLine($"Regra de negocio: {exception.Message}");
    }
    catch (DbUpdateException exception)
    {
        Console.WriteLine(
            "Erro ao gravar no banco: " + (exception.InnerException?.Message ?? exception.Message));
    }
    catch (Exception exception)
    {
        Console.WriteLine(
            "Erro inesperado: " + (exception.InnerException?.Message ?? exception.Message));
    }

    Console.WriteLine();
}

static string Ler(string mensagem)
{
    Console.Write(mensagem);
    return Console.ReadLine() ?? string.Empty;
}

static int LerInteiro(string mensagem)
{
    if (!int.TryParse(Ler(mensagem), out var valor))
    {
        throw new ValidacaoException(["Digite um numero inteiro valido."]);
    }

    return valor;
}

static void Cadastrar(PerguntaService service)
{
    foreach (var conteudo in service.ListarConteudos())
    {
        Console.WriteLine($"{conteudo.Id} - {conteudo.Titulo} ({conteudo.Categoria.Nome})");
    }

    var pergunta = new Pergunta
    {
        ConteudoId = LerInteiro("Id do conteudo: "),
        Enunciado = Ler("Enunciado: "),
        OpcaoA = Ler("Alternativa A: "),
        OpcaoB = Ler("Alternativa B: "),
        OpcaoC = Ler("Alternativa C: "),
        OpcaoD = Ler("Alternativa D (Enter para deixar vazia): "),
        RespostaCorreta = Ler("Resposta correta (A/B/C/D): ")
    };

    service.Cadastrar(pergunta);
    Console.WriteLine($"Pergunta cadastrada com id {pergunta.Id}.");
}

static void Exibir(List<Pergunta> perguntas)
{
    if (perguntas.Count == 0)
    {
        Console.WriteLine("Nenhuma pergunta encontrada.");
        return;
    }

    foreach (var pergunta in perguntas)
    {
        Console.WriteLine($"[{pergunta.Id}] {pergunta.Enunciado}");
        Console.WriteLine(
            $"    Categoria: {pergunta.Conteudo.Categoria.Nome} | Conteudo: {pergunta.Conteudo.Titulo}");

        foreach (var alternativa in pergunta.ObterAlternativas())
        {
            Console.WriteLine($"    {alternativa.Letra}) {alternativa.Texto}");
        }

        Console.WriteLine($"    Correta: {pergunta.RespostaCorreta}");
    }

    Console.WriteLine();
    Console.WriteLine($"Total: {perguntas.Count}");
}
