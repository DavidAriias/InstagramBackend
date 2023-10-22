using Instagram.config.constants;
using Instagram.Domain.Repositories.Interfaces.SMS;
using Twilio;
using Twilio.Rest.Verify.V2.Service;

namespace Instagram.Infraestructure.Services.SMS
{
    public class SmsService : ISmsService
    {
        public async Task SendVerifySMSAsync(string phoneNumber, string message)
        {
            TwilioClient.Init(EnvironmentConfig.AccountSid, EnvironmentConfig.AccountToken);

            await VerificationResource.CreateAsync(
            to: phoneNumber,
            channel: "sms",  // Método de entrega (en este caso, SMS)
            pathServiceSid: EnvironmentConfig.PathSid // Puedes usar "default" o el SID de un servicio personalizado de Verify
            );

        }

        public async Task<bool> VerifyCodeAsync(string phoneNumber, string code)
        {
            TwilioClient.Init(EnvironmentConfig.AccountSid, EnvironmentConfig.AccountToken);

            var verificationCheck = await VerificationCheckResource.CreateAsync(
                 to: phoneNumber,
                 code: code,
                 pathServiceSid: EnvironmentConfig.PathSid
             );

            return verificationCheck.Status == "approved";
        }
    }
}
