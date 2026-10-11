using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UbicatApi.Models;

namespace UbicatApi.Controllers
{
    public class MascotaWebController : Controller
    {
        private readonly DataContext _context;

        public MascotaWebController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("/mascota/{codigoQR}")]
        public async Task<IActionResult> Detalle(string codigoQR)
        {
            var qr = await _context.QR
                .Include(q => q.Mascota)
                .FirstOrDefaultAsync(q => q.codigo == codigoQR);

            if (qr == null || qr.Mascota == null)
                return NotFound("No encontramos una mascota asociada a este código QR.");

            return View(qr);
        }
    }
}