using Instagram.App.UseCases.MediaCases.PostCase;
using Instagram.App.UseCases.MediaCases.ReelCase;
using Instagram.App.UseCases.MediaCases.StoryCase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Instagram.Presentation.Controllers.Media.Shared
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedController : ControllerBase
    {
        private readonly IPostCase _postCase;
        private readonly IStoryCase _storyCase;
        private readonly IReelCase _reelCase;
        public FeedController(IPostCase postCase, IStoryCase storyCase, IReelCase reelCase)
        {
            _postCase = postCase;
            _storyCase = storyCase;
            _reelCase = reelCase;
        }

        [Authorize]
        [HttpGet("GetFeedPostByUserId")]
        public async Task<IActionResult> GetFeedPostByUserId(Guid userId)
        {
            try
            {
                var result = await _postCase.GetFeedPostByUserId(userId);

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

        [Authorize]
        [HttpGet("GetFeedReelByUserId")]
        public async Task<IActionResult> GetFeedReelByUserId(Guid userId)
        {
            try
            {
                var result = await _reelCase.GetFeedReelByUserId(userId);

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

        [Authorize]
        [HttpGet("GetStoriesByUserId/{userId}")]
        public async Task<IActionResult> GetStoriesByUserId(Guid userId)
        {
            try
            {
                var result = await _storyCase.GetStoriesByUserId(userId);

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
    }
}
