
namespace UbicatApi.DTOs
{
    public class CambiarEstadoMascotaDto
    {
        public string estado { get; set; } = string.Empty;

        public double? latitudPerdida { get; set; }
        public double? longitudPerdida { get; set; }
        public int? radioBusquedaMetros { get; set; }

        public string? telefonoContacto { get; set; }
        public string? emailContacto { get; set; }
        public string? mensaje { get; set; }
    }
}
