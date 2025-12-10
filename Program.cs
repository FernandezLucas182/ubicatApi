using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using UbicatApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Controladores + evitar ciclos JSON
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var config = builder.Configuration;

// =====================
// CONFIGURAR JWT
// =====================
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

// =====================
// CONFIGURAR DB CONTEXT
// =====================
builder.Services.AddDbContext<DataContext>(
    options => options.UseMySql(
        config["ConnectionStrings:DefaultConnection"],
        ServerVersion.AutoDetect(config["ConnectionStrings:DefaultConnection"])
    )
);

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

app.MapControllers();

app.Run();
