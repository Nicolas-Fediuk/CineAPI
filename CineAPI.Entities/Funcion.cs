using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CineAPI.Entities
{
    public class Funcion
    {
        public string FUNCION_GUID { get; set; }
        public string FUNCION_PELIGUID { get; set; }
        public int FUNCION_SALAID { get; set; }
        public DateTime FUNCION_FECHA { get; set; }
        public string FUNCION_HORA { get; set; }
        public string FUNCION_DURACION { get; set; }

    }
}
