using System.ComponentModel.DataAnnotations;

namespace UbicatApi.Models
{
    public class Foro
    {
        [Key]
        public int idForo { get; set; }

        public int idUsuario { get; set; }
        public int idCategoria { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public string imagenPrincipal { get; set; }
        public DateTime fechaPublicacion { get; set; }
    }
}
