using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using UbicatApi.Models;
using UbicatApi.Services;


var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

// =====================================================
// 1) CONTROLADORES / JSON
// =====================================================
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =====================================================
// 2) CONFIGURAR JWT
// =====================================================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secreto = config["TokenAuthentication:SecretKey"];
        if (string.IsNullOrEmpty(secreto))
            throw new Exception("Falta configurar TokenAuthentication:Secret");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["TokenAuthentication:Issuer"],
            ValidAudience = config["TokenAuthentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(secreto)),
        };
    });

// =====================================================
// 3) CONFIGURAR ENTITY FRAMEWORK + MySQL
// =====================================================
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(
        config["ConnectionStrings:DefaultConnection"],
        ServerVersion.AutoDetect(config["ConnectionStrings:DefaultConnection"])
    )
);

// =====================================================
// 4) REGISTRAR SERVICIOS PERSONALIZADOS (SMTP + Cloudinary)
// =====================================================
builder.Services.AddScoped<EmailService>();          // Servicio de envío de emails
builder.Services.AddScoped<LocalFileService>();



// =====================================================
// 5) BUILD & MIDDLEWARE
// =====================================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();


app.MapControllers();

app.Run();
