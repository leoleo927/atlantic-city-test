using Microsoft.EntityFrameworkCore;
using OrderManagement.API.Domain.Entities;

namespace OrderManagement.API.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Pedido
        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroPedido)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Cliente)
                .IsRequired()
                .HasMaxLength(150);
            entity.Property(e => e.FechaCreacion)
                .IsRequired();
            entity.Property(e => e.FechaModificacion)
                .IsRequired(false);
            entity.Property(e => e.Total)
                .HasColumnType("decimal(10,2)");
            entity.Property(e => e.Estado)
                .IsRequired()
                .HasMaxLength(50);
        });

        // Configuración de Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
            entity.HasIndex(e => e.Email)
                .IsUnique();
            entity.Property(e => e.PasswordHash)
                .IsRequired();
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Rol)
                .HasMaxLength(20);
        });

        // Datos semilla para pruebas
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = 1,
                Email = "admin@test.com",
                // Password: "Admin123!" (hasheado con BCrypt)
                PasswordHash = "$2a$11$Z9JUJ/9x4Q7K2qLHJQW0iu8yxQ7VZ4Z5JvJ5J5J5J5J5J5J5J5J5J",
                Nombre = "Administrador",
                Rol = "Admin",
                FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
