
using Microsoft.AspNetCore.Http;

namespace UbicatApi.DTOs
{
    public class ReportarVistaDTO
    {
        public string ubicacion { get; set; } = string.Empty;
        public string mensaje { get; set; } = string.Empty;

        // Datos opcionales de quien reporta
        public string? telefonoReportante { get; set; }
        public string? emailReportante { get; set; }

        // Foto opcional
        public IFormFile? foto { get; set; }

        /// Coordenadas GPS opcionales
        public string? latitud { get; set; }
        public string? longitud { get; set; }
    }
}
