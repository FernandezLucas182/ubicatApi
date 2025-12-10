using System.ComponentModel.DataAnnotations;

namespace UbicatApi.Models
{
    public class Categorias
    {
        [Key]
        public int idCategoria { get; set; }
        public string nombreCategoria { get; set; }
        public string descripcion { get; set; }
    }
}
