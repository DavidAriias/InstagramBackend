using Instagram.App.UseCases.MediaCases.Types.Reels;
using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.MediaCases.Types.Stories;
using Instagram.Domain.Entities.Media;
using Instagram.Domain.Entities.Shared.Media;
using Instagram.Infraestructure.Data.Models.Mongo.Reel;
using Instagram.Infraestructure.Data.Models.Mongo.Shared.Media;
using Instagram.Infraestructure.Data.Models.Mongo.Story;
using Instagram.Infraestructure.Mappers.Shared.Media;
using Instagram.Infraestructure.Mappers.Song;

namespace Instagram.Infraestructure.Mappers.Story
{
    public static class StoryMapper
    {
        public static StoryEntity MapStoryDocumentToStoryEntity(StoryDocument storyDocument) => new()
        {
            LocationStory = (storyDocument.Location is not null) ?
            MediaMapper
            .MapLocationDocumentToLocationEntity(storyDocument.Location) : null,
            MediaUrl = storyDocument.Url,
            StoryId = storyDocument.Id.ToString(),
            UserId = Guid.Parse(storyDocument.UserId),
            PostDate = storyDocument.PublishDate
        };

        public static StoryDocument MapStoryEntityToStoryDocument(StoryEntity storyEntity) => new()
        {
            UserId = storyEntity.UserId
            .ToString(),
            Url = storyEntity.MediaUrl,
            Location = (storyEntity.LocationStory is not null) ?
            MediaMapper
            .MapLocationEntityToMapLocationDocument(storyEntity.LocationStory) :
            null,
            PublishDate = storyEntity.PostDate,
            Song = (storyEntity.SongMedia is not null) ? SongMapper
            .MapSongMediaEntityToSongDocument(storyEntity.SongMedia) : null
        };

        public static StoryTypeOut MapStoryEntityToStoryTypeOut(StoryEntity storyEntity) => new()
        {
            PostDate = storyEntity.PostDate,
            LocationStory = (storyEntity.LocationStory is not null) ?
             MediaMapper
            .MapLocationEntityToLocationType(storyEntity.LocationStory) : null,
            StoryId = storyEntity.StoryId.ToString(),
            MediaUrl = storyEntity.MediaUrl,
            SongMedia = (storyEntity.SongMedia is not null) ?
            MediaMapper
            .MapSongPostEntityToSongPostType(storyEntity.SongMedia) : null
        };

        public static StoryEntity MapReelTypeInToReelEntity(StoryTypeIn storyTypeIn) => new()
        {
            UserId = storyTypeIn.UserId,
            LocationStory = (storyTypeIn.Location is not null) ?
            MediaMapper
            .MapLocationTypeToLocationEntity(storyTypeIn.Location) : null,
            SongMedia = (storyTypeIn.Music is not null) ?
            SongMapper
            .MapSongPostTypeToSongPostEntity(storyTypeIn.Music) : null
        };
    }
}
