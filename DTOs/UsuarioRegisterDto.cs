using System.ComponentModel.DataAnnotations;

namespace UbicatApi.DTOs
{
    public class UsuarioRegisterDto
    {
        [Required]
        public string nombre { get; set; }

        [Required]
        public string apellido { get; set; }

        [Required, EmailAddress]
        public string email { get; set; }

        [Required]
        public string telefono { get; set; }

        [Required]
        public string ubicacion { get; set; }

        [Required]
        public string rol { get; set; } // dueño, veterinario, rescatista, publico

        [Required]
        public string password { get; set; }
    }
}
