
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UbicatApi.DTOs
{
    public class UsuarioRegisterDto
    {
        [Required(ErrorMessage = "La foto de perfil es obligatoria.")]
        public IFormFile? fotoPerfil { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, MinimumLength = 2)]
        public string nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, MinimumLength = 2)]
        public string apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        [StringLength(150)]
        public string email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(30, MinimumLength = 6)]
        public string telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ubicación es obligatoria.")]
        [StringLength(100, MinimumLength = 2)]
        public string ubicacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public string rol { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres.")]
        public string password { get; set; } = string.Empty;

        // Datos requeridos únicamente para veterinarios
        [StringLength(50)]
        public string? matricula { get; set; }

        [StringLength(100)]
        public string? jurisdiccion { get; set; }

        // Modalidades de atención veterinaria
        public bool ofreceServiciosPagos { get; set; } = false;

        public bool ofreceVoluntariado { get; set; } = false;

        // Documentación profesional
        public IFormFile? documentacion { get; set; }
    }
}
