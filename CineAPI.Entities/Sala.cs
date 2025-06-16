using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineAPI.Entities
{
    public class Sala
    {
        public int SALA_ID { get; set; }
        public string SALA_NOMBRE { get; set; } = null!;
        public int SALA_CAPACIDAD{ get; set; }
    }
}
