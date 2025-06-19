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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ADMIN")]
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
        public async Task<string> Get()
        {
            return "ok"; 
        }

        [HttpPost]
        public async Task<ActionResult> Post(ReservaDTO reservaDTO)
        {
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
    }
}
