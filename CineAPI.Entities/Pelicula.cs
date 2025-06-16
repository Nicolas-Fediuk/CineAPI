using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineAPI.Entities
{
    public class Pelicula
    {
        public string PELI_GUID { get; set; }
        public string PELI_TITULO { get; set; }
        public string PELI_DESCRIP { get; set; }
        public string PELI_POSTER { get; set; }
        public int PELI_GENEROID { get; set; }
    }
}
