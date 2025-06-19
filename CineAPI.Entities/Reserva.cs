using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineAPI.Entities
{
    public class Reserva
    {
        public string RESER_GUID { get; set; }
        public string RESER_USRCORREO { get; set; }
        public string RESER_FUNCIONGUID { get; set; }
        public DateTime RESER_FECHA { get; set; }
    }
}
