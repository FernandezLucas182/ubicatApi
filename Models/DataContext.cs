using Microsoft.EntityFrameworkCore;

namespace UbicatApi.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Mascota> Mascota { get; set; }
        public DbSet<QR> QR { get; set; }
        public DbSet<Categorias> Categorias { get; set; }
        public DbSet<Foro> Foro { get; set; }
        public DbSet<FotosForo> FotosForo { get; set; }
        public DbSet<Comentarios> Comentarios { get; set; }
        public DbSet<Validaciones> Validaciones { get; set; }
        public DbSet<ReporteMascota> ReporteMascota { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
            base.OnModelCreating(modelBuilder);

            // Relación QR 1:1 Mascota
            modelBuilder.Entity<QR>()
                .HasOne(q => q.Mascota)
                .WithOne(m => m.QR)
                .HasForeignKey<QR>(q => q.idMascota);

            // ReporteMascota -> Mascota (muchos a 1)
            modelBuilder.Entity<ReporteMascota>()
                .ToTable("reporte_mascota")
                .HasOne(r => r.Mascota)
                .WithMany()
                .HasForeignKey(r => r.idMascota)
                .OnDelete(DeleteBehavior.Cascade);

            // Forzar nombres EXACTOS según MySQL
            modelBuilder.Entity<Usuario>().ToTable("usuario");
            modelBuilder.Entity<Mascota>().ToTable("mascota");
            modelBuilder.Entity<QR>().ToTable("qr");
            modelBuilder.Entity<Categorias>().ToTable("categorias");
            modelBuilder.Entity<Foro>().ToTable("foro");
            modelBuilder.Entity<FotosForo>().ToTable("fotosforo");
            modelBuilder.Entity<Comentarios>().ToTable("comentarios");
            modelBuilder.Entity<Validaciones>().ToTable("validaciones");
        }
    }
}
