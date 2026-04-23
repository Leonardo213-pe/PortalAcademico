using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PortalAcademicoWeb.Data;
using PortalAcademicoWeb.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PortalAcademicoWeb.Controllers;

public class CursosController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IDistributedCache _cache;
    private const string CACHE_KEY = "cursos_activos";

    // Opciones JSON que evitan el ciclo infinito
    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve
    };

    public CursosController(ApplicationDbContext db, IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }

    // GET: /Cursos
    public async Task<IActionResult> Index(
        string? nombre,
        int? creditosMin,
        int? creditosMax,
        string? horarioDesde,
        string? horarioHasta)
    {
        List<Curso> todosCursos;

        bool hayFiltros = !string.IsNullOrEmpty(nombre)
            || creditosMin.HasValue
            || creditosMax.HasValue
            || !string.IsNullOrEmpty(horarioDesde)
            || !string.IsNullOrEmpty(horarioHasta);

        if (!hayFiltros)
        {
            var cached = await _cache.GetStringAsync(CACHE_KEY);
            if (cached != null)
            {
                todosCursos = JsonSerializer.Deserialize<List<Curso>>(
                    cached, _jsonOpts) ?? new List<Curso>();
            }
            else
            {
                todosCursos = await _db.Cursos
                    .Include(c => c.Matriculas)
                    .Where(c => c.Activo)
                    .ToListAsync();

                var opciones = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
                };

                await _cache.SetStringAsync(
                    CACHE_KEY,
                    JsonSerializer.Serialize(todosCursos, _jsonOpts),
                    opciones);
            }
        }
        else
        {
            todosCursos = await _db.Cursos
                .Include(c => c.Matriculas)
                .Where(c => c.Activo)
                .ToListAsync();
        }

        // Aplicar filtros en memoria
        var cursos = todosCursos.AsEnumerable();

        if (!string.IsNullOrEmpty(nombre))
            cursos = cursos.Where(c =>
                c.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));

        if (creditosMin.HasValue)
            cursos = cursos.Where(c => c.Creditos >= creditosMin.Value);

        if (creditosMax.HasValue)
            cursos = cursos.Where(c => c.Creditos <= creditosMax.Value);

        if (!string.IsNullOrEmpty(horarioDesde) &&
            TimeOnly.TryParse(horarioDesde, out var desde))
            cursos = cursos.Where(c => c.HorarioInicio >= desde);

        if (!string.IsNullOrEmpty(horarioHasta) &&
            TimeOnly.TryParse(horarioHasta, out var hasta))
            cursos = cursos.Where(c => c.HorarioFin <= hasta);

        ViewBag.Nombre = nombre;
        ViewBag.CreditosMin = creditosMin;
        ViewBag.CreditosMax = creditosMax;
        ViewBag.HorarioDesde = horarioDesde;
        ViewBag.HorarioHasta = horarioHasta;

        return View(cursos.ToList());
    }

    // GET: /Cursos/Detalle/5
    public async Task<IActionResult> Detalle(int id)
    {
        var curso = await _db.Cursos
            .Include(c => c.Matriculas)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (curso == null)
            return NotFound();

        // Guardar último curso visitado en sesión
        HttpContext.Session.SetString("UltimoCursoId", id.ToString());
        HttpContext.Session.SetString("UltimoCursoNombre", curso.Nombre);

        return View(curso);
    }
}