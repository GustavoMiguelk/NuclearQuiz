using NuclearQuiz.Exceptions;
using NuclearQuiz.Models;
using NuclearQuiz.Repositories;
using NuclearQuiz.Utils;

namespace NuclearQuiz.Services;

public class PerguntaService
{
    private readonly PerguntaRepository _repositorio;

    public PerguntaService(PerguntaRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public List<Conteudo> ListarConteudos()
    {
        return _repositorio.ListarConteudos();
    }

    public Pergunta Cadastrar(Pergunta nova)
    {
        nova.Normalizar();
        nova.Validar();

        if (!_repositorio.ExisteConteudo(nova.ConteudoId))
        {
            throw new RegraNegocioException($"O conteudo {nova.ConteudoId} nao existe.");
        }

        var enunciadoNormalizado = Texto.Normalizar(nova.Enunciado);
        var perguntaDuplicada = _repositorio
            .ListarPorConteudo(nova.ConteudoId)
            .Any(pergunta => Texto.Normalizar(pergunta.Enunciado) == enunciadoNormalizado);

        if (perguntaDuplicada)
        {
            throw new RegraNegocioException(
                "Ja existe uma pergunta com esse enunciado nesse conteudo.");
        }

        _repositorio.Adicionar(nova);
        return nova;
    }
}
