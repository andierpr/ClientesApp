using Microsoft.EntityFrameworkCore;
using ClientesApp.Models;

namespace ClientesApp.Data
{
    public class ClientesContext : DbContext
    {
        public ClientesContext(DbContextOptions<ClientesContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Estado> Estados { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- ESTADOS ----------
            modelBuilder.Entity<Estado>(entity =>
            {
                entity.ToTable("Estados");

                entity.HasKey(e => e.IdEstado);

                entity.Property(e => e.IdEstado)
                      .HasColumnName("IdEstado");

                entity.Property(e => e.UF)
                      .HasMaxLength(2)
                      .IsRequired();

                entity.Property(e => e.NomeEstado)
                      .HasMaxLength(100)
                      .IsRequired();
            });

            // ---------- CLIENTES ----------
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Clientes");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.IdEstado)
                      .HasColumnName("IdEstado");

                entity.HasOne(c => c.Estado)
                      .WithMany(e => e.Clientes!)
                      .HasForeignKey(c => c.IdEstado)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
