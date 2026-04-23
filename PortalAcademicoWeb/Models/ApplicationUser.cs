using Microsoft.AspNetCore.Identity;

namespace PortalAcademicoWeb.Models;

public class ApplicationUser : IdentityUser
{
    public string NombreCompleto { get; set; } = string.Empty;
    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}