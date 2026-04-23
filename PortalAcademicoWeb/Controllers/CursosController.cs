using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademicoWeb.Data;
using PortalAcademicoWeb.Models;

namespace PortalAcademicoWeb.Controllers;

public class CursosController : Controller
{
    private readonly ApplicationDbContext _db;

    public CursosController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET: /Cursos
    public async Task<IActionResult> Index(
        string? nombre,
        int? creditosMin,
        int? creditosMax,
        string? horarioDesde,
        string? horarioHasta)
    {
        var query = _db.Cursos
            .Include(c => c.Matriculas)
            .Where(c => c.Activo)
            .AsQueryable();

        // Filtro por nombre
        if (!string.IsNullOrEmpty(nombre))
            query = query.Where(c => c.Nombre.Contains(nombre));

        // Filtro por créditos
        if (creditosMin.HasValue)
            query = query.Where(c => c.Creditos >= creditosMin.Value);

        if (creditosMax.HasValue)
            query = query.Where(c => c.Creditos <= creditosMax.Value);

        // Filtro por horario
        if (!string.IsNullOrEmpty(horarioDesde) &&
            TimeOnly.TryParse(horarioDesde, out var desde))
            query = query.Where(c => c.HorarioInicio >= desde);

        if (!string.IsNullOrEmpty(horarioHasta) &&
            TimeOnly.TryParse(horarioHasta, out var hasta))
            query = query.Where(c => c.HorarioFin <= hasta);

        // Guardar filtros en ViewBag para mantenerlos en el form
        ViewBag.Nombre = nombre;
        ViewBag.CreditosMin = creditosMin;
        ViewBag.CreditosMax = creditosMax;
        ViewBag.HorarioDesde = horarioDesde;
        ViewBag.HorarioHasta = horarioHasta;

        var cursos = await query.ToListAsync();
        return View(cursos);
    }

    // GET: /Cursos/Detalle/5
    public async Task<IActionResult> Detalle(int id)
    {
        var curso = await _db.Cursos
            .Include(c => c.Matriculas)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (curso == null)
            return NotFound();

        // Guardar último curso visitado en sesión (P4)
        HttpContext.Session.SetString("UltimoCursoId", id.ToString());
        HttpContext.Session.SetString("UltimoCursoNombre", curso.Nombre);

        return View(curso);
    }
}