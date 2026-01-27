using AcademiaLounge.Models;
using Microsoft.EntityFrameworkCore;

namespace AcademiaLounge.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Aluno> Alunos => Set<Aluno>();
    public DbSet<Plano> Planos => Set<Plano>();
    public DbSet<Assinatura> Assinaturas { get; set; } = default!;
    public DbSet<Usuario> Usuarios { get; set; } = default!;

    public DbSet<Pagamento> Pagamentos { get; set; } = default!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ALUNO (seu mapping atual)
        modelBuilder.Entity<Aluno>(e =>
        {
            e.ToTable("alunos");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(120).IsRequired();
            e.Property(x => x.Telefone).HasColumnName("telefone").HasMaxLength(30);
            e.Property(x => x.Email).HasColumnName("email").HasMaxLength(180);
            e.Property(x => x.Documento).HasColumnName("documento").HasMaxLength(30);
            e.Property(x => x.Observacoes).HasColumnName("observacoes");
            e.Property(x => x.Status).HasColumnName("status");
            e.Property(x => x.CriadoEm).HasColumnName("criado_em");
            e.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");

            e.HasIndex(x => x.Nome);
            e.HasIndex(x => x.Telefone);
            e.HasIndex(x => x.Status);
        });

        // PLANO
        modelBuilder.Entity<Plano>(e =>
        {
            e.ToTable("planos");
            e.HasKey(x => x.Id);

            e.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(80).IsRequired();
            e.Property(x => x.Descricao).HasColumnName("descricao");
            e.Property(x => x.DuracaoDias).HasColumnName("duracao_dias").IsRequired();
            e.Property(x => x.Valor).HasColumnName("valor").HasPrecision(12, 2).IsRequired();
            e.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

            e.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
            e.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em").IsRequired();

            e.HasIndex(x => x.Nome);
            e.HasIndex(x => x.Ativo);
        });

        modelBuilder.Entity<Assinatura>(e =>
        {
            e.ToTable("assinaturas");
            e.HasKey(x => x.Id);

            e.Property(x => x.AlunoId).HasColumnName("aluno_id").IsRequired();
            e.Property(x => x.PlanoId).HasColumnName("plano_id").IsRequired();

            e.Property(x => x.DataInicio).HasColumnName("data_inicio").IsRequired();
            e.Property(x => x.DataVencimento).HasColumnName("data_vencimento").IsRequired();

            e.Property(x => x.Status).HasColumnName("status").IsRequired();
            e.Property(x => x.Observacoes).HasColumnName("observacoes");

            e.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
            e.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em").IsRequired();

            e.HasIndex(x => x.AlunoId);
            e.HasIndex(x => x.PlanoId);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.DataVencimento);

            e.HasOne(x => x.Aluno)
                .WithMany()
                .HasForeignKey(x => x.AlunoId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Plano)
                .WithMany()
                .HasForeignKey(x => x.PlanoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("usuarios");
            e.HasKey(x => x.Id);

            e.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(120).IsRequired();
            e.Property(x => x.Email).HasColumnName("email").HasMaxLength(180);
            e.Property(x => x.Telefone).HasColumnName("telefone").HasMaxLength(30);

            e.Property(x => x.Login).HasColumnName("login").HasMaxLength(80).IsRequired();
            e.Property(x => x.SenhaHash).HasColumnName("senha_hash").IsRequired();

            e.Property(x => x.Perfil).HasColumnName("perfil").IsRequired();
            e.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

            e.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
            e.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em").IsRequired();

            e.HasIndex(x => x.Login).IsUnique();
        });

        modelBuilder.Entity<Pagamento>(e =>
        {
            e.ToTable("pagamentos");
            e.HasKey(x => x.Id);

            e.Property(x => x.AssinaturaId).HasColumnName("assinatura_id").IsRequired();
            e.Property(x => x.Competencia).HasColumnName("competencia").HasMaxLength(7).IsRequired();
            e.Property(x => x.Valor).HasColumnName("valor").HasPrecision(12, 2).IsRequired();
            e.Property(x => x.Forma).HasColumnName("forma").IsRequired();
            e.Property(x => x.Status).HasColumnName("status").IsRequired();
            e.Property(x => x.DataPagamento).HasColumnName("data_pagamento");
            e.Property(x => x.Observacoes).HasColumnName("observacoes");
            e.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
            e.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em").IsRequired();

            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.Competencia);
            e.HasIndex(x => x.AssinaturaId);

            // Evita duplicar pagamento para mesma assinatura+competência
            e.HasIndex(x => new { x.AssinaturaId, x.Competencia }).IsUnique();

            e.HasOne(x => x.Assinatura)
                .WithMany()
                .HasForeignKey(x => x.AssinaturaId)
                .OnDelete(DeleteBehavior.Restrict);
        });


    }
}
