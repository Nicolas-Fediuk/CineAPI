using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineAPI.Entities.DTO
{
    public class UsuarioDTO
    {
        public string USER_NOMBRE { get; set; }
        public string USER_APELLIDO { get; set; }
        public string USER_CORREO { get; set; }
        public DateTime USER_FECNAC { get; set; }
        public string USER_TELEFONO { get; set; }
        public string USER_GENERO { get; set; }
        public bool USER_RECALERT { get; set; }
    }
}
