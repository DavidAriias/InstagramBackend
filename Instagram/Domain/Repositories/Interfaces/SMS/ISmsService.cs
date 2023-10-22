namespace Instagram.Domain.Repositories.Interfaces.SMS
{
    public interface ISmsService
    {
        public Task SendVerifySMSAsync(string phoneNumber, string message);
        public Task<bool> VerifyCodeAsync(string phoneNumber, string code);
    }
}
