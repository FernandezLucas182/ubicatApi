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
