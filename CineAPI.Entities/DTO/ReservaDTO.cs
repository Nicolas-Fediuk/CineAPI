using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineAPI.Entities.DTO
{
    public class ReservaDTO
    {
        public string RESER_FUNCIONGUID { get; set; }
        public List<string> RESER_ASIENTOS { get; set; }
    }
}
