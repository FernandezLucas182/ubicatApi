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
        public Mascota Mascota { get; set; }

        public int? idUsuarioReporta { get; set; }

        public string tipoReporte { get; set; }

        public string? ubicacion { get; set; }

        public string? mensaje { get; set; }

        // ==================================
        // NUEVA PROPIEDAD — FOTO DEL REPORTE
        // ==================================
        public string? foto { get; set; }

        public DateTime fecha { get; set; }
    }
}
