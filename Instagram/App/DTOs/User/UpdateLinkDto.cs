using Instagram.App.UseCases.UserCase.Types;

namespace Instagram.App.DTOs.User
{
    public class UpdateLinkDto
    {
        public LinkType Link { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}
