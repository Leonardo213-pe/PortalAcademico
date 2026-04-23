using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalAcademicoWeb.Models;

public class Curso
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El código es obligatorio")]
    [StringLength(20)]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [Range(1, 20, ErrorMessage = "Los créditos deben ser mayores a 0")]
    public int Creditos { get; set; }

    [Range(1, 500, ErrorMessage = "El cupo debe ser mayor a 0")]
    public int CupoMaximo { get; set; }

    public TimeOnly HorarioInicio { get; set; }
    public TimeOnly HorarioFin { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

    [NotMapped]
    public int CuposDisponibles =>
        CupoMaximo - Matriculas.Count(m => m.Estado != EstadoMatricula.Cancelada);
}