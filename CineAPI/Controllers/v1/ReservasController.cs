using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v1/reservas")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ADMIN")]
    public class ReservasController : ControllerBase
    {
        [HttpGet]
        public async Task<string> Get()
        {
            return "Ok";
        }
    }
}
