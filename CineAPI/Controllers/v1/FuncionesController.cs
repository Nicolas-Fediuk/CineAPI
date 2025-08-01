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

                var salaPeli = await conexion.GetSalaNombreGUID(funcionDTO.FUNCION_SALAID);

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
            var funcion = mapper.Map<Funcion>(funcionDTO);

            funcion.FUNCION_GUID = Guid.NewGuid().ToString();  

            await conexion.NuevaFuncion(funcion);

            return Ok();
        }

        [HttpDelete("id")]
        public async Task<ActionResult> Delete(string idFuncion)
        {

            if (!await conexion.ExisteFuncion(idFuncion))
            {
                return NotFound($"No existe la función con ID: {idFuncion}");
            }

            await conexion.EliminarFuncion(idFuncion);

            return Ok("Función eliminada");
        }

        [HttpPut("funcionId")]
        public async Task<ActionResult<FuncionDTO>> Put(string funcionId, FuncionDTO funcionDTO)
        {
            if (!await conexion.ExisteFuncion(funcionId))
            {
                return NotFound($"No existe la función con ID: {funcionId}");
            }

            var funcion = mapper.Map<Funcion>(funcionDTO);

            await conexion.EditarFuncion(funcionId, funcion);

            return Ok(funcionDTO);
        }
    }
}
