using Instagram.Domain.Enums;

namespace Instagram.App.UseCases.UserCase.Types
{
    public class UserTypeIn
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Name { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateOnly Birthday { get; set; }
        public PronounEnum? Pronoun { get; set; }
        public IFile? ImageProfile { get; set; }
    }
}
