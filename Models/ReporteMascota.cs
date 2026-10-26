
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UbicatApi.Models
{
    public class ReporteMascota
    {
        [Key]
        public int idReporte { get; set; }

        public int idMascota { get; set; }

        [ForeignKey("idMascota")]
        public Mascota Mascota { get; set; } = null!;

        public int? idUsuarioReporta { get; set; }

        public string tipoReporte { get; set; } = string.Empty;

        public string? ubicacion { get; set; }

        // Coordenadas del avistamiento
        public double? latitud { get; set; }
        public double? longitud { get; set; }

        public string? mensaje { get; set; }
        public string? foto { get; set; }

        public DateTime fecha { get; set; }

        public string? emailReportante { get; set; }
        public string? telefonoReportante { get; set; }
    }
}
