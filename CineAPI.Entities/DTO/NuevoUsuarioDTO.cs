using CineAPI.Validaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineAPI.Entities.DTO
{
    public class NuevoUsuarioDTO
    {
        [Required(ErrorMessage = "El campo nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El campo nombre debe te tener una logitud mínima de 3 caracteres a 100.")]
        public string USER_NOMBRE { get; set; }

        [Required(ErrorMessage = "El campo apellido es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El campo apellido debe te tener una logitud mínima de 3 caracteres a 100.")]
        public string USER_APELLIDO { get; set; }

        [Required(ErrorMessage = "El campo fecha de nacimiento es obligatorio.")]
        [DataType(DataType.DateTime, ErrorMessage = "Formato de fecha inválido.")]
        [FechaDeNacimientoValida]
        public DateTime USER_FECNAC { get; set; }

        [Phone]
        public string USER_TELEFONO { get; set; }

        public int USER_GENERO { get; set; }

        [Required(ErrorMessage = "El campo recibe alerta es obligatorio.")]
        public bool USER_RECALERT { get; set; }
        public Credenciales CREDEN_USR { get; set; }

        [Required(ErrorMessage = "Se tiene que ingresar al menos un rol.")]
        public List<int> ROLUSER_ROLES { get; set; }
    }
}
