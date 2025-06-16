using CineAPI.Entities.Validaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineAPI.Entities
{
    public class Credenciales
    {
        [Required]
        [EmailAddress]
        public string CREDEN_CORREO { get; set; }
        [Required]
        [Password]
        public string CREDEN_PASSWORD { get; set; }
    }
}
