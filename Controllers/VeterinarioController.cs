
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UbicatApi.Models;

namespace UbicatApi.Controllers
{
    [ApiController]
    [Route("api/veterinario")]
    [Authorize]
    public class VeterinarioController : ControllerBase
    {
        private readonly DataContext context;

        public VeterinarioController(DataContext context)
        {
            this.context = context;
        }



        // Listado público de veterinarios aprobados.
        [HttpGet("listado")]
        [AllowAnonymous]
        public async Task<IActionResult> ListarVeterinarios()
        {
            var veterinarios = await context.Veterinario
                .AsNoTracking()
                .Where(v =>
                    v.estadoVerificacion == "aprobado" &&
                    (v.ofreceVoluntariado || v.ofreceServiciosPagos) &&
                    v.Usuario != null &&
                    v.Usuario.rol == "veterinario")
                .Select(v => new VeterinarioListadoDto
                {
                    idVeterinario = v.idVeterinario,
                    idUsuario = v.idUsuario,
                    nombre = v.Usuario!.nombre,
                    apellido = v.Usuario!.apellido,
                    telefono = v.Usuario!.telefono,
                    email = v.Usuario!.email,
                    ubicacion = v.Usuario!.ubicacion,
                    especialidad = v.Usuario!.especialidad,
                    fotoPerfil = v.Usuario!.fotoPerfil,
                    matricula = v.matricula,
                    jurisdiccion = v.jurisdiccion,
                    clasificacionPromedio = v.Usuario!.clasificacionPromedio,
                    ofreceVoluntariado = v.ofreceVoluntariado,
                    ofreceServiciosPagos = v.ofreceServiciosPagos
                })
                .ToListAsync();

            return Ok(veterinarios);
        }


        // El veterinario configura sus modalidades de atención.
        [HttpPut("modalidades")]
        public async Task<IActionResult> ActualizarModalidades(
            [FromBody] ModalidadesVeterinarioDto dto)
        {
            if (!int.TryParse(User.Identity?.Name, out int idUsuario))
                return Unauthorized("No se pudo identificar al usuario.");

            var usuario = await context.Usuario
                .FirstOrDefaultAsync(u => u.idUsuario == idUsuario);

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            if (usuario.rol != "veterinario")
                return Forbid();

            var veterinario = await context.Veterinario
                .FirstOrDefaultAsync(v => v.idUsuario == idUsuario);

            if (veterinario == null)
                return NotFound("No se encontraron los datos del veterinario.");

            if (veterinario.estadoVerificacion != "aprobado")
                return StatusCode(403, new
                {
                    mensaje = "Debés tener la matrícula aprobada para configurar tus modalidades."
                });

            if (!dto.ofreceVoluntariado && !dto.ofreceServiciosPagos)
                return BadRequest(
                    "Debés seleccionar al menos una modalidad de atención.");

            veterinario.ofreceVoluntariado = dto.ofreceVoluntariado;
            veterinario.ofreceServiciosPagos = dto.ofreceServiciosPagos;

            await context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Modalidades actualizadas correctamente.",
                veterinario.ofreceVoluntariado,
                veterinario.ofreceServiciosPagos
            });
        }
    }

    public class ModalidadesVeterinarioDto
    {
        public bool ofreceVoluntariado { get; set; }
        public bool ofreceServiciosPagos { get; set; }
    }
}
