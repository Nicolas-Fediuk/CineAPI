using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineAPI.Entities
{
    public class Usuario
    {
        public string USER_GUID { get; set; }
        public string USER_NOMBRE { get; set; }
        public string USER_APELLIDO { get; set; }
        public string USER_CORREO { get; set; }
        public DateTime USER_FECNAC { get; set; }
        public string USER_TELEFONO { get; set; }
        public int USER_GENERO { get; set; }
        public bool USER_RECALERT { get; set; }
    }
}
