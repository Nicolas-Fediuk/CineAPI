using AutoMapper;
using CineAPI.Datos;
using CineAPI.Entities;
using CineAPI.Entities.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Transactions;

namespace CineAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v1/reservas")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ADMIN")]
    public class ReservasController : ControllerBase
    {
        private readonly IConexion conexion;
        private readonly IMapper mapper;
        private readonly ILogger logger;

        public ReservasController(IConexion conexion, IMapper mapper) 
        {
            this.conexion = conexion;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<List<DetalleReservaDTO>> Get()
        {
            IEnumerable<DetalleReservaDTO> reservas = await conexion.GetReservas();

            return reservas.ToList();    
        }

        [HttpGet("miReserva")]
        public async Task<List<DetalleReservaDTO>> GetPorUsuario()
        {
            var correo = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            IEnumerable<DetalleReservaDTO> reservas = await conexion.GetReservasPorUsuario(correo.ToString());

            return reservas.ToList();
        }


        [HttpGet("asientosDisponibles")]
        public async Task<ActionResult<List<AsientosDTO>>> GetDisponibilidad(string funcion)
        {
            if (!await conexion.ExisteFuncion(funcion))
            {
                return NotFound("La función ingresada no existe");
            }

            var asientosDisponibles = await conexion.GetAsientosDisponibles(funcion);

            var asientosDTO = mapper.Map<List<AsientosDTO>>(asientosDisponibles);

            return Ok(asientosDTO);
        }

        [HttpPost]
        public async Task<ActionResult> Post(ReservaDTO reservaDTO)
        {
            if (await VerificarAsientos(reservaDTO))
            {
                return BadRequest("Asiento no disponible");
            }

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Reserva reserva = new Reserva();
                    reserva.RESER_GUID = Guid.NewGuid().ToString();
                    reserva.RESER_USRCORREO = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                    reserva.RESER_FECHA = DateTime.Now;
                    reserva.RESER_FUNCIONGUID = reservaDTO.RESER_FUNCIONGUID;

                    await conexion.CargarReserva(reserva);

                    foreach (var asiento in reservaDTO.RESER_ASIENTOS)
                    {
                        AsientoReservado asientoReservado = new AsientoReservado();
                        asientoReservado.ARESER_FUNCIONGUID = reservaDTO.RESER_FUNCIONGUID;
                        asientoReservado.ARESER_RESERGUID = reserva.RESER_GUID;
                        asientoReservado.ARESER_ASIENTOGUID = asiento;

                        await conexion.CargarAsientosReservados(asientoReservado);
                    }

                    scope.Complete();
                }               

                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Error al grabar la reserva: {ex.Message}");
            }
        }

        private async Task<bool> VerificarAsientos(ReservaDTO reservaDTO)
        {
            foreach(var asiento in reservaDTO.RESER_ASIENTOS)
            {
                if(await conexion.ExisteReserva(reservaDTO.RESER_FUNCIONGUID, asiento))
                {
                    return true;
                }
            }

            return false;   
        }


        [HttpDelete]
        public async Task<ActionResult> Delete(string reserva)
        {
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await conexion.EliminarReserva(reserva);
                    await conexion.EliminarAsientosReservados(reserva);

                    scope.Complete();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar la reserva: {ex.Message}");
            }
        }
    }
}
