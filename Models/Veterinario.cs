using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UbicatApi.Models
{
    [Table("veterinario")]
    public class Veterinario
    {
        [Key]
        public int idVeterinario { get; set; }

        public int idUsuario { get; set; }

        [Required]
        [MaxLength(50)]
        public string matricula { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string jurisdiccion { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? documentacion { get; set; }

        [Required]
        [MaxLength(20)]
        public string estadoVerificacion { get; set; } = "pendiente";

        public DateTime? fechaVerificacion { get; set; }

        [MaxLength(500)]
        public string? observacion { get; set; }

        // Modalidades de atención
        public bool ofreceVoluntariado { get; set; } = false;

        public bool ofreceServiciosPagos { get; set; } = false;

        [ForeignKey(nameof(idUsuario))]
        public Usuario? Usuario { get; set; }
    }

}