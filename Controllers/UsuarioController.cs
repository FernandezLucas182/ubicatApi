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

        public UsuarioController(DataContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        // ====================================
        // 1. OBTENER TODOS (solo para debug)
        // ====================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await context.Usuario.ToListAsync());
        }

        // ====================================
        // 2. PERFIL DEL USUARIO (JWT)
        // ====================================
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            int id = int.Parse(User?.Identity?.Name ?? "0");

            if (id == 0)
                return BadRequest("No se pudo obtener ID del usuario");

            var u = await context.Usuario.FindAsync(id);

            return Ok(u);
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
                    usuario = u
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
        public async Task<IActionResult> Register([FromBody] UsuarioRegisterDto dto)
        {
            try
            {
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
                    clasificacionPromedio = 0
                };

                context.Usuario.Add(nuevo);
                await context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Usuario registrado correctamente",
                    usuario = nuevo
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
