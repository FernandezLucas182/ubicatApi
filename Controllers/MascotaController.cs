using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UbicatApi.DTOs;
using UbicatApi.Models;

namespace UbicatApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MascotaController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IConfiguration configuration;

        public MascotaController(DataContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        // ============================================================
        // POST - Crear Mascota CON FOTO (multipart/form-data)
        // ============================================================
        [Authorize]
        [HttpPost("crear-con-foto")]
        public async Task<IActionResult> CrearConFoto([FromForm] MascotaCreateDto dto)
        {
            try
            {
                int idUsuario = int.Parse(User.Identity?.Name ?? "0");

                string? nombreArchivo = null;

                if (dto.foto != null)
                {
                    string carpeta = Path.Combine(Directory.GetCurrentDirectory(),
                                                   "wwwroot/uploads/mascotas");

                    if (!Directory.Exists(carpeta))
                        Directory.CreateDirectory(carpeta);

                    nombreArchivo = Guid.NewGuid().ToString() +
                                    Path.GetExtension(dto.foto.FileName);

                    string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                    using var stream = new FileStream(rutaCompleta, FileMode.Create);
                    await dto.foto.CopyToAsync(stream);
                }

                Mascota m = new Mascota
                {
                    idUsuario = idUsuario,
                    nombre = dto.nombre,
                    especie = dto.especie,
                    edad = dto.edad,
                    enfermedades = dto.enfermedades,
                    cuidados = dto.cuidados,
                    estado = dto.estado,
                    foto = nombreArchivo // 👈 SOLO el nombre
                };

                context.Mascota.Add(m);
                await context.SaveChangesAsync();

                return Ok(m);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // ============================================================
        // POST - Registrar Mascota (con foto obligatoria + QR automático)
        // ============================================================
        [Authorize]
        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromForm] MascotaCreateDto dto)
        {
            try
            {
                int idUsuario = int.Parse(User.Identity?.Name ?? "0");

                // La foto es obligatoria
                if (dto.foto == null || dto.foto.Length == 0)
                    return BadRequest("La foto de la mascota es obligatoria.");

                var existe = await context.Mascota.AnyAsync(x =>
                    x.idUsuario == idUsuario &&
                    x.nombre == dto.nombre &&
                    x.especie == dto.especie
                );

                if (existe)
                    return BadRequest("Ya registraste una mascota con ese nombre y especie.");

                // Guardar foto
                string carpeta = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/mascotas"
                );

                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                string nombreArchivo =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(dto.foto.FileName);

                string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using var stream = new FileStream(
                    rutaCompleta,
                    FileMode.Create
                );

                await dto.foto.CopyToAsync(stream);

                // Crear mascota
                Mascota m = new Mascota
                {
                    idUsuario = idUsuario,
                    nombre = dto.nombre,
                    especie = dto.especie,
                    edad = dto.edad,
                    enfermedades = dto.enfermedades,
                    cuidados = dto.cuidados,
                    estado = dto.estado,
                    foto = nombreArchivo
                };

                context.Mascota.Add(m);
                await context.SaveChangesAsync();

                // Crear QR automáticamente
                QR qr = new QR
                {
                    idUsuario = idUsuario,
                    idMascota = m.idMascota,
                    codigo = Guid.NewGuid().ToString().Substring(0, 8),
                    urlDatosMascota = $"https://ubicat.com/mascota/{m.idMascota}",
                    fechaGeneracion = DateTime.Now,
                    ubicacionActual = "Sin ubicación"
                };

                qr.Mascota = null;

                context.QR.Add(qr);
                await context.SaveChangesAsync();

                m.QR = qr;

                return Ok(new
                {
                    mensaje = "Mascota registrada correctamente",
                    mascota = m,
                    qr = qr
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ============================================================
        // GET - Mis mascotas
        // ============================================================
        [Authorize]
        [HttpGet("mias")]
        public async Task<IActionResult> MisMascotas()
        {
            int idUsuario = int.Parse(User.Identity?.Name ?? "0");

            var mascotas = await context.Mascota
                .Where(x => x.idUsuario == idUsuario)
                .Select(m => new
                {
                    m.idMascota,
                    m.nombre,
                    m.estado,
                    fotoUrl = m.foto == null
                        ? null
                        : $"{Request.Scheme}://{Request.Host}/uploads/mascotas/{m.foto}"
                })
                .ToListAsync();

            return Ok(mascotas);
        }


        // ============================================================
        // GET - Obtener detalle de una mascota
        // ============================================================
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            int idUsuario = int.Parse(User.Identity?.Name ?? "0");

            var mascota = await context.Mascota
                .Include(x => x.QR)
                .FirstOrDefaultAsync(x =>
                    x.idMascota == id &&
                    x.idUsuario == idUsuario);

            if (mascota == null)
                return NotFound("Mascota no encontrada.");

            return Ok(new
            {
                mascota.idMascota,
                mascota.nombre,
                mascota.especie,
                mascota.edad,
                mascota.enfermedades,
                mascota.cuidados,
                mascota.estado,

                fotoUrl = mascota.foto == null
                    ? null
                    : $"{Request.Scheme}://{Request.Host}/uploads/mascotas/{mascota.foto}",

                qr = mascota.QR
            });
        }



        // ============================================================
        // POST - Subir / cambiar foto de perfil de mascota
        // ============================================================
        [Authorize]
        [HttpPost("foto-perfil/{idMascota}")]
        public async Task<IActionResult> SubirFotoPerfil(
            int idMascota,
            [FromForm] IFormFile foto)
        {
            int idUsuario = int.Parse(User.Identity?.Name ?? "0");

            var mascota = await context.Mascota
                .FirstOrDefaultAsync(x => x.idMascota == idMascota && x.idUsuario == idUsuario);

            if (mascota == null)
                return BadRequest("Mascota no encontrada");

            if (foto == null || foto.Length == 0)
                return BadRequest("No se envió ninguna imagen");

            string carpeta = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/uploads/mascotas"
            );

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            string nombreArchivo = Guid.NewGuid().ToString() +
                                   Path.GetExtension(foto.FileName);

            string ruta = Path.Combine(carpeta, nombreArchivo);

            using var stream = new FileStream(ruta, FileMode.Create);
            await foto.CopyToAsync(stream);

            mascota.foto = nombreArchivo;
            await context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Foto de perfil actualizada",
                foto = nombreArchivo
            });
        }




        // ============================================================
        // PUT - Editar Mascota
        // ============================================================
        [Authorize]
        [HttpPut("editar/{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] MascotaDto dto)
        {
            try
            {
                int idUsuario = int.Parse(User.Identity?.Name ?? "0");

                var m = await context.Mascota.FindAsync(id);

                if (m == null || m.idUsuario != idUsuario)
                    return BadRequest("Mascota no encontrada o no pertenece al usuario.");

                m.nombre = dto.nombre;
                m.especie = dto.especie;
                m.edad = dto.edad;
                m.enfermedades = dto.enfermedades;
                m.cuidados = dto.cuidados;
                m.estado = dto.estado;

                if (dto.foto != null)
                {
                    m.foto = dto.foto;
                }

                context.Mascota.Update(m);
                await context.SaveChangesAsync();

                return Ok(new { mensaje = "Mascota actualizada", mascota = m });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // ============================================================
        // PUT - Cambiar solamente el estado de una mascota
        // ============================================================

        [Authorize]
        [HttpPut("estado/{id}")]
        public async Task<IActionResult> CambiarEstado(
            int id,
            [FromBody] CambiarEstadoMascotaDto dto)
        {
            int idUsuario = int.Parse(User.Identity?.Name ?? "0");

            var mascota = await context.Mascota.FirstOrDefaultAsync(x =>
                x.idMascota == id &&
                x.idUsuario == idUsuario);

            if (mascota == null)
                return NotFound("Mascota no encontrada.");
            bool nuevaPerdida = mascota.estado != "perdida";

            if (dto == null ||
                (dto.estado != "perdida" && dto.estado != "en casa"))
            {
                return BadRequest("El estado debe ser 'perdida' o 'en casa'.");
            }

            if (dto.estado == "perdida")
            {
                // Validar coordenadas: deben enviarse juntas
                if (dto.latitudPerdida.HasValue != dto.longitudPerdida.HasValue)
                {
                    return BadRequest(
                        "Debés enviar latitud y longitud juntas.");
                }

                if (dto.latitudPerdida.HasValue &&
                    (dto.latitudPerdida < -90 || dto.latitudPerdida > 90))
                {
                    return BadRequest("La latitud no es válida.");
                }

                if (dto.longitudPerdida.HasValue &&
                    (dto.longitudPerdida < -180 || dto.longitudPerdida > 180))
                {
                    return BadRequest("La longitud no es válida.");
                }

                if (dto.radioBusquedaMetros.HasValue &&
                    (dto.radioBusquedaMetros < 100 ||
                     dto.radioBusquedaMetros > 50000))
                {
                    return BadRequest(
                        "El radio debe estar entre 100 y 50000 metros.");
                }

                // Registrar la fecha solo al iniciar una nueva pérdida
                if (mascota.estado != "perdida")
                {
                    mascota.fechaPerdida = DateTime.UtcNow;
                }

                if (dto.latitudPerdida.HasValue)
                    mascota.latitudPerdida = dto.latitudPerdida;

                if (dto.longitudPerdida.HasValue)
                    mascota.longitudPerdida = dto.longitudPerdida;

                if (dto.radioBusquedaMetros.HasValue)
                    mascota.radioBusquedaMetros = dto.radioBusquedaMetros.Value;
            }
            else
            {
                // Al regresar a casa, cerrar la alerta activa
                mascota.fechaPerdida = null;
            }

            if (dto.estado == "perdida")
            {
                var reportePerdida = await context.ReporteMascota
                    .Where(r => r.idMascota == mascota.idMascota
                        && r.tipoReporte == "perdida")
                    .OrderByDescending(r => r.fecha)
                    .FirstOrDefaultAsync();

                if (reportePerdida == null || nuevaPerdida)
                {
                    reportePerdida = new ReporteMascota
                    {
                        idMascota = mascota.idMascota,
                        idUsuarioReporta = idUsuario,
                        tipoReporte = "perdida",
                        fecha = DateTime.UtcNow
                    };

                    context.ReporteMascota.Add(reportePerdida);
                }

                reportePerdida.mensaje = dto.mensaje;
                reportePerdida.telefonoReportante = dto.telefonoContacto;
                reportePerdida.emailReportante = dto.emailContacto;
            }

            mascota.estado = dto.estado;

            await context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Estado actualizado correctamente",
                estado = mascota.estado,
                latitudPerdida = mascota.latitudPerdida,
                longitudPerdida = mascota.longitudPerdida,
                radioBusquedaMetros = mascota.radioBusquedaMetros,
                fechaPerdida = mascota.fechaPerdida
            });
        }




        [Authorize]
        [HttpGet("perdidas-con-ubicacion")]
        public async Task<IActionResult> ObtenerMascotasPerdidas()
        {
            var mascotas = await context.Mascota
                .Include(m => m.QR)
                .Where(m => m.estado == "perdida")
                .ToListAsync();

            var resultado = new List<object>();

            foreach (var mascota in mascotas)
            {
                var avistamiento = await context.ReporteMascota
                    .Where(r => r.idMascota == mascota.idMascota
                        && r.tipoReporte == "vista"
                        && r.latitud != null
                        && r.longitud != null)
                    .OrderByDescending(r => r.fecha)
                    .FirstOrDefaultAsync();

                // Ubicación del último avistamiento
                double? latitud = avistamiento?.latitud
                    ?? mascota.QR?.latitudActual;

                double? longitud = avistamiento?.longitud
                    ?? mascota.QR?.longitudActual;

                resultado.Add(new
                {
                    mascota.idMascota,
                    mascota.nombre,
                    mascota.especie,
                    mascota.edad,
                    mascota.foto,
                    mascota.estado,

                    // Último avistamiento
                    ubicacion = avistamiento?.ubicacion
                        ?? mascota.QR?.ubicacionActual,
                    latitud,
                    longitud,
                    fechaAvistamiento = avistamiento?.fecha,

                    // Datos iniciales de la pérdida
                    mascota.latitudPerdida,
                    mascota.longitudPerdida,
                    mascota.radioBusquedaMetros,
                    mascota.fechaPerdida
                });
            }

            return Ok(resultado);
        }



        // ============================================================
        // DELETE - Eliminar Mascota
        // ============================================================
        [Authorize]
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                int idUsuario = int.Parse(User.Identity?.Name ?? "0");

                var m = await context.Mascota.FindAsync(id);

                if (m == null || m.idUsuario != idUsuario)
                    return BadRequest("No existe o no es tu mascota.");

                var qr = await context.QR.FirstOrDefaultAsync(x => x.idMascota == id);
                if (qr != null)
                    context.QR.Remove(qr);

                context.Mascota.Remove(m);
                await context.SaveChangesAsync();

                return Ok("Mascota eliminada con éxito.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
