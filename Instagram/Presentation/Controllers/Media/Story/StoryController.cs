using HotChocolate.Subscriptions;
using Instagram.App.DTOs.Media;
using Instagram.App.UseCases.MediaCases.StoryCase;
using Instagram.App.UseCases.MediaCases.Types.Stories;
using Instagram.App.UseCases.NotificationCase;
using Instagram.Presentation.GraphQL.Suscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Instagram.Presentation.Controllers.Media.Story
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoryController : ControllerBase
    {

        private readonly INotificationCase _notificationCase;
        private readonly IStoryCase _storyCase;

        public StoryController(INotificationCase notificationCase, IStoryCase storyCase)
        {

            _notificationCase = notificationCase;
            _storyCase = storyCase;
        }

        [Authorize]
        [HttpPost("CreateStory")]
        public async Task<IActionResult> CreateStory([FromBody] StoryTypeIn storyType)
        {
            try
            {
                var result = await _storyCase.CreateStory(storyType);

                await _notificationCase.SendPushNotificationToFollowers($"{storyType.Username} has shared a story", storyType.UserId);

                return Ok(result); // 200 OK
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("DeleteStory")]
        public async Task<IActionResult> DeleteStory([FromBody] DeleteStoryDto request)
        {
            try
            {
                // Validar si mediaId es nulo o vacío
                if (string.IsNullOrEmpty(request.MediaId))
                {
                    return BadRequest("The media id must be provided");
                }

                var result = await _storyCase.DeleteStory(request.MediaId, request.UserId);

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
