using Differ.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Differ.Data;

public class DiffContext(DbContextOptions<DiffContext>options):DbContext(options)
{
    public DbSet<DiffEntity> DiffEntities { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DiffEntity>(entity =>
        {
            entity.ToTable("Diff");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key)
                .IsRequired()
                .HasMaxLength(200);
            entity.HasIndex(e => e.Key)
                .IsUnique();
            entity.Property(e => e.Right).HasMaxLength(2000);
            entity.Property(e => e.Left).HasMaxLength(2000);
        });
    }
}