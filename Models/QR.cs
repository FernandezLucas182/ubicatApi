using System.ComponentModel.DataAnnotations;

namespace UbicatApi.Models
{
    public class QR
    {
        [Key]
        public int idQR { get; set; }

        public int idUsuario { get; set; }
        public int idMascota { get; set; }
        public string codigo { get; set; }
        public string urlDatosMascota { get; set; }
        public DateTime fechaGeneracion { get; set; }
        public string ubicacionActual { get; set; }

        public Mascota Mascota { get; set; }
    }
}
