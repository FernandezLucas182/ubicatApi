
using System;
using System.ComponentModel.DataAnnotations;

namespace UbicatApi.Models
{
    public class QR
    {
        [Key]
        public int idQR { get; set; }

        public int idUsuario { get; set; }
        public int idMascota { get; set; }
        public string codigo { get; set; } = string.Empty;
        public string urlDatosMascota { get; set; } = string.Empty;
        public DateTime fechaGeneracion { get; set; }
        public string ubicacionActual { get; set; } = string.Empty;

        // Coordenadas de la última ubicación conocida
        public double? latitudActual { get; set; }
        public double? longitudActual { get; set; }

        public Mascota Mascota { get; set; } = null!;

        public DateTime? fechaUltimoEscaneo { get; set; }
    }
}
