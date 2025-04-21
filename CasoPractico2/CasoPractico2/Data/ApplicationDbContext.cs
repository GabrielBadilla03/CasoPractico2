using CasoPractico2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CasoPractico2.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "0A7AEBC7-A3BD-4369-AE34-8689052AF9A6",
                    Name = "Administrador",
                    NormalizedName = "ADMINISTRADOR",
                    ConcurrencyStamp = "3A4F1A4A-951F-4E18-ACF1-3B506C11E3FC"
                },
                new IdentityRole
                {
                    Id = "2EACDB14-CE5F-4B62-8F2D-D511E204A98B",
                    Name = "Usuario",
                    NormalizedName = "USUARIO",
                    ConcurrencyStamp = "4F05C542-DF4C-4A8C-A165-26C115A7E267"
                },
                new IdentityRole
                {
                    Id = "C971527E-B9FB-49A4-A748-84BEA4AB67B0",
                    Name = "Organizador",
                    NormalizedName = "ORGANIZADOR",
                    ConcurrencyStamp = "9842DF61-EDCF-4543-876C-CEB19F2ABB81"
                }
            );

            builder.Entity<Categoria>()
                .HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Evento>()
                .HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Inscripcion>()
                .HasOne(i => i.Usuario)
                .WithMany()
                .HasForeignKey(i => i.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Inscripcion>()
                .HasOne(i => i.Evento)
                .WithMany(e => e.Inscripciones)
                .HasForeignKey(i => i.EventoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}





/*prueba errores*/