using System.ComponentModel.DataAnnotations;

namespace UbicatApi.Models
{
    public class FotosForo
    {
        [Key]
        public int idFotos { get; set; }

        public int idForo { get; set; }
        public int idUsuario { get; set; }
        public string urlFoto { get; set; }
    }
}
