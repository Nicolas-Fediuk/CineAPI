using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CineAPI.Entities.DTO
{
    public class FuncionDTO
    {
        public string FUNCION_PELICULA { get; set; }
        public int FUNCION_SALA { get; set; }
        public DateTime FUNCION_FECHA { get; set; }
        public string FUNCION_HORA { get; set; }
        public string FUNCION_DURACION { get; set; }

        [JsonIgnore]
        public TimeSpan FUNCION_HORA_TS => TimeSpan.Parse(FUNCION_HORA);
        [JsonIgnore]
        public TimeSpan FUNCION_DURACION_TS => TimeSpan.Parse(FUNCION_DURACION);
    }
}
