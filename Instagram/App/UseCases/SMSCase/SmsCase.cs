using Instagram.Domain.Repositories.Interfaces.SMS;
using Twilio.TwiML.Messaging;

namespace Instagram.App.UseCases.SMSCase
{
    public class SmsCase : ISmsCase
    {
        private readonly ISmsService _smsService;
        public SmsCase(ISmsService smsService) 
        { 
            _smsService = smsService;
        }
        public async Task SendVerifySMSAsync(string phoneNumber, string message)
        {
           await _smsService.SendVerifySMSAsync(phoneNumber, message);
        }

        public async Task<bool> VerifySMSAsync(string phoneNumber, string code)
        {
            return await _smsService.VerifyCodeAsync(phoneNumber, code);
        }
    }
}
