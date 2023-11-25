using Instagram.App.Auth;

namespace Instagram.App.DTOs.User
{
    public class SignInRequestDTO
    {
        public string StringForMethod { get; set; } = null!;
        public string Password { get; set; } = null!;
        public AuthMethod AuthMethod { get; set; }
    }
}
