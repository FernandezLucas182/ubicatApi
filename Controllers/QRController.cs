using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UbicatApi.Models;
using Microsoft.AspNetCore.Authorization;
using UbicatApi.DTOs;
using UbicatApi.Services;

namespace UbicatApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QRController : ControllerBase
    {
        private readonly DataContext context;
        private readonly EmailService emailService;
        private readonly LocalStorageService storage;

        public QRController(DataContext context, EmailService emailService, LocalStorageService storage)
        {
            this.context = context;
            this.emailService = emailService;
            this.storage = storage;
        }


        // ============================================================
        // GET - Escanear QR (PÚBLICO)
        // ============================================================
        [HttpGet("scan/{codigo}")]
        [AllowAnonymous]
        public async Task<IActionResult> Scan(string codigo)
        {
            try
            {
                var qr = await context.QR.FirstOrDefaultAsync(x => x.codigo == codigo);
                if (qr == null) return NotFound("QR no encontrado.");

                var mascota = await context.Mascota.FirstOrDefaultAsync(m => m.idMascota == qr.idMascota);
                if (mascota == null) return NotFound("Mascota no encontrada.");

                var dueño = await context.Usuario.FirstOrDefaultAsync(u => u.idUsuario == qr.idUsuario);

                // Email automático al dueño
                if (dueño != null && !string.IsNullOrEmpty(dueño.email))
                {
                    string subject = mascota.estado?.ToLower() == "perdida"
                        ? $"URGENTE: Escanearon el QR de tu mascota PERDIDA ({mascota.nombre})"
                        : $"Alguien escaneó el QR de tu mascota {mascota.nombre}";

                    string body =
                        $"Alguien escaneó el QR de tu mascota.\n\n" +
                        $"Fecha/hora: {DateTime.Now}\n" +
                        $"Estado actual: {mascota.estado}\n";

                    await emailService.EnviarEmail(dueño.email, subject, body);
                }

                return Ok(new
                {
                    mensaje = "QR válido",
                    qr,
                    mascota = new
                    {
                        mascota.idMascota,
                        mascota.nombre,
                        mascota.especie,
                        mascota.edad,
                        mascota.cuidados,
                        mascota.enfermedades,
                        mascota.estado,
                        mascota.foto
                    },
                    dueño = new
                    {
                        dueño.idUsuario,
                        dueño.nombre,
                        dueño.apellido,
                        dueño.telefono,
                        dueño.email,
                        dueño.ubicacion
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message, inner = ex.InnerException?.Message });
            }
        }

        // ============================================================
        // POST - Reporte de VISTA (PÚBLICO + FOTO)
        // ============================================================
        [HttpPost("reportar-vista/{codigo}")]
        [AllowAnonymous]
        public async Task<IActionResult> ReportarVista(string codigo, [FromForm] ReportarVistaDTO dto)
        {
            try
            {
                var qr = await context.QR.FirstOrDefaultAsync(x => x.codigo == codigo);
                if (qr == null) return NotFound("QR no encontrado.");

                var mascota = await context.Mascota.FindAsync(qr.idMascota);
                if (mascota == null) return NotFound("Mascota no encontrada.");

                // ------------------------------
                // SUBIR FOTO A CLOUDINARY
                string? fotoUrl = null;
                if (dto.foto != null)
                    fotoUrl = await storage.GuardarImagen(dto.foto);

                // ------------------------------

                // Actualizar ubicación en QR
                qr.ubicacionActual = dto.ubicacion;

                // Guardar historial
                context.ReporteMascota.Add(new ReporteMascota
                {
                    idMascota = mascota.idMascota,
                    idUsuarioReporta = null,
                    tipoReporte = "vista",
                    ubicacion = dto.ubicacion,
                    mensaje = dto.mensaje,
                    fecha = DateTime.Now,
                    foto = fotoUrl
                });

                await context.SaveChangesAsync();

                // Enviar email al dueño
                var dueño = await context.Usuario.FindAsync(mascota.idUsuario);

                if (dueño != null && !string.IsNullOrEmpty(dueño.email))
                {
                    string cuerpo =
                        $"Hola {dueño.nombre},\n\n" +
                        $"Alguien vio a tu mascota **{mascota.nombre}**.\n\n" +
                        $"Ubicación: {dto.ubicacion}\n" +
                        $"Mensaje: {dto.mensaje}\n\n" +
                        $"Contacto:\n" +
                        $"- Teléfono: {dto.telefonoReportante}\n" +
                        $"- Email: {dto.emailReportante}\n\n" +
                        $"Foto: {(fotoUrl ?? "No enviada")}\n\n" +
                        $"Fecha/hora: {DateTime.Now}\n\n" +
                        $"— Equipo Ubicat";

                    await emailService.EnviarEmail(dueño.email, $"Vieron a tu mascota {mascota.nombre}", cuerpo);
                }

                return Ok(new
                {
                    mensaje = "Reporte registrado con foto. El dueño fue notificado.",
                    mascota = mascota.nombre,
                    ubicacion = dto.ubicacion,
                    foto = fotoUrl
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message, inner = ex.InnerException?.Message });
            }
        }

        // ============================================================
        // GET - Historial de reportes
        // ============================================================
        [Authorize]
        [HttpGet("historial/{idMascota}")]
        public async Task<IActionResult> Historial(int idMascota)
        {
            int idUsuario = int.Parse(User.Identity?.Name ?? "0");

            var mascota = await context.Mascota.FindAsync(idMascota);
            if (mascota == null) return NotFound("Mascota no encontrada.");

            if (mascota.idUsuario != idUsuario)
                return Unauthorized("No podés ver el historial de una mascota que no es tuya.");

            var historial = await context.ReporteMascota
                .Where(r => r.idMascota == idMascota)
                .OrderByDescending(r => r.fecha)
                .Select(r => new
                {
                    r.idReporte,
                    r.tipoReporte,
                    r.ubicacion,
                    r.mensaje,
                    r.fecha,
                    r.foto
                })
                .ToListAsync();

            return Ok(historial);
        }
    }
}
