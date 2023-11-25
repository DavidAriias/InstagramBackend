using Instagram.App.UseCases.MediaCases.CaptionCase;
using Instagram.App.UseCases.MediaCases.CommentCase;
using Instagram.App.UseCases.MediaCases.LikeCase;
using Instagram.App.UseCases.MediaCases.LocationCase;
using Instagram.App.UseCases.MediaCases.ReplyCase;
using Instagram.App.UseCases.MediaCases.TagsCase;
using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.NotificationCase;
using Instagram.config.helpers;
using Instagram.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Instagram.Presentation.Controllers.Media.Shared
{
    public class UpdateMediaController : ControllerBase
    {
        private readonly ITagsCase _tagsCase;
        private readonly ICaptionCase _captionCase;
        private readonly ILocationCase _locationCase;
        private readonly ICommentCase _commentCase;
        private readonly INotificationCase _notificationCase;
        private readonly ILikeCase _likeCase;
        private readonly IReplyCase _replyCase;
        public UpdateMediaController(
            ITagsCase tagsCase,
            ICaptionCase captionCase,
            ILocationCase locationCase,
            ICommentCase commentCase,
            INotificationCase notificationCase,
            ILikeCase likeCase,
            IReplyCase replyCase
            )
        {
            _tagsCase = tagsCase;
            _captionCase = captionCase;
            _locationCase = locationCase;
            _commentCase = commentCase;
            _notificationCase = notificationCase;
            _likeCase = likeCase;
            _replyCase = replyCase;

        }

        [Authorize]
        [HttpPut("UpdateTagsToMedia")]
        public async Task<IActionResult> UpdateTagsToMedia([FromBody] TagsTypeIn request)
        {
            try
            {
                // Validar si mediaId es nulo o vacío
                if (string.IsNullOrEmpty(request.MediaId))
                {
                    return BadRequest("The media id must be provided");
                }

                // Validar si no se proporcionan etiquetas (tags)
                if (request.Tags == null || request.Tags.Count < 1)
                {
                    return BadRequest("To update tags, at least one tag must be provided");
                }

                // Comprueba si todas las etiquetas son válidas utilizando el helper TagHelper.
                bool areTagsValid = TagHelper.AreAllHashtagsValid(request.Tags);

                // Si las etiquetas no son válidas, devuelve una respuesta de conflicto.
                if (!areTagsValid) return BadRequest("Tags are not valid, the format is wrong");

                // Llamar a la función que realiza la actualización de etiquetas (tags)
                var result = await _tagsCase.UpdateTagsAsync(request);

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

        [Authorize]
        [HttpPut("UpdateCaptionToMedia")]
        public async Task<IActionResult> UpdateCaptionToMedia([FromBody] CaptionTypeIn request)
        {
            try
            {
                if (request.ContentType == ContentEnum.Story)
                {
                    return BadRequest("Story not supported");
                }

                var result = await _captionCase.UpdateCaption(request);

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

        [Authorize]
        [HttpPut("UpdateLocationToMedia/{mediaId}/{contentEnum}")]
        public async Task<IActionResult> UpdateLocationToMedia(string mediaId, ContentEnum contentEnum, [FromBody] LocationType locationType)
        {
            try
            {
                // Validar si mediaId es nulo o vacío
                if (string.IsNullOrEmpty(mediaId))
                {
                    return BadRequest("The media id must be provided");
                }

                // Validar si contentEnum es Story
                if (contentEnum == ContentEnum.Story)
                {
                    return BadRequest("Story not supported");
                }

                var result = await _locationCase.UpdateLocation(locationType, mediaId, contentEnum);

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


        [Authorize]
        [HttpPost("AddLikeToMedia")]
        public async Task<IActionResult> AddLikeToMedia([FromBody] LikeTypeIn like)
        {
            try
            {
                var result = await _likeCase.AddLikeToMedia(like);

                await _notificationCase.SendPushNotificationToUser($"{result.SenderUsername} has given a like to your post", result.ReceiverId);

                return Ok(result); // 200 OK
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("RemoveLikeFromMedia")]
        public async Task<IActionResult> RemoveLikeFromMedia([FromBody] LikeTypeIn like)
        {
            try
            {
                var result = await _likeCase.RemoveLikeToMedia(like);

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

        [Authorize]
        [HttpDelete("RemoveCommentFromMedia")]
        public async Task<IActionResult> RemoveCommentFromMedia([FromBody] CommentTypeIn comment)
        {
            try
            {
                // Validar si el campo CommentId es nulo o vacío
                if (string.IsNullOrEmpty(comment.CommentId))
                {
                    return BadRequest("Comment ID field is missing");
                }

                var result = await _commentCase.DeleteComment(comment);

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

        [Authorize]
        [HttpPost("AddCommentToMedia")]
        public async Task<IActionResult> AddCommentToMedia([FromBody] CommentTypeIn comment)
        {
            try
            {
                // Validar si el campo Comment es nulo o vacío
                if (string.IsNullOrEmpty(comment.Comment))
                {
                    return BadRequest("Comment field is missing");
                }

                var result = await _commentCase.CreateComment(comment);

                await _notificationCase.SendPushNotificationToUser($"{result.SenderUsername} comments your post", result.ReceiverId);

                return Ok(result); // 200 OK
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [Authorize]
        [HttpPost("AddReplyToMedia")]
        public async Task<IActionResult> AddReplyToMedia([FromBody] ReplyType replyType)
        {
            try
            {
                var result = await _replyCase.CreateReply(replyType);

                await _notificationCase.SendPushNotificationToUser($"{result.SenderUsername} has replied to comment", result.ReceiverId);

                return Ok(result); // 200 OK
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("RemoveReplyFromMedia")]
        public async Task<IActionResult> RemoveReplyFromMedia([FromBody] ReplyType replyType)
        {
            try
            {
                // Validar si el campo ReplyId es nulo o vacío
                if (replyType.ReplyId is null)
                {
                    return BadRequest("Reply ID field is missing");
                }

                var result = await _replyCase.DeleteReply(replyType);

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
