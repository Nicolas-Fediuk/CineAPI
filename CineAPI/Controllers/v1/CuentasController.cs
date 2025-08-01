using AutoMapper;
using CineAPI.Datos;
using CineAPI.Datos.ADO.NET;
using CineAPI.Entities;
using CineAPI.Entities.DTO;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Transactions;

namespace CineAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v1/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly IConexion conexion;
        private readonly IDataProtector dataProtector;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;

        public CuentasController(IConexion conexion, IDataProtectionProvider dataProtectionProvider, IConfiguration configuration, IMapper mapper)
        {
            this.conexion = conexion;
            dataProtector = dataProtectionProvider.CreateProtector("3F2504E0-4F89-11D3-9A0C-0305E82C3301");
            this.configuration = configuration;
            this.mapper = mapper;
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Registrar(NuevoUsuarioDTO nuevoUsuarioDTO)
        {
            var PasswordEncriptado = dataProtector.Protect(nuevoUsuarioDTO.CREDEN_USR.CREDEN_PASSWORD);

            nuevoUsuarioDTO.CREDEN_USR.CREDEN_PASSWORD = PasswordEncriptado;

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var datosUsuarios = mapper.Map<Usuario>(nuevoUsuarioDTO);
                    await conexion.NuevoUsuarioDatos(datosUsuarios);

                    var credenciales = mapper.Map<Credenciales>(nuevoUsuarioDTO.CREDEN_USR);
                    await conexion.NuevoUsuarioCredenciales(credenciales);

                    var permisosUsuario = mapper.Map<RolesUsuario>(nuevoUsuarioDTO);
                    await conexion.NuevoPermisoUsuario(permisosUsuario);

                    scope.Complete();
                }
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }        

            var respuesta = await CrearToken(nuevoUsuarioDTO.CREDEN_USR);

            return respuesta;
        }

        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Login(CredencialesDTO credenciales)
        {
            var password = await conexion.GetPasswordUsr(credenciales.CREDEN_CORREO);

            if (password is null)
            {
                return BadRequest("Credenciales incorrectas");
            }

            password = dataProtector.Unprotect(password.ToString());

            if (credenciales.CREDEN_PASSWORD == password)
            {
                var credencialesProduccion = mapper.Map<Credenciales>(credenciales);
                var respuesta = await CrearToken(credencialesProduccion);
                return Ok(respuesta);
            }
            else
            {
                return BadRequest("Credencailes incorrectas");
            }
        }


        private async Task<RespuestaAutenticacionDTO> CrearToken(Credenciales credenciales)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", credenciales.CREDEN_CORREO)
            };

            //Para traerme todos los claims del usuaurio que estan en base
            var claimsDB = await conexion.GetClaims(credenciales.CREDEN_CORREO);

            claims.AddRange(claimsDB);

            //armamos el token
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            //Expiracion del token
            var expiracion = DateTime.UtcNow.AddYears(1);

            //Token armado
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);

            return new RespuestaAutenticacionDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Experacion = expiracion
            };
        }
    }
}
