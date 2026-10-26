
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UbicatApi.Models;

namespace UbicatApi.Controllers
{
    [ApiController]
    [Route("api/verificacion-veterinarios")]
    [Authorize(Roles = "admin")]
    public class VerificacionVeterinariosController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IWebHostEnvironment env;

        public VerificacionVeterinariosController(
            DataContext context,
            IWebHostEnvironment env)
        {
            this.context = context;
            this.env = env;
        }

        // GET api/verificacion-veterinarios/pendientes
        [HttpGet("pendientes")]
        public async Task<IActionResult> ObtenerPendientes()
        {
            var solicitudes = await context.Veterinario
                .Where(v => v.estadoVerificacion == "pendiente")
                .Select(v => new
                {
                    v.idVeterinario,
                    v.idUsuario,
                    v.Usuario!.nombre,
                    v.Usuario.apellido,
                    v.Usuario.email,
                    v.matricula,
                    v.jurisdiccion,
                    v.estadoVerificacion,
                    tieneDocumentacion = v.documentacion != null
                })
                .ToListAsync();

            return Ok(solicitudes);
        }

        // GET api/verificacion-veterinarios/{id}/documentacion
        [HttpGet("{id:int}/documentacion")]
        public async Task<IActionResult> VerDocumentacion(int id)
        {
            var veterinario = await context.Veterinario
                .FirstOrDefaultAsync(v => v.idVeterinario == id);

            if (veterinario == null)
                return NotFound("Solicitud no encontrada.");

            if (string.IsNullOrWhiteSpace(veterinario.documentacion))
                return NotFound("La solicitud no tiene documentación.");

            string nombreArchivo =
                Path.GetFileName(veterinario.documentacion);

            string ruta = Path.Combine(
                env.WebRootPath ?? Path.Combine(
                    env.ContentRootPath, "wwwroot"),
                "uploads",
                "veterinarios",
                nombreArchivo);

            if (!System.IO.File.Exists(ruta))
                return NotFound("No se encontró el archivo.");

            string extension = Path.GetExtension(ruta).ToLowerInvariant();

            string contentType = extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            return PhysicalFile(ruta, contentType);
        }

        // PUT api/verificacion-veterinarios/{id}/resolver
        [HttpPut("{id:int}/resolver")]
        public async Task<IActionResult> ResolverSolicitud(
            int id,
            [FromBody] ResolverVeterinarioDto dto)
        {
            if (dto == null)
                return BadRequest("Debés indicar la resolución.");

            string estado = dto.estado?.Trim().ToLowerInvariant() ?? "";

            if (estado != "aprobado" && estado != "rechazado")
                return BadRequest(
                    "El estado debe ser 'aprobado' o 'rechazado'.");

            string? observacion = dto.observacion?.Trim();

            if (estado == "rechazado" &&
                string.IsNullOrWhiteSpace(observacion))
            {
                return BadRequest(
                    "Debés indicar el motivo del rechazo.");
            }

            if (observacion?.Length > 500)
                return BadRequest(
                    "La observación no puede superar los 500 caracteres.");

            var veterinario = await context.Veterinario
                .FirstOrDefaultAsync(v => v.idVeterinario == id);

            if (veterinario == null)
                return NotFound("Solicitud no encontrada.");

            if (veterinario.estadoVerificacion != "pendiente")
                return Conflict(
                    "Esta solicitud ya fue resuelta.");

            veterinario.estadoVerificacion = estado;
            veterinario.fechaVerificacion = DateTime.UtcNow;
            veterinario.observacion = observacion;

            await context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = estado == "aprobado"
                    ? "Veterinario aprobado correctamente."
                    : "Solicitud de veterinario rechazada.",
                veterinario.idVeterinario,
                veterinario.estadoVerificacion,
                veterinario.fechaVerificacion,
                veterinario.observacion
            });
        }
    }

    public class ResolverVeterinarioDto
    {
        public string? estado { get; set; }
        public string? observacion { get; set; }
    }
}
