using AutoMapper;
using CineAPI.Datos;
using CineAPI.Datos.ADO.NET;
using CineAPI.Entities;
using CineAPI.Entities.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CineAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v1/funciones")]
    public class FuncionesController : ControllerBase
    {
        private readonly IConexion conexion;
        private readonly IMapper mapper;

        public FuncionesController(IConexion conexion, IMapper mapper)
        {
            this.conexion = conexion;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListFuncionesDTO>>> Get()
        {
            var listadoFuncionesDTO = new List<ListFuncionesDTO>();

            var funciones = await conexion.GetFunciones();

            foreach (var funcionDTO in funciones)
            {
                var nombrePeli = await conexion.GetPeliculaNombreGUID(funcionDTO.FUNCION_PELIGUID);

                if (nombrePeli is null)
                {
                    return StatusCode(500, "Error al consultar los datos");
                }
                if (nombrePeli == "")
                {
                    return BadRequest("Nombre de la pelicula no encontrado");
                }

                var salaPeli = await conexion.GetSalaNombreGUID(funcionDTO.FUNCION_SALAID);

                if (salaPeli is null)
                {
                    return StatusCode(500, "Error al consultar los datos");
                }
                if (salaPeli == "")
                {
                    return BadRequest("Nombre de la sala no encontrado");
                }

                listadoFuncionesDTO.Add(new ListFuncionesDTO
                {
                    FUNCION_ID = funcionDTO.FUNCION_GUID,
                    FUNCION_PELICULA = nombrePeli,
                    FUNCION_SALA = salaPeli,
                    FUNCION_FECHA = funcionDTO.FUNCION_FECHA,
                    FUNCION_HORA = funcionDTO.FUNCION_HORA,
                    FUNCION_DURACION = funcionDTO.FUNCION_DURACION

                });
            }

            return listadoFuncionesDTO;
        }


        [HttpPost]
        public async Task<ActionResult> Post(FuncionDTO funcionDTO)
        {
            if (funcionDTO.FUNCION_PELICULA is null)
            {
                return BadRequest("Ingrese el nombre de la pelicula");
            }
            if (funcionDTO.FUNCION_SALA == 0)
            {
                return BadRequest("Ingrese una ID de sala");
            }

            var funcion = new Funcion
            {
                FUNCION_GUID = Guid.NewGuid().ToString(),
                FUNCION_PELIGUID = funcionDTO.FUNCION_PELICULA,
                FUNCION_SALAID = funcionDTO.FUNCION_SALA,
                FUNCION_FECHA = funcionDTO.FUNCION_FECHA.Date,
                FUNCION_HORA = funcionDTO.FUNCION_HORA,
                FUNCION_DURACION = funcionDTO.FUNCION_DURACION
            };

            await conexion.NuevaFuncion(funcion);

            return Ok();
        }

        [HttpDelete("id")]
        public async Task<ActionResult> Delete(string idFuncion)
        {
            if (idFuncion is null)
            {
                return BadRequest("Ingrese un ID de funcion");
            }

            if (!await conexion.ExisteFuncion(idFuncion))
            {
                return NotFound($"No existe la función con ID: {idFuncion}");
            }

            bool eliminado = await conexion.EliminarFuncion(idFuncion);

            if (eliminado)
            {
                return Ok("Funcion eliminada");
            }
            else
            {
                return StatusCode(500, "Error al eliminar la funcion");
            }
        }

        [HttpPut("funcionId")]
        public async Task<ActionResult<FuncionDTO>> Put(string funcionId, FuncionDTO funcionDTO)
        {
            if(funcionId is null)
            {
                return BadRequest("Ingrese un ID de función");
            }

            if (!await conexion.ExisteFuncion(funcionId))
            {
                return NotFound($"No existe la función con ID: {funcionId}");
            }

            conexion.EditarFuncion(funcionId, funcionDTO);

            return Ok(funcionDTO);
        }
    }
}
