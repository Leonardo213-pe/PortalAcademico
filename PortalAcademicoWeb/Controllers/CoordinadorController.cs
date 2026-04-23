using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PortalAcademicoWeb.Data;
using PortalAcademicoWeb.Models;

namespace PortalAcademicoWeb.Controllers;

[Authorize(Roles = "Coordinador")]
public class CoordinadorController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IDistributedCache _cache;

    public CoordinadorController(
        ApplicationDbContext db,
        IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }

    // GET: /Coordinador
    public async Task<IActionResult> Index()
    {
        var cursos = await _db.Cursos
            .Include(c => c.Matriculas)
            .OrderBy(c => c.Nombre)
            .ToListAsync();

        return View(cursos);
    }

    // GET: /Coordinador/CrearCurso
    public IActionResult CrearCurso()
    {
        return View(new Curso());
    }

    // POST: /Coordinador/CrearCurso
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearCurso(Curso curso)
    {
        // Validación server-side
        if (curso.HorarioFin <= curso.HorarioInicio)
            ModelState.AddModelError("HorarioFin",
                "El horario de fin debe ser posterior al de inicio.");

        if (curso.Creditos <= 0)
            ModelState.AddModelError("Creditos",
                "Los créditos deben ser mayores a 0.");

        // Verificar código único
        var codigoExiste = await _db.Cursos
            .AnyAsync(c => c.Codigo == curso.Codigo);
        if (codigoExiste)
            ModelState.AddModelError("Codigo",
                "Ya existe un curso con ese código.");

        if (!ModelState.IsValid)
            return View(curso);

        _db.Cursos.Add(curso);
        await _db.SaveChangesAsync();

        // Invalidar caché al crear curso
        await _cache.RemoveAsync("cursos_activos");

        TempData["Exito"] = $"Curso '{curso.Nombre}' creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Coordinador/EditarCurso/5
    public async Task<IActionResult> EditarCurso(int id)
    {
        var curso = await _db.Cursos.FindAsync(id);
        if (curso == null)
            return NotFound();

        return View(curso);
    }

    // POST: /Coordinador/EditarCurso/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarCurso(int id, Curso curso)
    {
        if (id != curso.Id)
            return BadRequest();

        // Validación server-side
        if (curso.HorarioFin <= curso.HorarioInicio)
            ModelState.AddModelError("HorarioFin",
                "El horario de fin debe ser posterior al de inicio.");

        if (curso.Creditos <= 0)
            ModelState.AddModelError("Creditos",
                "Los créditos deben ser mayores a 0.");

        // Verificar código único (excluyendo el curso actual)
        var codigoExiste = await _db.Cursos
            .AnyAsync(c => c.Codigo == curso.Codigo && c.Id != id);
        if (codigoExiste)
            ModelState.AddModelError("Codigo",
                "Ya existe un curso con ese código.");

        if (!ModelState.IsValid)
            return View(curso);

        _db.Cursos.Update(curso);
        await _db.SaveChangesAsync();

        // Invalidar caché al editar curso
        await _cache.RemoveAsync("cursos_activos");

        TempData["Exito"] = $"Curso '{curso.Nombre}' actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Coordinador/DesactivarCurso/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DesactivarCurso(int id)
    {
        var curso = await _db.Cursos.FindAsync(id);
        if (curso == null)
            return NotFound();

        curso.Activo = false;
        await _db.SaveChangesAsync();

        // Invalidar caché al desactivar curso
        await _cache.RemoveAsync("cursos_activos");

        TempData["Exito"] = $"Curso '{curso.Nombre}' desactivado.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Coordinador/MatriculasCurso/5
    public async Task<IActionResult> MatriculasCurso(int id)
    {
        var curso = await _db.Cursos
            .Include(c => c.Matriculas)
            .ThenInclude(m => m.Usuario)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (curso == null)
            return NotFound();

        return View(curso);
    }

    // POST: /Coordinador/CambiarEstado
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(
        int matriculaId,
        EstadoMatricula estado,
        int cursoId)
    {
        var matricula = await _db.Matriculas.FindAsync(matriculaId);
        if (matricula == null)
            return NotFound();

        matricula.Estado = estado;
        await _db.SaveChangesAsync();

        TempData["Exito"] = $"Estado cambiado a '{estado}' correctamente.";
        return RedirectToAction(nameof(MatriculasCurso), new { id = cursoId });
    }
}