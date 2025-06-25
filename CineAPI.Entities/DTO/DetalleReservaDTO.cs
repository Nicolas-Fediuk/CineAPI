using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineAPI.Entities.DTO
{
    public class DetalleReservaDTO
    {
        public string RESER_GUID { get; set; }
        public string RESER_USRCORREO { get; set; }
        public string PELI_TITULO { get; set; }
        public DateTime FUNCION_FECHA { get; set; }
        public string FUNCION_HORA { get; set; }
        public string SALA_NOMBRE { get; set; }
        public int ASIENTO_FILA { get; set; }
        public int ASIENTO_NRO { get; set; }
    }
}
