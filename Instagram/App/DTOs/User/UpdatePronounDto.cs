using Instagram.Domain.Enums;

namespace Instagram.App.DTOs.User
{
    public class UpdatePronounDto
    {
        public PronounEnum Pronoun { get; set; }
        public Guid UserId { get; set; }
    }
}
