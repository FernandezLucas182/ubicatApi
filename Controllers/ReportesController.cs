using Microsoft.AspNetCore.Mvc;
using UbicatApi.Models;
using UbicatApi.DTOs;
using UbicatApi.Services;

namespace UbicatApi.Controllers
{
    [ApiController]
    [Route("api/reportes")]
    public class ReportesController : ControllerBase
    {
        private readonly DataContext _ctx;
        private readonly LocalFileService _files;

        public ReportesController(DataContext ctx, LocalFileService files)
        {
            _ctx = ctx;
            _files = files;
        }

        [HttpPost("vista/{idMascota}")]
        public async Task<IActionResult> ReportarVista(int idMascota, [FromForm] ReportarVistaDTO dto)
        {
            string fotoUrl = null;

            if (dto.foto != null)
                fotoUrl = await _files.SaveReporteImage(dto.foto);

            var reporte = new ReporteMascota
            {
                idMascota = idMascota,
                tipoReporte = "vista",
                ubicacion = dto.ubicacion,
                mensaje = dto.mensaje,
                foto = fotoUrl,
                fecha = DateTime.Now
            };

            _ctx.ReporteMascota.Add(reporte);
            await _ctx.SaveChangesAsync();

            return Ok(reporte);
        }
    }
}
