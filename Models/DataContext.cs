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
        public DbSet<Veterinario> Veterinario { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

            // Relación QR 1:1 Mascota
            modelBuilder.Entity<QR>()
                .HasOne(q => q.Mascota)
                .WithOne(m => m.QR)
                .HasForeignKey<QR>(q => q.idMascota);

            // ReporteMascota -> Mascota
            modelBuilder.Entity<ReporteMascota>()
                .ToTable("reporte_mascota")
                .HasOne(r => r.Mascota)
                .WithMany()
                .HasForeignKey(r => r.idMascota)
                .OnDelete(DeleteBehavior.Cascade);

            // Veterinario -> Usuario (relación 1:1)
            modelBuilder.Entity<Veterinario>()
                .ToTable("veterinario")
                .HasOne(v => v.Usuario)
                .WithOne()
                .HasForeignKey<Veterinario>(v => v.idUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Veterinario>()
                .HasIndex(v => v.idUsuario)
                .IsUnique();

            // Matrícula única dentro de cada jurisdicción
            modelBuilder.Entity<Veterinario>()
                .HasIndex(v => new { v.jurisdiccion, v.matricula })
                .IsUnique();

            // Forzar nombres exactos según MySQL
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
