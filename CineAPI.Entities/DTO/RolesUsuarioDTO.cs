using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineAPI.Entities.DTO
{
    public class RolesUsuarioDTO
    {
        public string ROLUSER_CORREO { get; set; }
        public List<Rol> ROLUSER_ROLID { get; set; }
    }
}
