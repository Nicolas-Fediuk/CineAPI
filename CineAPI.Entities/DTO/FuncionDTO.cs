using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CineAPI.Entities.DTO
{
    public class FuncionDTO
    {
        [Required(ErrorMessage = "El ID de pelicula es requerido")]
        public string FUNCION_PELIGUID { get; set; }

        [Required]
        public int FUNCION_SALAID { get; set; }

        [Required]
        public DateTime FUNCION_FECHA { get; set; }

        [Required]
        public string FUNCION_HORA { get; set; }

        [Required]
        public string FUNCION_DURACION { get; set; }
    }
}
