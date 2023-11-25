using HotChocolate.Subscriptions;
using Instagram.App.UseCases.MediaCases.PostCase;
using Instagram.App.UseCases.MediaCases.Types.Posts;
using Instagram.App.UseCases.NotificationCase;
using Instagram.config.helpers;
using Instagram.Presentation.GraphQL.Suscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Instagram.Presentation.Controllers.Media.Post
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ITopicEventSender _eventSender;
        private readonly INotificationCase _notificationCase;
        private readonly IPostCase _postCase;

        public PostController(
            ITopicEventSender eventSender,
            INotificationCase notificationCase,
            IPostCase postCase)
        {
            _eventSender = eventSender;
            _notificationCase = notificationCase;
            _postCase = postCase;
        }

        [Authorize]
        [HttpPost("CreatePost")]
        public async Task<IActionResult> CreatePost([FromBody] PostTypeIn request)
        {
            try
            {
                // Comprueba la longitud de la leyenda.
                if (request.Caption?.Length > 2200)
                {
                    return Conflict("The caption must be a maximum of 2200 characters");
                }

                // Comprueba la cantidad de etiquetas.
                if (request.Tags?.Count > 30)
                {
                    return Conflict("The maximum number of tags is 30");
                }

                // Comprueba la cantidad de medios.
                if (request.Media?.Count > 10 || request.Media?.Count < 1)
                {
                    return Conflict("The maximum number of media is 10 and the minimum is 1");
                }

                // Verifica la validez de etiquetas y menciones.
                bool areTags = request.Tags is null || TagHelper.AreAllHashtagsValid(request.Tags);
                bool areMentions = request.Mentions is null || !request.Mentions
                    .Any(m => !MentionHelper.IsMentionValid(m.UserId));

                if (!areTags || !areMentions)
                {
                    return BadRequest("Tags or mentions are not valid");
                }

                // Crea la publicación.
                var result = await _postCase.CreatePostAsync(request);

                // Envía notificaciones push a los seguidores sobre la nueva publicación.
                await _notificationCase.SendPushNotificationToFollowers($"{request.Username} has shared a post", request.UserId);

                // Envía un evento de suscripción para la nueva publicación.
                await _eventSender.SendAsync(nameof(Subscription.OnNotificationCreatePost), result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("DeletePost/{userId}/{mediaId}")]
        public async Task<IActionResult> DeletePost(Guid userId, string mediaId)
        {
            try
            {
                // Validar si mediaId es nulo o vacío
                if (string.IsNullOrEmpty(mediaId))
                {
                    return BadRequest("The media id must be provided");
                }

                // Llamar a la función que realiza la eliminación del post
                var result = await _postCase.DeletePostAsync(mediaId, userId);

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
