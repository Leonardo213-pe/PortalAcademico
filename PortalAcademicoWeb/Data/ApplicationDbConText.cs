using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortalAcademicoWeb.Models;

namespace PortalAcademicoWeb.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Matricula> Matriculas => Set<Matricula>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Código único
        builder.Entity<Curso>()
            .HasIndex(c => c.Codigo)
            .IsUnique();

        // Un usuario no puede matricularse dos veces en el mismo curso
        builder.Entity<Matricula>()
            .HasIndex(m => new { m.CursoId, m.UsuarioId })
            .IsUnique();

        // Restricción DB: HorarioFin > HorarioInicio
        builder.Entity<Curso>().ToTable(tb =>
            tb.HasCheckConstraint("CK_Curso_Horario", "\"HorarioFin\" > \"HorarioInicio\""));

        // Restricción DB: Creditos > 0
        builder.Entity<Curso>().ToTable(tb =>
            tb.HasCheckConstraint("CK_Curso_Creditos", "\"Creditos\" > 0"));

        // Seed: 3 cursos activos
        builder.Entity<Curso>().HasData(
            new Curso
            {
                Id = 1, Codigo = "CS101", Nombre = "Introducción a Programación",
                Creditos = 4, CupoMaximo = 30,
                HorarioInicio = new TimeOnly(8, 0), HorarioFin = new TimeOnly(10, 0), Activo = true
            },
            new Curso
            {
                Id = 2, Codigo = "MAT201", Nombre = "Cálculo Diferencial",
                Creditos = 3, CupoMaximo = 25,
                HorarioInicio = new TimeOnly(10, 0), HorarioFin = new TimeOnly(12, 0), Activo = true
            },
            new Curso
            {
                Id = 3, Codigo = "FIS101", Nombre = "Física Mecánica",
                Creditos = 5, CupoMaximo = 20,
                HorarioInicio = new TimeOnly(14, 0), HorarioFin = new TimeOnly(17, 0), Activo = true
            }
        );
    }
}