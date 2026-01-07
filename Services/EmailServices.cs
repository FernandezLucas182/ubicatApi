namespace UbicatApi.Services;

public class EmailService
{
    public Task EnviarEmail(string email, string asunto, string mensaje)
    {
        // Servicio simulado por ahora
        Console.WriteLine($"EMAIL -> {email} | {asunto} | {mensaje}");
        return Task.CompletedTask;
    }
}
