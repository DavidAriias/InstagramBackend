using Instagram.App.UseCases.MediaCases.Types.Posts;
using Instagram.App.UseCases.MediaCases.Types.Reels;
using Instagram.App.UseCases.MediaCases.Types.Stories;

namespace Instagram.App.UseCases.UserCase.Types
{
    public class UserTypeOut
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public bool Isverificated { get; set; }
        public bool IsPrivate { get; set; }
        public string? ImageProfile { get; set; }
        public string? Description { get; set; }
        public string? Pronoun { get; set; }
        public LinkType? Link { get; set; }
        public DateOnly Birthday { get; set; }

        [UsePaging]
        public IEnumerable<PostTypeOut>? Posts { get; set; }

        [UsePaging]
        public IEnumerable<ReelTypeOut>? Reels { get; set; }
        [UsePaging]
        public IEnumerable<StoryTypeOut>? Stories { get; set; }

    }
}
