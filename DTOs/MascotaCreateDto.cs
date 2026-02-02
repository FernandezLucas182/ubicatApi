using Microsoft.AspNetCore.Http;

namespace UbicatApi.DTOs
{
    public class MascotaCreateDto
    {
        public string nombre { get; set; }
        public string especie { get; set; }
        public int edad { get; set; }
        public string enfermedades { get; set; }
        public string cuidados { get; set; }
        public string estado { get; set; }

        public IFormFile? foto { get; set; } // 👈 archivo
    }
}
