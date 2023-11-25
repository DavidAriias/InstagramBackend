using Instagram.App.Auth;
using Instagram.App.DTOs.User;
using Instagram.App.UseCases.NotificationCase;
using Instagram.App.UseCases.Types.Notification;
using Instagram.App.UseCases.Types.Shared;
using Instagram.App.UseCases.UserCase.EditProfile;
using Instagram.App.UseCases.UserCase.Followed;
using Instagram.App.UseCases.UserCase.Followers;
using Instagram.App.UseCases.UserCase.GetProfile;
using Instagram.App.UseCases.UserCase.SignIn;
using Instagram.App.UseCases.UserCase.SignUp;
using Instagram.App.UseCases.UserCase.Types;
using Instagram.App.UseCases.UserCase.Types.Events;
using Instagram.config.helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Instagram.Presentation.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IGetProfileCase _getProfileCase;
        private readonly ISignInCase _signInCase;
        private readonly ISignUpCase _signUpCase;
        private readonly IEditProfileCase _editProfileCase;
        private readonly IFollowerCase _followerCase;
        private readonly IFollowedCase _followedCase;
        private readonly INotificationCase _notificationCase;
       
        public UserController(
            IGetProfileCase getProfileCase,
            ISignInCase signInCase,
            ISignUpCase signUpCase,
            IEditProfileCase editProfileCase,
            IFollowerCase followerCase,
            IFollowedCase followedCase,
            INotificationCase notificationCase,
            IAuthService authService
            )
        {
            _getProfileCase = getProfileCase;
            _signInCase = signInCase;
            _signUpCase = signUpCase;
            _editProfileCase = editProfileCase;
            _followerCase = followerCase;
            _followedCase = followedCase;
            _notificationCase = notificationCase;
        }

        [HttpPost("SignIn")]
        public async Task<ActionResult<AuthTypeOut>> SignInUser([FromBody] SignInRequestDTO signInRequest)
        {
            try
            {
                var result = await _signInCase.SignInUser(signInRequest.StringForMethod, signInRequest.Password, signInRequest.AuthMethod);

                if (result is null)
                {
                    return Unauthorized("User, phone number, email, or password are wrong");
                }

                return Ok(result);
            }
            catch (System.Exception)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{userId}")]
        [Authorize]
        public async Task<ActionResult<UserTypeOut>> GetUserProfile(Guid userId)
        {
            try
            {
                var result = await _getProfileCase.GetProfile(userId);

                if (result is null)
                {
                    return NotFound("The user doesn't exist");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error " + ex.Message);
            }
        }

        [HttpPost("SignUpUser")]
        public async Task<ActionResult<ResponseType<string>>> SignUpUser([FromBody] UserTypeIn request)
        {
            try
            {
                // Validaciones adicionales pueden ser agregadas según tus necesidades

                if (string.IsNullOrWhiteSpace(request.PhoneNumber) && string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest("You need to send a phone number or email to begin your account");
                }

                if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && !PhoneNumberHelper.IsValidPhoneNumber(request.PhoneNumber))
                {
                    return BadRequest("The phone number is not valid");
                }

                if (!string.IsNullOrWhiteSpace(request.Email) && !EmailHelper.IsValidEmail(request.Email))
                {
                    return BadRequest("The email input is not formatted as an email");
                }

                if (!UsernameHelper.IsValidUsername(request.Username))
                {
                    return Conflict("The username must be between 1 and 30 characters and it doesn't have blank spaces");
                }

                if (!BirthdayHelper.IsValidAge(request.Birthday))
                {
                    return Conflict("You can't make an Instagram account");
                }

                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest("The email is empty");
                }

                var signUpResult = await _signUpCase.CreateUser(request);
                return Ok(signUpResult);
            }
            catch (System.Exception)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize]
        [HttpPut("UpdateBirthday")]
        public async Task<ActionResult<ResponseType<string>?>> UpdateBirthday([FromBody] UpdateBirthdayDto dto)
        {
            try
            {
                //Valida que sea mayor a 13 años
                if (!BirthdayHelper.IsValidAge(dto.Birthday))
                {
                    return Conflict("You can't change your birthday to less than 13 years");
                }

                var result = await _editProfileCase.UpdateBirthday(dto.Birthday, dto.UserId);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return Ok(result);
                }
                else
                {
                    return StatusCode((int)result.StatusCode, result);
                }
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }


        [Authorize]
        [HttpPut("UpdatePassword")]
        public async Task<ActionResult<ResponseType<string>>> UpdatePassword([FromBody] UpdatePasswordDto request)
        {
            try
            {
                /*
                 * Valida que la contraseña sea de longitud de min 8 caracteres y de max 16,
                 * que contenga mayúsculas y minúsculas
                 */
                if (!PassHelper.IsValidPass(request.Password))
                {
                    return BadRequest("The password must be at least 8 characters, use uppercase, lowercase, and symbols");
                }

                var result = await _editProfileCase.UpdatePassword(request.Password, request.UserId);

                return result.StatusCode == HttpStatusCode.OK
                    ? Ok(result)
                    : StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [Authorize]
        [HttpPut("UpdateUsername")]
        public async Task<ActionResult<ResponseType<string>>> UpdateUsername([FromBody] UpdateUsernameDto request)
        {
            try
            {
                /*
                 * Valida que el nombre de usuario sea de longitud máxima de 30 caracteres
                 * y al menos sea de 1 caracter y que no contenga espacios en blanco
                 */
                if (!UsernameHelper.IsValidUsername(request.Username))
                {
                    return Conflict("The username must be between 1 and 30 characters and it doesn't have blank spaces");
                }

                var result = await _editProfileCase.UpdateUsername(request.Username, request.UserId);

                return result.StatusCode == HttpStatusCode.OK
                    ? Ok(result)
                    : StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error"+ ex.Message);
            }
        }

        [Authorize]
        [HttpPut("UpdateLink")]
        public async Task<ActionResult<ResponseType<LinkType?>>> UpdateLink([FromBody] UpdateLinkDto request)
        {
            try
            {
                if (!LinkHelper.IsLink(request.Link.Link))
                {
                    return BadRequest("The link doesn't have the format for being a URL");
                }

                if (request.Link.Link.Length > 200)
                {
                    return BadRequest("The max length must be 200 characters for the link");
                }

                if (request.Link.Title.Length > 150)
                {
                    return BadRequest("The max length must be 150 characters for the title");
                }

                var result = await _editProfileCase.UpdateLink(request.Link, request.UserId);

                return result.StatusCode == HttpStatusCode.OK
                    ? Ok(result)
                    : StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [Authorize]
        [HttpPut("UpdateBiography")]
        public async Task<ActionResult<ResponseType<string>>> UpdateBiography([FromBody] UpdateBiographyDto request)
        {
            try
            {
                if (request.Biography.Length > 150)
                {
                    return BadRequest("The max length must be 150 characters for bio");
                }

                var result = await _editProfileCase.UpdateBiography(request.Biography, request.UserId);

                return result.StatusCode == HttpStatusCode.OK
                    ? Ok(result)
                    : StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [Authorize]
        [HttpPut("UpdateName")]
        public async Task<ActionResult<ResponseType<string>>> UpdateName([FromBody] UpdateNameDto request)
        {
            try
            {
                if (request.Name.Length > 80)
                {
                    return BadRequest("The max length must be 80 characters for name");
                }

                var result = await _editProfileCase.UpdateName(request.Name, request.UserId);

                return result.StatusCode == HttpStatusCode.OK
                    ? Ok(result)
                    : StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [Authorize]
        [HttpPut("UpdatePronoun")]
        public async Task<ActionResult<ResponseType<string>>> UpdatePronoun([FromBody] UpdatePronounDto request)
        {
            try
            {
                var result = await _editProfileCase.UpdatePronoun(request.Pronoun, request.UserId);

                return result.StatusCode == HttpStatusCode.OK
                    ? Ok(result)
                    : StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [Authorize]
        [HttpPut("UpdateImageProfile")]
        public async Task<ActionResult<ResponseType<string>>> UpdateImageProfile([FromForm] UpdateImageProfileDto request)
        {
            try
            {
                if (request.Image == null || request.Image.Length == 0)
                {
                    return BadRequest("Image file is required");
                }

                var result = await _editProfileCase.UpdateImageProfile(request.Image, request.UserId);

                return result.StatusCode == HttpStatusCode.OK
                    ? Ok(result)
                    : StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error"+ ex.Message);
            }
        }

        [Authorize]
        [HttpPost("FollowToUser")]
        public async Task<ActionResult<FollowedEventType>> FollowToUser([FromBody] FollowToUserDto request)
        {
            try
            {
                // Validación para que el usuario no pueda seguirse a sí mismo
                if (request.FollowerId == request.FollowedUserId)
                {
                    return BadRequest("The user can't follow itself");
                }

                // Validación para que no pueda seguir más de una vez al mismo usuario
                bool isFollowed = await _followerCase.IsUserFollowToOther(request.FollowerId, request.FollowedUserId);

                if (isFollowed)
                {
                    return Conflict("You already follow it");
                }

                var result = await _followedCase.FollowToUser(request.FollowerId, request.FollowedUserId);

                await _notificationCase.SendPushNotificationToUser($"{result.FollowerName} starts follow you", result.FollowedUserId);

                return result;
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [Authorize]
        [HttpPost("UnfollowUser")]
        public async Task<ActionResult<ResponseType<Guid>>> UnfollowUser([FromBody] UnfollowUserDto request)
        {
            try
            {
                // Validación para que el userId no sea igual al followerId
                if (request.UserId == request.FollowerId)
                {
                    return BadRequest("The user id can't be the same as the follower id");
                }

                bool isFollowed = await _followerCase.IsUserFollowToOther(request.FollowerId, request.UserId);

                if (!isFollowed)
                {
                    return Conflict("You can't unfollow an user that you don't follow");
                }

                return await _followerCase.UnfollowUser(request.FollowerId, request.UserId);
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [Authorize]
        [HttpPut("UpdateIsPrivateProfile")]
        public async Task<IActionResult> UpdateIsPrivateProfile([FromBody] Guid userId)
        {
            try
            {
                var result = await _editProfileCase.UpdateIsPrivateProfile(userId);

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
        [HttpPut("UpdateIsVerificateProfile")]
        public async Task<IActionResult> UpdateIsVerificateProfile([FromBody] Guid userId)
        {
            try
            {
                var result = await _editProfileCase.UpdateIsVerificateProfile(userId);

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
        [HttpPost("RegisterDeviceToken")]
        public async Task<IActionResult> RegisterDeviceToken([FromBody] DeviceTokenType deviceToken)
        {
            try
            {
                var result = await _notificationCase.RegisterDeviceToken(deviceToken);

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


