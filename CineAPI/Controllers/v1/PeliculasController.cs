using AutoMapper;
using CineAPI.Datos;
using CineAPI.Datos.ADO.NET;
using CineAPI.Entities;
using CineAPI.Entities.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.Servicios;

namespace CineAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v1/peliculas")]
    public class PeliculasController : ControllerBase
    {
        private readonly IConexion conexion;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly IMapper mapper;
        private const string contenedor = "posterPeliculas";

        public PeliculasController(IConexion conexion, IAlmacenadorArchivos almacenadorArchivos, IMapper mapper)
        {
            this.conexion = conexion;
            this.almacenadorArchivos = almacenadorArchivos;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<List<PeliculasDTO>> Get()
        {
            var peliculas = await conexion.GetPeliculas();

            var peliculasDTO = mapper.Map<List<PeliculasDTO>>(peliculas);

            return peliculasDTO;
        }

        [HttpGet("peliculasId")]
        public async Task<List<PeliculasIdDTO>> GetPeluculasId()
        {
            var peliculas = await conexion.GetPeliculasId();

            return peliculas.ToList();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] NuevaPeliculaDTO nuevaPeliculaDTO)
        {
            var url = await almacenadorArchivos.Almacenar(contenedor, nuevaPeliculaDTO.PELI_POSTER);

            var pelicula = mapper.Map<Pelicula>(nuevaPeliculaDTO);

            pelicula.PELI_GUID = Guid.NewGuid().ToString();
            pelicula.PELI_POSTER = url;

            try
            {               
                conexion.NuevaPelicula(pelicula);
                return Ok(pelicula);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear la pelicula: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<ActionResult> ModificarPelicula(string nombre,[FromForm] NuevaPeliculaDTO peliculaDTO)
        {
            if(nombre is null)
            {
                return BadRequest("Ingrese el nombre de la pelicula a modificar");
            }

            var peliDBGUID = await conexion.GetPeliculaGUIDNombre(nombre);

            if(peliDBGUID is null)
            {
                return BadRequest("Nombre de la pelicula inexistente");
            }

            var pelicula = mapper.Map<Pelicula>(peliculaDTO);


            if (peliculaDTO.PELI_POSTER is not null)
            {
                var fotoActual = await conexion.GetPosterGuid(peliDBGUID);
                var url = await almacenadorArchivos.Editar(fotoActual, contenedor, peliculaDTO.PELI_POSTER);
                pelicula.PELI_POSTER = url;
            }

            try
            {
                await conexion.PutPelicula(peliDBGUID, pelicula);
                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Error al modificar la pelicula: {ex.Message}");
            }
        }

        [HttpDelete("{nombre}")]
        public async Task<ActionResult> EliminarPelicula(string nombre)
        {
            if (nombre is null)
            {
                return BadRequest("Ingrese el nombre de la pelicula a modificar");
            }

            var peliDBGUID = await conexion.GetPeliculaGUIDNombre(nombre);

            if (peliDBGUID is null)
            {
                return BadRequest("Nombre de la pelicula inexistente");
            }

            try
            {
                conexion.EliminarPelicula(peliDBGUID);
                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500, "Error al eliminar la pelicula");
            }
        }
    }
}
