using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CineAPI.Entities.DTO
{
    public class ListFuncionesDTO 
    {
        public string FUNCION_ID { get; set; }  
        public string FUNCION_PELICULA { get; set; }
        public string FUNCION_SALA { get; set; }
        public DateTime FUNCION_FECHA { get; set; }
        public string FUNCION_HORA { get; set; }
        public string FUNCION_DURACION { get; set; }
    }
}
