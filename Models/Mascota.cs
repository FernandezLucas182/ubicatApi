using System.ComponentModel.DataAnnotations;

namespace UbicatApi.Models
{
    public class Mascota
    {
        [Key]
        public int idMascota { get; set; }

        public int idUsuario { get; set; }
        public string nombre { get; set; }

        public string especie { get; set; }
        public int edad { get; set; }
        public string enfermedades { get; set; }
        public string cuidados { get; set; }
        public string estado { get; set; }
        public string foto { get; set; }
        

        public QR QR { get; set; }
    }
}
