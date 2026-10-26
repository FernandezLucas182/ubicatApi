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
        public async Task<IActionResult> EditProfile(
            [FromBody] UsuarioEditarPerfilDto dto)
        {
            try
            {
                if (!int.TryParse(User.Identity?.Name, out int id))
                    return Unauthorized();

                var u = await context.Usuario.FindAsync(id);

                if (u == null)
                    return NotFound("Usuario no encontrado");

                u.nombre = dto.nombre;
                u.apellido = dto.apellido;
                u.telefono = dto.telefono;
                u.email = dto.email;
                u.ubicacion = dto.ubicacion;

                await context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Perfil actualizado correctamente",
                    u
                });
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
                // Impedir el ingreso de veterinarios no aprobados.
                if (string.Equals(
                        u.rol,
                        "veterinario",
                        StringComparison.OrdinalIgnoreCase))
                {
                    var veterinario = await context.Veterinario
                        .FirstOrDefaultAsync(v => v.idUsuario == u.idUsuario);

                    if (veterinario == null)
                    {
                        return Unauthorized(
                            "Tu documentación profesional no está registrada.");
                    }

                    if (!string.Equals(
                            veterinario.estadoVerificacion,
                            "aprobado",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.Equals(
                                veterinario.estadoVerificacion,
                                "rechazado",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            return Unauthorized(
                                "Tu documentación fue rechazada. Contactá al administrador para conocer cómo corregirla.");
                        }

                        return Unauthorized(
                            "Tu documentación está pendiente de revisión. Podrás ingresar cuando sea aprobada.");
                    }
                }

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
            string? rutaFotoFisica = null;
            string? rutaDocumentacionFisica = null;

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

                // Validaciones exclusivas para veterinarios
                if (dto.rol == "veterinario")
                {
                    if (!dto.ofreceServiciosPagos && !dto.ofreceVoluntariado)
                    {
                        return BadRequest(
                            "Debés seleccionar al menos una modalidad de atención.");
                    }
                    if (string.IsNullOrWhiteSpace(dto.matricula))
                        return BadRequest("La matrícula profesional es obligatoria.");

                    if (string.IsNullOrWhiteSpace(dto.jurisdiccion))
                        return BadRequest("La jurisdicción es obligatoria.");

                    if (dto.documentacion == null ||
                        dto.documentacion.Length == 0)
                    {
                        return BadRequest(
                            "Debés adjuntar la documentación profesional.");
                    }

                    if (dto.documentacion.Length > 5 * 1024 * 1024)
                        return BadRequest(
                            "La documentación no puede superar los 5 MB.");
                }

                // Validar correo duplicado
                var existe = await context.Usuario
                    .AnyAsync(x => x.email == dto.email);

                if (existe)
                    return BadRequest("El correo ya está registrado.");

                // Validar foto de perfil
                var foto = dto.fotoPerfil;

                if (foto == null || foto.Length == 0)
                    return BadRequest("La foto de perfil es obligatoria.");

                if (foto.Length > 2 * 1024 * 1024)
                    return BadRequest("La foto no puede superar los 2 MB.");

                var tiposImagen = new Dictionary<string, string>
                {
                    { "image/jpeg", ".jpg" },
                    { "image/png", ".png" },
                    { "image/webp", ".webp" }
                };

                if (!tiposImagen.TryGetValue(
                        foto.ContentType.ToLowerInvariant(),
                        out string? extensionFoto))
                {
                    return BadRequest(
                        "Solo se permiten imágenes JPG, PNG o WebP para el perfil.");
                }

                // Hashear contraseña
                string hashedPass = Convert.ToBase64String(
                    KeyDerivation.Pbkdf2(
                        password: dto.password,
                        salt: System.Text.Encoding.ASCII.GetBytes(
                            configuration["Salt"] ?? ""),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));

                string raizWeb = env.WebRootPath ??
                    Path.Combine(env.ContentRootPath, "wwwroot");

                // Guardar foto de perfil
                string carpetaPerfiles = Path.Combine(
                    raizWeb, "uploads", "perfiles");

                Directory.CreateDirectory(carpetaPerfiles);

                string nombreFoto = $"{Guid.NewGuid():N}{extensionFoto}";
                rutaFotoFisica = Path.Combine(carpetaPerfiles, nombreFoto);

                await using (var stream = new FileStream(
                    rutaFotoFisica, FileMode.CreateNew))
                {
                    await foto.CopyToAsync(stream);
                }

                string rutaFoto = $"/uploads/perfiles/{nombreFoto}";

                // Guardar documentación profesional si es veterinario
                string? rutaDocumentacion = null;

                if (dto.rol == "veterinario")
                {
                    var tiposDocumentacion = new Dictionary<string, string>
                    {
                        { "application/pdf", ".pdf" },
                        { "image/jpeg", ".jpg" },
                        { "image/png", ".png" },
                        { "image/webp", ".webp" }
                    };

                    var documento = dto.documentacion!;

                    if (!tiposDocumentacion.TryGetValue(
                            documento.ContentType.ToLowerInvariant(),
                            out string? extensionDocumento))
                    {
                        return BadRequest(
                            "La documentación debe ser PDF, JPG, PNG o WebP.");
                    }

                    string carpetaVeterinarios = Path.Combine(
                        raizWeb, "uploads", "veterinarios");

                    Directory.CreateDirectory(carpetaVeterinarios);

                    string nombreDocumento =
                        $"{Guid.NewGuid():N}{extensionDocumento}";

                    rutaDocumentacionFisica = Path.Combine(
                        carpetaVeterinarios, nombreDocumento);

                    await using (var stream = new FileStream(
                        rutaDocumentacionFisica, FileMode.CreateNew))
                    {
                        await documento.CopyToAsync(stream);
                    }

                    rutaDocumentacion =
                        $"/uploads/veterinarios/{nombreDocumento}";
                }

                // Guardar usuario y datos profesionales en una transacción
                await using var transaccion =
                    await context.Database.BeginTransactionAsync();

                try
                {
                    var nuevo = new Usuario
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

                    if (dto.rol == "veterinario")
                    {
                        var veterinario = new Veterinario
                        {
                            idUsuario = nuevo.idUsuario,
                            matricula = dto.matricula!.Trim(),
                            jurisdiccion = dto.jurisdiccion!.Trim(),
                            documentacion = rutaDocumentacion,
                            estadoVerificacion = "pendiente",
                            ofreceServiciosPagos = dto.ofreceServiciosPagos,
                            ofreceVoluntariado = dto.ofreceVoluntariado
                        };

                        context.Veterinario.Add(veterinario);
                        await context.SaveChangesAsync();
                    }

                    await transaccion.CommitAsync();
                }
                catch
                {
                    await transaccion.RollbackAsync();
                    throw;
                }

                return Ok(new
                {
                    mensaje = dto.rol == "veterinario"
                        ? "Registro recibido. Tu matrícula y documentación están pendientes de verificación."
                        : "Usuario registrado correctamente"
                });
            }
            catch (DbUpdateException)
            {
                // Evitar dejar archivos huérfanos si falla la base de datos
                EliminarArchivoSiExiste(rutaFotoFisica);
                EliminarArchivoSiExiste(rutaDocumentacionFisica);

                return BadRequest(
                    "No se pudo completar el registro. Verificá los datos e intentá nuevamente.");
            }
            catch (Exception)
            {
                EliminarArchivoSiExiste(rutaFotoFisica);
                EliminarArchivoSiExiste(rutaDocumentacionFisica);

                return StatusCode(500,
                    "Ocurrió un error al registrar el usuario.");
            }
        }

        private static void EliminarArchivoSiExiste(string? ruta)
        {
            if (!string.IsNullOrWhiteSpace(ruta) &&
                System.IO.File.Exists(ruta))
            {
                System.IO.File.Delete(ruta);
            }
        }



        // ====================================
        // 7. ACTUALIZAR FOTO DE PERFIL
        // ====================================
        [Authorize]
        [HttpPut("profile/foto")]
        public async Task<IActionResult> ActualizarFotoPerfil(
            [FromForm] IFormFile? fotoPerfil)
        {
            if (!int.TryParse(User.Identity?.Name, out int id) || id <= 0)
                return Unauthorized("No se pudo identificar al usuario.");

            if (fotoPerfil == null || fotoPerfil.Length == 0)
                return BadRequest("Tenés que seleccionar una imagen.");

            if (fotoPerfil.Length > 2 * 1024 * 1024)
                return BadRequest("La foto no puede superar los 2 MB.");

            var tiposPermitidos = new Dictionary<string, string>
            {
                { "image/jpeg", ".jpg" },
                { "image/png", ".png" },
                { "image/webp", ".webp" }
            };

            if (!tiposPermitidos.TryGetValue(
                    fotoPerfil.ContentType.ToLowerInvariant(),
                    out string? extension))
            {
                return BadRequest("Solo se permiten imágenes JPG, PNG o WebP.");
            }

            var usuario = await context.Usuario.FindAsync(id);

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            string carpeta = Path.Combine(
                env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"),
                "uploads",
                "perfiles"
            );

            Directory.CreateDirectory(carpeta);

            string nombreArchivo = $"{Guid.NewGuid():N}{extension}";
            string rutaArchivo = Path.Combine(carpeta, nombreArchivo);
            string? fotoAnterior = usuario.fotoPerfil;

            try
            {
                using (var stream = new FileStream(
                           rutaArchivo, FileMode.CreateNew))
                {
                    await fotoPerfil.CopyToAsync(stream);
                }

                usuario.fotoPerfil = $"/uploads/perfiles/{nombreArchivo}";
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (System.IO.File.Exists(rutaArchivo))
                    System.IO.File.Delete(rutaArchivo);

                return StatusCode(500,
                    "No se pudo guardar la foto de perfil.");
            }

            // Eliminar la foto anterior solo después de guardar la nueva.
            if (!string.IsNullOrWhiteSpace(fotoAnterior))
            {
                string nombreAnterior = Path.GetFileName(fotoAnterior);

                if (!string.IsNullOrWhiteSpace(nombreAnterior))
                {
                    string rutaAnterior = Path.Combine(
                        carpeta, nombreAnterior);

                    if (System.IO.File.Exists(rutaAnterior) &&
                        !string.Equals(
                            rutaAnterior,
                            rutaArchivo,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            System.IO.File.Delete(rutaAnterior);
                        }
                        catch (IOException)
                        {
                            // La actualización ya se guardó en la base.
                        }
                        catch (UnauthorizedAccessException)
                        {
                            // La actualización ya se guardó en la base.
                        }
                    }
                }
            }

            return Ok(new
            {
                mensaje = "Foto de perfil actualizada correctamente.",
                fotoPerfil = usuario.fotoPerfil
            });
        }


    }
}
