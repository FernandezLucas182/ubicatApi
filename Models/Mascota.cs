
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UbicatApi.Models
{
    public class Mascota
    {
        [Key]
        public int idMascota { get; set; }

        public int idUsuario { get; set; }

        [ForeignKey("idUsuario")]
        public Usuario? Usuario { get; set; }

        public string nombre { get; set; }
        public string especie { get; set; }
        public int edad { get; set; }
        public string enfermedades { get; set; }
        public string cuidados { get; set; }
        public string estado { get; set; }

        // Puede no tener foto todavía
        public string? foto { get; set; }

        public QR? QR { get; set; }

        // Datos iniciales de la pérdida
        public double? latitudPerdida { get; set; }
        public double? longitudPerdida { get; set; }

        // Radio de búsqueda en metros (500 m por defecto)
        public int radioBusquedaMetros { get; set; } = 500;

        // Fecha en que se registró la pérdida
        public DateTime? fechaPerdida { get; set; }
    }
}
