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
        // POST - Registrar Mascota (con creación automática del QR)
        // ============================================================
        [Authorize]
        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] MascotaDto dto)
        {
            try
            {
                int idUsuario = int.Parse(User.Identity?.Name ?? "0");

                // Verificar mascota repetida (mismo nombre + especie + usuario)
                var existe = await context.Mascota.AnyAsync(x =>
                    x.idUsuario == idUsuario &&
                    x.nombre == dto.nombre &&
                    x.especie == dto.especie
                );

                if (existe)
                    return BadRequest("Ya registraste una mascota con ese nombre y especie.");

                Mascota m = new Mascota
                {
                    idUsuario = idUsuario,
                    nombre = dto.nombre,
                    especie = dto.especie,
                    edad = dto.edad,
                    enfermedades = dto.enfermedades,
                    cuidados = dto.cuidados,
                    estado = dto.estado,
                    foto = dto.foto
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

                // Evitar referencia circular
                qr.Mascota = null;

                context.QR.Add(qr);
                await context.SaveChangesAsync();

                // incluir QR en la respuesta
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
                .Include(x => x.QR)
                .ToListAsync();

            return Ok(mascotas);
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
                m.foto = dto.foto;

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
