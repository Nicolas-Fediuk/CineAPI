using AutoMapper;
using CineAPI.Datos;
using CineAPI.Entities.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v1/usuarios")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ADMIN")]
    public class UsuariosController : ControllerBase
    {
        private readonly IConexion conexion;
        private readonly IMapper mapper;

        
        public UsuariosController(IConexion conexion, IMapper mapper) 
        {
            this.conexion = conexion;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<List<UsuarioDTO>> Get()
        {
            var usuarios = await conexion.GetUsuarios();

            var usuariosDTO = mapper.Map<List<UsuarioDTO>>(usuarios);

            usuariosDTO = await conexion.BuscarGeneroDescrip(usuariosDTO);

            return usuariosDTO;
        }

        [HttpPut("/hacerAdmin/{correo}")]
        public async Task<ActionResult> HacerAdmin(string correo)
        {
            var existe = await conexion.ExisteUsuario(correo);

            if(!existe) 
            {
                return BadRequest("El correo ingresado no existe");
            }

            conexion.HacerAdmin(correo);

            return Ok();
        }
    }
}
