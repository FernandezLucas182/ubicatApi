using Microsoft.AspNetCore.Http;

namespace UbicatApi.Services
{
    public class LocalFileService
    {
        private readonly IWebHostEnvironment _env;

        public LocalFileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveReporteImage(IFormFile file)
        {
            var year = DateTime.Now.Year.ToString();
            var month = DateTime.Now.Month.ToString("D2");

            var folder = Path.Combine(_env.WebRootPath, "uploads", "reportes", year, month);
            Directory.CreateDirectory(folder);

            var name = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);
            var path = Path.Combine(folder, name);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/reportes/{year}/{month}/{name}";
        }
    }
}
