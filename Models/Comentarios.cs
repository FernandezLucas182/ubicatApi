using System.ComponentModel.DataAnnotations;

namespace UbicatApi.Models
{
    public class Comentarios
    {
        [Key]
        public int idComentario { get; set; }

        public int idUsuario { get; set; }
        public int idForo { get; set; }
        public string texto { get; set; }
        public DateTime fecha { get; set; }
    }
}
