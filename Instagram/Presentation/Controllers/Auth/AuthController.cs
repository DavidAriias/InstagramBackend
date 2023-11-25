using Instagram.App.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Instagram.Presentation.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [Authorize]
        [HttpPost("CheckTokenStatus")]
        public async Task<IActionResult> CheckTokenStatus([FromBody] AuthTypeIn authType)
        {
            try
            {
                if (authType.Token is null)
                {
                    return BadRequest("You must send access token"); // 400 Bad Request
                }

                var result = await _authService.CheckStatus(authType);

                if (result != null)
                {
                    return Ok(result); // 200 OK
                }
                else
                {
                    return NotFound(); // 404 Not Found
                }
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [HttpPost("CloseSession")]
        public async Task<IActionResult> CloseSession([FromBody] AuthTypeIn auth)
        {
            try
            {
                var result = await _authService.CloseSession(auth);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return Ok(result); // 200 OK
                }
                else
                {
                    return StatusCode((int)result.StatusCode, result); // Otro código de estado según el resultado
                }
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }
    }
}
