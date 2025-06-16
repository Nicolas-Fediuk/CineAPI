using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineAPI.Entities
{
    public class RolesUsuario
    {
        public int ROLUSER_ID { get; set; }
        public string ROLUSER_CORREO { get; set; }
        public List<Rol> ROLUSER_ROLID { get; set; }
    }
}
