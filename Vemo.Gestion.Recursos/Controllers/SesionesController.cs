using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vemo.Gestion.Recursos.Data;
using Vemo.Gestion.Recursos.Data.Entidades;
using Vemo.Gestion.Recursos.Data.Models;

[Route("api/[controller]")]
[ApiController]
public class SesionesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public SesionesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> IniciarSesion([FromBody] int consumo)
    {
        var recurso = await _context.RecursosCompartidos.FirstOrDefaultAsync();
        if (recurso == null) return NotFound("Recurso no encontrado.");

        if (recurso.CapacidadDisponible < consumo)
        {
            return BadRequest("No hay capacidad suficiente para iniciar esta sesión.");
        }

        var usuarioId = User.FindFirst("id")?.Value;
        var Sesiones = new Sesiones
        {
            UsuarioId = usuarioId,
            Consumo = consumo,
            Inicio = DateTime.UtcNow
        };

        recurso.CapacidadDisponible -= consumo;

        _context.Sesiones.Add(Sesiones);
        _context.RecursosCompartidos.Update(recurso);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<Sesiones>(Sesiones, "Sesión iniciada exitosamente."));
    }

    [HttpPut("{id}/detener")]
    [Authorize]
    public async Task<IActionResult> DetenerSesion(int id)
    {
        var sesion = await _context.Sesiones.FindAsync(id);
        if (sesion == null || sesion.Fin != null)
        {
            return NotFound("Sesión no encontrada o ya finalizada.");
        }

        var recurso = await _context.RecursosCompartidos.FirstOrDefaultAsync();
        if (recurso == null) return NotFound("Recurso no encontrado.");

        sesion.Fin = DateTime.UtcNow;
        recurso.CapacidadDisponible += sesion.Consumo;

        _context.Sesiones.Update(sesion);
        _context.RecursosCompartidos.Update(recurso);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<Sesiones>(sesion, "Sesión detenida exitosamente."));
    }

    [HttpGet("activas")]
    [Authorize]
    public async Task<IActionResult> ObtenerSesionesActivas()
    {
        var sesiones = await _context.Sesiones
            .Where(s => s.Fin == null)
            .ToListAsync();

        return Ok(new ApiResponse<List<Sesiones>>(sesiones, "Sesiones activas obtenidas."));
    }

    [HttpGet("consolidado")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ObtenerConsolidado()
    {
        var recurso = await _context.RecursosCompartidos.FirstOrDefaultAsync();
        if (recurso == null) return NotFound("Recurso no encontrado.");

        var sesionesActivas = await _context.Sesiones
            .Where(s => s.Fin == null)
            .ToListAsync();

        var consolidado = new
        {
            CapacidadTotal = recurso.CapacidadTotal,
            CapacidadDisponible = recurso.CapacidadDisponible,
            SesionesActivas = sesionesActivas.Count
        };

        return Ok(new ApiResponse<object>(consolidado, "Consolidado obtenido."));
    }
}