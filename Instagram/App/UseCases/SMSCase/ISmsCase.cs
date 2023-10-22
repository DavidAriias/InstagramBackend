namespace Instagram.App.UseCases.SMSCase
{
    public interface ISmsCase
    {
        public Task SendVerifySMSAsync(string phoneNumber, string message);
        public Task<bool> VerifySMSAsync(string phoneNumber, string code);
    }
}
