using CineAPI.Datos;
using CineAPI.Datos.ADO.NET;
using CineAPI.Entities;
using CineAPI.Entities.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CineAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v1/salas")]
    public class SalasController : ControllerBase
    {
        private readonly IConexion conexion;

        public SalasController(IConexion conexion)
        {
            this.conexion = conexion;
        }

        [HttpGet]
        public async Task<List<SalasConAsientosDTO>> Get()
        {
            return await conexion.GetSalasConAsientos();
        }
    }
}
