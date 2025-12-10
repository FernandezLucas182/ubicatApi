using System.ComponentModel.DataAnnotations;

namespace UbicatApi.Models
{
    public class Usuario
    {
        [Key]
        public int idUsuario { get; set; }

        public string nombre { get; set; }
        public string apellido { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public string ubicacion { get; set; }
        public string rol { get; set; }
        public string? especialidad { get; set; }
        public string password { get; set; }
        public decimal clasificacionPromedio { get; set; }
    }
}
