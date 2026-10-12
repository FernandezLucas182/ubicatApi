using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UbicatApi.DTOs;
using UbicatApi.Models;

namespace UbicatApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;

        public UsuarioController(
    DataContext context,
    IConfiguration configuration,
    IWebHostEnvironment env)
        {
            this.context = context;
            this.configuration = configuration;
            this.env = env;
        }

        // ====================================
        // 1. OBTENER TODOS (solo para debug)
        // ====================================
        // Obtener usuarios sin exponer contraseñas ni hashes
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await context.Usuario
                .Select(u => new
                {
                    u.idUsuario,
                    u.nombre,
                    u.apellido,
                    u.email,
                    u.telefono,
                    u.ubicacion,
                    u.rol,
                    u.especialidad,
                    u.clasificacionPromedio,
                    u.fotoPerfil
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        // ====================================
        // 2. PERFIL DEL USUARIO (JWT)
        // ====================================
        // Obtener perfil del usuario autenticado sin exponer el hash
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            if (!int.TryParse(User.Identity?.Name, out int id) || id <= 0)
                return Unauthorized("No se pudo identificar al usuario.");

            var usuario = await context.Usuario
                .Where(u => u.idUsuario == id)
                .Select(u => new
                {
                    u.idUsuario,
                    u.nombre,
                    u.apellido,
                    u.email,
                    u.telefono,
                    u.ubicacion,
                    u.rol,
                    u.especialidad,
                    u.clasificacionPromedio,
                    u.fotoPerfil
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            return Ok(usuario);
        }
        // ====================================
        // 3. EDITAR PERFIL
        // ====================================
        [Authorize]
        [HttpPut("edit")]
        public async Task<IActionResult> EditProfile([FromBody] UsuarioDto dto)
        {
            try
            {
                int id = int.Parse(User.Identity?.Name ?? "0");
                var u = await context.Usuario.FindAsync(id);

                if (u == null)
                    return NotFound("Usuario no encontrado");

                u.nombre = dto.nombre;
                u.apellido = dto.apellido;
                u.telefono = dto.telefono;
                u.email = dto.email;
                u.ubicacion = dto.ubicacion;

                context.Usuario.Update(u);
                await context.SaveChangesAsync();

                return Ok(new { mensaje = "Perfil actualizado correctamente", u });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ====================================
        // 4. CAMBIAR PASSWORD
        // ====================================
        [Authorize]
        [HttpPut("changePass")]
        public async Task<IActionResult> ChangePassword([FromBody] CambiarClaveDTO dto)
        {
            try
            {
                int id = int.Parse(User.Identity?.Name ?? "0");
                var u = await context.Usuario.FindAsync(id);

                if (u == null)
                    return NotFound("Usuario no encontrado");

                string hashedActual = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: dto.ClaveActual,
                    salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"] ?? ""),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));

                if (hashedActual != u.password)
                    return BadRequest("La clave actual es incorrecta");

                string hashedNueva = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: dto.ClaveNueva,
                    salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"] ?? ""),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));

                u.password = hashedNueva;
                context.Usuario.Update(u);
                await context.SaveChangesAsync();

                return Ok(new { mensaje = "Contraseña actualizada correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // ====================================
        // 4.5 HASH CONTRASEÑA (igual que API anterior)
        // ====================================
        [HttpPost("hashcontra")]
        public IActionResult HashContra([FromQuery] string clave)
        {
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: clave,
                    salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"] ?? ""),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));

                return Ok(new { hash = hashed });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        // ====================================
        // 5. LOGIN
        // ====================================
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginDto dto)
        {
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: dto.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"] ?? ""),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));

                var u = await context.Usuario.FirstOrDefaultAsync(x => x.email == dto.Usuario);

                if (u == null || u.password != hashed)
                    return BadRequest("Usuario o clave incorrecta");

                var secreto = configuration["TokenAuthentication:SecretKey"];
                var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(secreto));
                var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, u.idUsuario.ToString()),
                    new Claim("FullName", u.nombre + " " + u.apellido),
                    new Claim(ClaimTypes.Role, u.rol),
                };

                var token = new JwtSecurityToken(
                    issuer: configuration["TokenAuthentication:Issuer"],
                    audience: configuration["TokenAuthentication:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: credenciales
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    usuario = new
                    {
                        u.idUsuario,
                        u.nombre,
                        u.apellido,
                        u.email,
                        u.telefono,
                        u.ubicacion,
                        u.rol,
                        u.especialidad,
                        u.clasificacionPromedio,
                        u.fotoPerfil
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // ====================================
        // 6. REGISTRO DE USUARIO
        // ====================================
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromForm] UsuarioRegisterDto dto)
        {
            try
            {
                var rolesPermitidos = new[]
                {
                    "dueño",
                    "veterinario",
                    "rescatista"
                };

                if (string.IsNullOrWhiteSpace(dto.rol) ||
                    !rolesPermitidos.Contains(
                        dto.rol.Trim(),
                        StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest("El rol seleccionado no es válido.");
                }

                dto.rol = dto.rol.Trim().ToLowerInvariant();

                // validar email duplicado
                var existe = await context.Usuario
                    .FirstOrDefaultAsync(x => x.email == dto.email);

                if (existe != null)
                    return BadRequest("El correo ya está registrado.");


                // hashear contraseña (idéntico a tu API anterior)
                string hashedPass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: dto.password,
                    salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"] ?? ""),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));


                // Validar la foto de perfil
                var foto = dto.fotoPerfil;

                if (foto == null || foto.Length == 0)
                    return BadRequest("La foto de perfil es obligatoria.");

                if (foto.Length > 2 * 1024 * 1024)
                    return BadRequest("La foto no puede superar los 2 MB.");

                var tiposPermitidos = new Dictionary<string, string>
                {
                    { "image/jpeg", ".jpg" },
                    { "image/png", ".png" },
                    { "image/webp", ".webp" }
                };

                if (!tiposPermitidos.TryGetValue(
                        foto.ContentType.ToLowerInvariant(),
                        out string? extension))
                {
                    return BadRequest("Solo se permiten imágenes JPG, PNG o WebP.");
                }

                // Guardar con un nombre generado por el servidor
                string carpeta = Path.Combine(
                    env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"),
                    "uploads",
                    "perfiles"
                );

                Directory.CreateDirectory(carpeta);

                string nombreArchivo = $"{Guid.NewGuid():N}{extension}";
                string rutaArchivo = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaArchivo, FileMode.CreateNew))
                {
                    await foto.CopyToAsync(stream);
                }

                string rutaFoto = $"/uploads/perfiles/{nombreArchivo}";

                // crear usuario
                Usuario nuevo = new Usuario
                {
                    nombre = dto.nombre,
                    apellido = dto.apellido,
                    email = dto.email,
                    telefono = dto.telefono,
                    ubicacion = dto.ubicacion,
                    rol = dto.rol,
                    password = hashedPass,
                    clasificacionPromedio = 0,
                    fotoPerfil = rutaFoto
                };

                context.Usuario.Add(nuevo);
                await context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Usuario registrado correctamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
