
public class CambiarEstadoMascotaDto
{
    public string estado { get; set; } = string.Empty;

    // Ubicación donde se perdió la mascota
    public double? latitudPerdida { get; set; }
    public double? longitudPerdida { get; set; }

    // Radio de búsqueda en metros
    public int? radioBusquedaMetros { get; set; }
}
