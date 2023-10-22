using Instagram.App.UseCases.MediaCases.Types.Posts;
using Instagram.App.UseCases.MediaCases.Types.Reels;
using Instagram.App.UseCases.MediaCases.Types.Stories;
using Instagram.App.UseCases.Types.Feed;
using Instagram.Domain.Entities.Media;
using Instagram.Domain.Entities.User;
using Instagram.Infraestructure.Mappers.Reel;
using Instagram.Infraestructure.Mappers.Story;
using Microsoft.Extensions.Hosting;
using Twilio.TwiML.Messaging;

namespace Instagram.Infraestructure.Mappers.Feed
{
    public static class FeedMapper
    {
        public static FeedType<StoryTypeOut> MapFeedStory(UserEntity user, IEnumerable<StoryEntity> stories) => new()
        {          
            Username = user.Username,
            ImageProfile = user?.Imageprofile,
            Media = stories.Select(story => StoryMapper.MapStoryEntityToStoryTypeOut(story))
                .ToList()
        };

        public static FeedType<ReelTypeOut> MapFeedReel(UserEntity user, IEnumerable<ReelEntity> reels) => new()
        {
            Username = user.Username,
            ImageProfile = user?.Imageprofile,
            Media = reels.Select(reel => ReelMapper.MapReelEntityToReelTypeOut(reel))
                .ToList()
        };

        public static FeedType<PostTypeOut> MapFeedPost(UserEntity user, IEnumerable<PostEntity> posts) => new()
        {
            Username = user.Username,
            ImageProfile = user?.Imageprofile,
            Media = posts.Select(post => PostMapper.MapPostEntityToPostTypeOut(post))
                .ToList()
        };

    }
}
