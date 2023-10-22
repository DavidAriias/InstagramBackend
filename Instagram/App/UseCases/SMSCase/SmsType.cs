namespace Instagram.App.UseCases.SMSCase
{
    public class SmsType
    {
        public SmsType(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public bool Success { get; }
        public string Message { get; }
    }
}
