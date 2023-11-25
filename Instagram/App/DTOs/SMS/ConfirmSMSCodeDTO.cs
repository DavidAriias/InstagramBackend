namespace Instagram.App.DTOs.SMS
{
    public class ConfirmSMSCodeDTO
    {
        public string PhoneNumber { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}
