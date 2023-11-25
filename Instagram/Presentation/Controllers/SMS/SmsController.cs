using Instagram.App.DTOs.SMS;
using Instagram.App.UseCases.SMSCase;
using Instagram.config.helpers;
using Microsoft.AspNetCore.Mvc;

namespace Instagram.Presentation.Controllers.SMS
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsController : ControllerBase
    {
        private readonly ISmsCase _smsCase;

        public SmsController(ISmsCase smsCase)
        {
            _smsCase = smsCase;
        }

        [HttpPost("SendVerificationSMS")]
        public async Task<ActionResult<SmsType>> SendVerificationSMS([FromBody] string phoneNumber)
        {
            try
            {
                if (!PhoneNumberHelper.IsValidPhoneNumber(phoneNumber))
                {
                    return BadRequest("The phone number isn't valid");
                }

                await _smsCase.SendVerifySMSAsync(phoneNumber, "Your verify code for your Instagram's account");

                return Ok(new SmsType(true, "We sent a code to your phone number"));
            }
            catch (Exception ex)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error" + ex.Message);
            }
        }

        [HttpPost("ConfirmSMSCode")]
        public async Task<ActionResult<SmsType>> ConfirmSMSCode([FromBody] ConfirmSMSCodeDTO request)
        {
            try
            {
                bool isVerified = await _smsCase.VerifySMSAsync(request.PhoneNumber, request.Code);

                if (isVerified)
                {
                    return Ok(new SmsType(true, "Verification successfully"));
                }
                else
                {
                    return Ok(new SmsType(false, "The code is wrong"));
                }
            }
            catch (System.Exception)
            {
                // Manejar otros errores según sea necesario
                return StatusCode(500, "Internal Server Error");
            }
        }
    }

}
