using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UbicatApi.Models;

namespace UbicatApi.Controllers
{
    [ApiController]
    [Route("api/public")]
    public class PublicController : ControllerBase
    {
        private readonly DataContext _ctx;

        public PublicController(DataContext ctx)
        {
            _ctx = ctx;
        }

        // ================================
        // ESCANEO QR (PUBLICO)
        // ================================
        [HttpGet("mascota/{codigoQR}")]
        public async Task<IActionResult> MascotaPorQR(string codigoQR)
        {
            var qr = await _ctx.QR
                .Include(q => q.Mascota)
                    .ThenInclude(m => m.Usuario)
                .FirstOrDefaultAsync(q => q.codigo == codigoQR);

            if (qr == null)
                return NotFound("QR inválido.");

            qr.fechaUltimoEscaneo = DateTime.Now;
            await _ctx.SaveChangesAsync();

            return Ok(new
            {
                mascota = new
                {
                    qr.Mascota.nombre,
                    qr.Mascota.especie,
                    qr.Mascota.estado,
                    qr.Mascota.foto
                },
                contacto = new
                {
                    qr.Mascota.Usuario.nombre,
                    qr.Mascota.Usuario.telefono,
                    qr.Mascota.Usuario.email,
                    qr.Mascota.Usuario.ubicacion
                },
                ultimaUbicacion = qr.ubicacionActual,
                ultimoEscaneo = qr.fechaUltimoEscaneo
            });
        }

        // ================================
        // REGISTRAR UBICACION DE QUIEN ENCUENTRA
        // ================================
        [HttpPost("mascota/{codigoQR}/ubicacion")]
        public async Task<IActionResult> RegistrarUbicacion(string codigoQR, [FromBody] string ubicacion)
        {
            var qr = await _ctx.QR.FirstOrDefaultAsync(x => x.codigo == codigoQR);

            if (qr == null)
                return NotFound("QR inválido.");

            qr.ubicacionActual = ubicacion;
            qr.fechaUltimoEscaneo = DateTime.Now;
            await _ctx.SaveChangesAsync();

            return Ok("Ubicación registrada.");
        }
    }
}
