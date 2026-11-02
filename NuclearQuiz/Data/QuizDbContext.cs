using Microsoft.EntityFrameworkCore;
using NuclearQuiz.Models;

namespace NuclearQuiz.Data;

public class QuizDbContext(DbContextOptions<QuizDbContext> options) : DbContext(options)
{
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Conteudo> Conteudos => Set<Conteudo>();
    public DbSet<Pergunta> Perguntas => Set<Pergunta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("CATEGORIA");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("ID").ValueGeneratedNever();
            entity.Property(x => x.Nome).HasColumnName("NOME");
            entity.Property(x => x.Descricao).HasColumnName("DESCRICAO");
        });

        modelBuilder.Entity<Conteudo>(entity =>
        {
            entity.ToTable("CONTEUDO");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("ID").ValueGeneratedNever();
            entity.Property(x => x.Titulo).HasColumnName("TITULO");
            entity.Property(x => x.Nivel).HasColumnName("NIVEL");
            entity.Property(x => x.CategoriaId).HasColumnName("CATEGORIA_ID");
            entity.HasOne(x => x.Categoria)
                .WithMany()
                .HasForeignKey(x => x.CategoriaId);
        });

        modelBuilder.Entity<Pergunta>(entity =>
        {
            entity.ToTable("PERGUNTA");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("ID").ValueGeneratedNever();
            entity.Property(x => x.Enunciado).HasColumnName("ENUNCIADO");
            entity.Property(x => x.OpcaoA).HasColumnName("OPCAO_A");
            entity.Property(x => x.OpcaoB).HasColumnName("OPCAO_B");
            entity.Property(x => x.OpcaoC).HasColumnName("OPCAO_C");
            entity.Property(x => x.OpcaoD).HasColumnName("OPCAO_D");
            entity.Property(x => x.RespostaCorreta).HasColumnName("RESPOSTA_CORRETA");
            entity.Property(x => x.ConteudoId).HasColumnName("CONTEUDO_ID");
            entity.HasOne(x => x.Conteudo)
                .WithMany()
                .HasForeignKey(x => x.ConteudoId);
        });
    }
}
