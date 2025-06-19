using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineAPI.Entities.DTO
{
    public class SalasConAsientosDTO
    {
        public string SALA_NOMBRE { get; set; }
        public List<AsientosDTO> Asientos { get; set; }
    }
}
