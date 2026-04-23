using System.ComponentModel.DataAnnotations;

namespace PortalAcademicoWeb.Models;

public enum EstadoMatricula { Pendiente, Confirmada, Cancelada }

public class Matricula
{
    public int Id { get; set; }

    public int CursoId { get; set; }
    public Curso Curso { get; set; } = null!;

    [Required]
    public string UsuarioId { get; set; } = string.Empty;
    public ApplicationUser Usuario { get; set; } = null!;

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public EstadoMatricula Estado { get; set; } = EstadoMatricula.Pendiente;
}