using System.ComponentModel.DataAnnotations;

namespace UbicatApi.Models
{
    public class Validaciones
    {
        [Key]
        public int idValidacion { get; set; }

        public int idComentario { get; set; }
        public int idUsuarioValidador { get; set; }
        public string observacion { get; set; }
        public int puntuacion { get; set; }
    }
}
