using Instagram.App.DTOs.Media;
using Instagram.App.UseCases.MediaCases.ReelCase;
using Instagram.App.UseCases.MediaCases.Types.Reels;
using Instagram.App.UseCases.NotificationCase;
using Instagram.config.helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Instagram.Presentation.Controllers.Media.Reel
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReelController : ControllerBase
    {
        private readonly INotificationCase _notificationCase;
        private readonly IReelCase _reelCase;

        public ReelController(INotificationCase notificationCase, IReelCase reelCase)
        {
            _notificationCase = notificationCase;
            _reelCase = reelCase;
        }

        [Authorize]
        [HttpPost("CreateReel")]
        public async Task<IActionResult> CreateReel([FromBody] ReelTypeIn reelType)
        {
            try
            {
                // Verifica si se proporciona el archivo 'File' para el reel, si no, devuelve un error de solicitud incorrecta.
                if (reelType.File is null)
                {
                    return BadRequest("Missing file for reel");
                }

                // Verifica si la leyenda excede la longitud máxima permitida.
                if (reelType.Caption?.Length > 2200)
                {
                    return BadRequest("The caption must be a maximum of 2200 characters");
                }

                // Verifica el número de etiquetas.
                if (reelType.Tags?.Count > 30)
                {
                    return BadRequest("The maximum number of tags is 30");
                }

                // Verifica la validez de etiquetas y menciones.
                bool areTags = reelType.Tags is null || TagHelper.AreAllHashtagsValid(reelType.Tags);
                bool areMentions = reelType.Mentions is null || !reelType.Mentions
                    .Any(m => !MentionHelper.IsMentionValid(m.UserId));

                if (!areTags || !areMentions)
                {
                    return BadRequest("Tags or mentions are not valid");
                }

                // Crea el reel.
                var result = await _reelCase.CreateReelAsync(reelType);

                // Envía notificaciones push a los seguidores sobre el nuevo reel.
                await _notificationCase.SendPushNotificationToFollowers($"{reelType.Username} has shared a reel", reelType.UserId);

      
                return Ok(result); // 200 OK
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("DeleteReel")]
        public async Task<IActionResult> DeleteReel([FromBody] DeleteReelDto request)
        {
            try
            {
                // Validar si mediaId es nulo o vacío
                if (string.IsNullOrEmpty(request.MediaId))
                {
                    return BadRequest("The media id must be provided");
                }

                var result = await _reelCase.DeleteReelAsync(request.MediaId, request.UserId);

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
