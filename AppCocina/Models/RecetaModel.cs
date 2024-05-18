using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCocina.Models
{
    public class RecetaModel
    {
        public string Nombre { get; set; } = null!;
        public bool Favorito { get; set; } = false;
        public string Calificacion { get; set; } = null!;
        public float Tiempo { get; set; }
        public float Calorias { get; set; }
        public string Foto { get; set; } = null!;
        public string Ingredientes { get; set; } = null!;
        public string Preparacion { get; set; } = null!;

    }
}
