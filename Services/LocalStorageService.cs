using Microsoft.AspNetCore.Http;

namespace UbicatApi.Services
{
    public class LocalStorageService
    {
        private readonly IWebHostEnvironment env;

        public LocalStorageService(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public async Task<string?> GuardarImagen(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            string folder = Path.Combine(env.WebRootPath, "reportes");
            Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return "/reportes/" + fileName;
        }
    }
}
