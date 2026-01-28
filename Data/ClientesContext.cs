using ClientesApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientesApp.Data
{
    public class ClientesContext : DbContext
    {
        public ClientesContext(DbContextOptions<ClientesContext> options)
            : base(options)
        {
        }

        // =====================
        // DbSets
        // =====================
        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Estado> Estados { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;

        // =====================
        // Fluent API
        // =====================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureEstado(modelBuilder);
            ConfigureCliente(modelBuilder);
        }

        // =====================
        // Configurações de Estado
        // =====================
        private static void ConfigureEstado(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Estado>(entity =>
            {
                entity.ToTable("Estados");

                entity.HasKey(e => e.IdEstado);

                entity.Property(e => e.IdEstado)
                      .HasColumnName("IdEstado");

                entity.Property(e => e.UF)
                      .IsRequired()
                      .HasMaxLength(2);

                entity.Property(e => e.NomeEstado)
                      .IsRequired()
                      .HasMaxLength(100);
            });
        }

        // =====================
        // Configurações de Cliente
        // =====================
        private static void ConfigureCliente(ModelBuilder modelBuilder)
        {
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
