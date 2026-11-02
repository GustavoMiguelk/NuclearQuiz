using Microsoft.EntityFrameworkCore;
using NuclearQuiz.Data;
using NuclearQuiz.Models;

namespace NuclearQuiz.Repositories;

public class PerguntaRepository
{
    private readonly QuizDbContext _db;

    public PerguntaRepository(QuizDbContext db)
    {
        _db = db;
    }

    public List<Conteudo> ListarConteudos()
    {
        return _db.Conteudos
            .AsNoTracking()
            .Include(c => c.Categoria)
            .OrderBy(c => c.Id)
            .ToList();
    }

    public bool ExisteConteudo(int id)
    {
        return _db.Conteudos.Count(c => c.Id == id) > 0;
    }

    public List<Pergunta> ListarPorConteudo(int conteudoId)
    {
        return _db.Perguntas
            .AsNoTracking()
            .Where(p => p.ConteudoId == conteudoId)
            .ToList();
    }

    public List<Pergunta> ListarTodas()
    {
        return ConsultaBase()
            .OrderBy(p => p.Id)
            .ToList();
    }

    public List<Pergunta> BuscarPorCategoria(string termo)
    {
        return ConsultaBase()
            .Where(p => p.Conteudo.Categoria.Nome.ToLower().Contains(termo))
            .OrderBy(p => p.Id)
            .ToList();
    }

    public List<Pergunta> BuscarPorTexto(string termo)
    {
        return ConsultaBase()
            .Where(p => p.Enunciado.ToLower().Contains(termo))
            .OrderBy(p => p.Id)
            .ToList();
    }

    public void Adicionar(Pergunta pergunta)
    {
        try
        {
            pergunta.Id = ProximoId();
            _db.Perguntas.Add(pergunta);
            _db.SaveChanges();
        }
        catch
        {
            _db.Entry(pergunta).State = EntityState.Detached;
            throw;
        }
    }

    private int ProximoId()
    {
        _db.Database.OpenConnection();

        try
        {
            using var cmd = _db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "SELECT SEQ_PERGUNTA.NEXTVAL FROM DUAL";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
        finally
        {
            _db.Database.CloseConnection();
        }
    }

    private IQueryable<Pergunta> ConsultaBase()
    {
        return _db.Perguntas
            .AsNoTracking()
            .Include(p => p.Conteudo)
            .ThenInclude(c => c.Categoria);
    }
}
