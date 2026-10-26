
public class VeterinarioListadoDto
{
    public int idVeterinario { get; set; }
    public int idUsuario { get; set; }
    public string nombre { get; set; } = string.Empty;
    public string apellido { get; set; } = string.Empty;
    public string telefono { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string ubicacion { get; set; } = string.Empty;
    public string? especialidad { get; set; }
    public string? fotoPerfil { get; set; }
    public string matricula { get; set; } = string.Empty;
    public string jurisdiccion { get; set; } = string.Empty;
    public decimal clasificacionPromedio { get; set; }
    public bool ofreceVoluntariado { get; set; }
    public bool ofreceServiciosPagos { get; set; }
}
