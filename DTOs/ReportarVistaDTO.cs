using Microsoft.AspNetCore.Http;

namespace UbicatApi.DTOs
{
    public class ReportarVistaDTO
    {
        public string ubicacion { get; set; }
        public string mensaje { get; set; }

        // Datos opcionales de quien reporta
        public string telefonoReportante { get; set; }
        public string emailReportante { get; set; }

        // FOTO DE LA MASCOTA VISTA
        public IFormFile foto { get; set; }
    }
}
