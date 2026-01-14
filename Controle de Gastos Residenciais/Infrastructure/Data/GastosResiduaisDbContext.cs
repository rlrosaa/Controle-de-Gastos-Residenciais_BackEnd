using Microsoft.EntityFrameworkCore;
using ControleGastosResiduais.Domain.Entities;

namespace ControleGastosResiduais.Infrastructure.Data;

/// <summary>
/// Contexto do Entity Framework Core para o sistema de gastos residenciais.
/// </summary>
public class GastosResiduaisDbContext : DbContext
{
    public GastosResiduaisDbContext(DbContextOptions<GastosResiduaisDbContext> options)
        : base(options)
    {
    }

    public DbSet<Pessoa> Pessoas => Set<Pessoa>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração de Pessoa
        modelBuilder.Entity<Pessoa>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(p => p.Idade)
                .IsRequired();

            // Relacionamento com Transacao (cascade delete)
            entity.HasMany(p => p.Transacoes)
                .WithOne(t => t.Pessoa)
                .HasForeignKey(t => t.PessoaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração de Categoria
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Descricao)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(c => c.Finalidade)
                .IsRequired();

            // Relacionamento com Transacao (restrict delete)
            entity.HasMany(c => c.Transacoes)
                .WithOne(t => t.Categoria)
                .HasForeignKey(t => t.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração de Transacao
        modelBuilder.Entity<Transacao>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Descricao)
                .IsRequired()
                .HasMaxLength(300);
            entity.Property(t => t.Valor)
                .IsRequired()
                .HasPrecision(18, 2);
            entity.Property(t => t.Tipo)
                .IsRequired();
            entity.Property(t => t.PessoaId)
                .IsRequired();
            entity.Property(t => t.CategoriaId)
                .IsRequired();

            // Índices para melhor performance
            entity.HasIndex(t => t.PessoaId);
            entity.HasIndex(t => t.CategoriaId);
        });
    }
}
