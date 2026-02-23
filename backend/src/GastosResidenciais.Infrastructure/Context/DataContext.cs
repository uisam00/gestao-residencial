using GastosResidenciais.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.Infrastructure.Context;

/// <summary>
/// Contexto principal de acesso ao banco de dados usando EF Core.
/// Expõe os DbSets de cada entidade e configura as regras de mapeamento.
/// </summary>
public class DataContext : DbContext, IDataContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Person> People => Set<Person>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasMany(p => p.Transactions)
                .WithOne(t => t.Person!)
                .HasForeignKey(t => t.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(400);

            entity.Property(c => c.Purpose)
                .IsRequired();
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(400);

            entity.Property(t => t.Amount)
                .IsRequired();

            entity.Property(t => t.Type)
                .IsRequired();

            entity.HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);
            entity.HasIndex(u => u.Username)
                .IsUnique();
        });
    }
}
