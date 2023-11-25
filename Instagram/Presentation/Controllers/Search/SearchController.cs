using Instagram.App.UseCases.MusicCase;
using Instagram.App.UseCases.SearchCase;
using Instagram.App.UseCases.Types.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Instagram.Presentation.Controllers.Search
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISeachCase _searchCase;
        private readonly IMusicCase _musicCase;
        public SearchController(ISeachCase searchCase, IMusicCase musicCase)
        {
            _searchCase = searchCase;
            _musicCase = musicCase;
        }

        [Authorize]
        [HttpGet("SearchUsers")]
        [ProducesResponseType(typeof(IReadOnlyCollection<SearchType>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchUsers([FromQuery] string input)
        {
            try
            {
                var result = await _searchCase.SearchUsers(input);

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
        [HttpGet("SearchSongByName")]
        [ProducesResponseType(typeof(IEnumerable<SongSearchType>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchSongByName([FromQuery] string trackName)
        {
            try
            {
                var result = await _musicCase.SearchSongByName(trackName);

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
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
