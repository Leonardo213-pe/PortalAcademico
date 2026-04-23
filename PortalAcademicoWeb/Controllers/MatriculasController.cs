using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademicoWeb.Data;
using PortalAcademicoWeb.Models;

namespace PortalAcademicoWeb.Controllers;

[Authorize] // Solo usuarios autenticados
public class MatriculasController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public MatriculasController(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // POST: /Matriculas/Inscribir
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inscribir(int cursoId)
    {
        var userId = _userManager.GetUserId(User)!;

        // Obtener el curso con sus matrículas
        var curso = await _db.Cursos
            .Include(c => c.Matriculas)
            .FirstOrDefaultAsync(c => c.Id == cursoId && c.Activo);

        // Validación 1: el curso existe
        if (curso == null)
        {
            TempData["Error"] = "El curso no existe o no está activo.";
            return RedirectToAction("Index", "Cursos");
        }

        // Validación 2: ya está inscrito en este curso
        var yaInscrito = await _db.Matriculas.AnyAsync(m =>
            m.CursoId == cursoId &&
            m.UsuarioId == userId &&
            m.Estado != EstadoMatricula.Cancelada);

        if (yaInscrito)
        {
            TempData["Error"] = "Ya estás inscrito en este curso.";
            return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
        }

        // Validación 3: no exceder el cupo máximo
        var inscritos = curso.Matriculas
            .Count(m => m.Estado != EstadoMatricula.Cancelada);

        if (inscritos >= curso.CupoMaximo)
        {
            TempData["Error"] = "El curso no tiene cupos disponibles.";
            return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
        }

        // Validación 4: conflicto de horario con otro curso matriculado
        var misMatriculas = await _db.Matriculas
            .Include(m => m.Curso)
            .Where(m =>
                m.UsuarioId == userId &&
                m.Estado != EstadoMatricula.Cancelada)
            .ToListAsync();

        var conflicto = misMatriculas.FirstOrDefault(m =>
            m.Curso.HorarioInicio < curso.HorarioFin &&
            m.Curso.HorarioFin > curso.HorarioInicio);

        if (conflicto != null)
        {
            TempData["Error"] =
                $"Conflicto de horario con '{conflicto.Curso.Nombre}' " +
                $"({conflicto.Curso.HorarioInicio:HH:mm}" +
                $"–{conflicto.Curso.HorarioFin:HH:mm}).";
            return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
        }

        // Todo OK: crear matrícula en estado Pendiente
        var matricula = new Matricula
        {
            CursoId = cursoId,
            UsuarioId = userId,
            Estado = EstadoMatricula.Pendiente,
            FechaRegistro = DateTime.UtcNow
        };

        _db.Matriculas.Add(matricula);
        await _db.SaveChangesAsync();

        TempData["Exito"] =
            $"Te inscribiste correctamente en '{curso.Nombre}'. " +
            $"Estado: Pendiente.";

        return RedirectToAction("MisMatriculas");
    }

    // GET: /Matriculas/MisMatriculas
    public async Task<IActionResult> MisMatriculas()
    {
        var userId = _userManager.GetUserId(User)!;

        var matriculas = await _db.Matriculas
            .Include(m => m.Curso)
            .Where(m => m.UsuarioId == userId)
            .OrderByDescending(m => m.FechaRegistro)
            .ToListAsync();

        return View(matriculas);
    }
}