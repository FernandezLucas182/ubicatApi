using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UbicatApi.Models;
using Microsoft.AspNetCore.Authorization;
using UbicatApi.DTOs;

namespace UbicatApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QRController : ControllerBase
    {
        private readonly DataContext context;

        public QRController(DataContext context)
        {
            this.context = context;
        }

        // ============================================================
        // GET - Escanear QR por código (PÚBLICO)
        // ============================================================
        [HttpGet("scan/{codigo}")]
        [AllowAnonymous]
        public async Task<IActionResult> Scan(string codigo)
        {
            try
            {
                var qr = await context.QR
                    .FirstOrDefaultAsync(x => x.codigo == codigo);

                if (qr == null)
                    return NotFound("QR no encontrado.");

                var mascota = await context.Mascota
                    .Where(x => x.idMascota == qr.idMascota)
                    .Select(m => new
                    {
                        m.idMascota,
                        m.nombre,
                        m.especie,
                        m.edad,
                        m.cuidados,
                        m.enfermedades,
                        m.estado,
                        m.foto
                    })
                    .FirstOrDefaultAsync();

                if (mascota == null)
                    return NotFound("Mascota no encontrada.");

                var dueño = await context.Usuario
                    .Where(u => u.idUsuario == qr.idUsuario)
                    .Select(u => new
                    {
                        u.idUsuario,
                        u.nombre,
                        u.apellido,
                        u.telefono,
                        u.email,
                        u.ubicacion
                    })
                    .FirstOrDefaultAsync();

                return Ok(new
                {
                    mensaje = "QR válido",
                    qr,
                    mascota,
                    dueño
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }

        }

        // ============================================================
        // PUT - Reportar mascota como perdida (DUEÑO)
        // ============================================================
        [Authorize]
        [HttpPut("reportar-perdida/{idMascota}")]
        public async Task<IActionResult> ReportarPerdida(int idMascota)
        {
            try
            {
                int idUsuario = int.Parse(User.Identity?.Name ?? "0");

                var mascota = await context.Mascota
                    .Include(m => m.QR)
                    .FirstOrDefaultAsync(m => m.idMascota == idMascota);

                if (mascota == null)
                    return NotFound("La mascota no existe.");

                if (mascota.idUsuario != idUsuario)
                    return Unauthorized("No podes reportar una mascota que no es tuya.");

                mascota.estado = "perdida";

                if (mascota.QR != null)
                {
                    mascota.QR.ubicacionActual = "Desconocida";
                }

                // Guardar historial
                context.ReporteMascota.Add(new ReporteMascota
                {
                    idMascota = idMascota,
                    idUsuarioReporta = idUsuario,
                    tipoReporte = "perdida",
                    fecha = DateTime.Now
                });

                await context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "La mascota fue reportada como perdida.",
                    mascota
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }

        }

        // ============================================================
        // PUT - Reportar mascota como EN CASA (DUEÑO)
        // ============================================================
        [Authorize]
        [HttpPut("reportar-en-casa/{idMascota}")]
        public async Task<IActionResult> ReportarEnCasa(int idMascota)
        {
            try
            {
                int idUsuario = int.Parse(User.Identity?.Name ?? "0");

                var mascota = await context.Mascota
                    .Include(m => m.QR)
                    .FirstOrDefaultAsync(m => m.idMascota == idMascota);

                if (mascota == null)
                    return NotFound("La mascota no existe.");

                if (mascota.idUsuario != idUsuario)
                    return Unauthorized("No podes reportar una mascota que no es tuya.");

                mascota.estado = "en casa";

                if (mascota.QR != null)
                {
                    mascota.QR.ubicacionActual = "En casa";
                }

                // Guardar historial
                context.ReporteMascota.Add(new ReporteMascota
                {
                    idMascota = idMascota,
                    idUsuarioReporta = idUsuario,
                    tipoReporte = "en_casa",
                    fecha = DateTime.Now
                });

                await context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "La mascota fue marcada como EN CASA.",
                    mascota
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }

        }

        // ============================================================
        // POST - Reportar que alguien VIO la mascota (PÚBLICO)
        // ============================================================
        [HttpPost("reportar-vista/{codigo}")]
        [AllowAnonymous]
        public async Task<IActionResult> ReportarVista(string codigo, [FromBody] ReportarVistaDTO dto)
        {
            try
            {
                var qr = await context.QR.FirstOrDefaultAsync(x => x.codigo == codigo);

                if (qr == null)
                    return NotFound("QR no encontrado.");

                var mascota = await context.Mascota.FindAsync(qr.idMascota);

                if (mascota == null)
                    return NotFound("Mascota no encontrada.");

                // Actualizar ubicación del QR
                qr.ubicacionActual = dto.ubicacion;

                // Guardar historial
                context.ReporteMascota.Add(new ReporteMascota
                {
                    idMascota = mascota.idMascota,
                    idUsuarioReporta = null, // Público
                    tipoReporte = "vista",
                    ubicacion = dto.ubicacion,
                    mensaje = dto.mensaje,
                    fecha = DateTime.Now
                });

                await context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Reporte de vista registrado.",
                    mascota = mascota.nombre,
                    ubicacion = dto.ubicacion
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }

        }

        // ============================================================
        // GET - Historial de reportes (solo dueño)
        // ============================================================
        [Authorize]
        [HttpGet("historial/{idMascota}")]
        public async Task<IActionResult> Historial(int idMascota)
        {
            int idUsuario = int.Parse(User.Identity?.Name ?? "0");

            var mascota = await context.Mascota.FirstOrDefaultAsync(m => m.idMascota == idMascota);

            if (mascota == null)
                return NotFound("Mascota no encontrada.");

            if (mascota.idUsuario != idUsuario)
                return Unauthorized("No puedes ver el historial de una mascota que no es tuya.");

            var historial = await context.ReporteMascota
                .Where(r => r.idMascota == idMascota)
                .OrderByDescending(r => r.fecha)
                .Select(r => new
                {
                    r.idReporte,
                    r.tipoReporte,
                    r.ubicacion,
                    r.mensaje,
                    r.fecha
                })
                .ToListAsync();

            return Ok(historial);
        }
    }
}
