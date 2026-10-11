using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace UbicatApi.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task EnviarEmail(string email, string asunto, string mensaje)
    {
        var remitente = _configuration["Email:Remitente"];
        var contraseña = _configuration["Email:ContrasenaAplicacion"];

        if (string.IsNullOrWhiteSpace(remitente) ||
            string.IsNullOrWhiteSpace(contraseña))
        {
            throw new InvalidOperationException(
                "Falta configurar el correo remitente en la configuración.");
        }

        var correo = new MimeMessage();
        correo.From.Add(MailboxAddress.Parse(remitente));
        correo.To.Add(MailboxAddress.Parse(email));
        correo.Subject = asunto;
        correo.Body = new TextPart("plain")
        {
            Text = mensaje
        };

        using var cliente = new SmtpClient();

        await cliente.ConnectAsync(
            "smtp.gmail.com",
            587,
            SecureSocketOptions.StartTls);

        await cliente.AuthenticateAsync(remitente, contraseña);
        await cliente.SendAsync(correo);
        await cliente.DisconnectAsync(true);
    }
}