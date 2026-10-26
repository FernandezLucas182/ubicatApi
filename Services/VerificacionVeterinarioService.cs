using Microsoft.EntityFrameworkCore;
using UbicatApi.Models;

namespace UbicatApi.Services
{
public class VerificacionVeterinarioService
{
private readonly DataContext context;

    public VerificacionVeterinarioService(DataContext context)
    {
        this.context = context;
    }

    public async Task<bool> EstaAprobadoAsync(int idUsuario)
    {
        return await context.Veterinario.AnyAsync(v =>
            v.idUsuario == idUsuario &&
            v.estadoVerificacion == "aprobado");
    }
}

}