using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vemo.Gestion.Recursos.Data.Entidades;

namespace Vemo.Gestion.Recursos.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuarios>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Modificar la fecha antes de guardar
            foreach (var entry in ChangeTracker.Entries<Usuarios>().Where(e => e.State == EntityState.Added))
            {
                var fecha = entry.Entity.FechaNacimiento;

                if (fecha.HasValue)
                    entry.Entity.FechaNacimiento = new DateTime(fecha.Value.Year, fecha.Value.Month, fecha.Value.Day);
            }

            
            return base.SaveChangesAsync(cancellationToken);
        }


        public DbSet<RecursosCompartidos> RecursosCompartidos { get; set; }
        public DbSet<Sesiones> Sesiones { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }
    }
}