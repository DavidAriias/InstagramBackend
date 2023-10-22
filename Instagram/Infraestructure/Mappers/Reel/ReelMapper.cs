using Instagram.App.UseCases.MediaCases.Types.Reels;
using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.Domain.Entities.Media;
using Instagram.Domain.Entities.Shared.Media;
using Instagram.Infraestructure.Data.Models.Mongo.Reel;
using Instagram.Infraestructure.Data.Models.Mongo.Shared.Media;
using Instagram.Infraestructure.Mappers.Shared.Media;
using Instagram.Infraestructure.Mappers.Song;

namespace Instagram.Infraestructure.Mappers.Reel
{
    public static class ReelMapper
    {
        public static ReelEntity MapReelDocumentToReelEntity(ReelDocument reelDocument) => new()
        {
            Caption = reelDocument.Description,
            DatePublication = reelDocument.PublishDate,
            UserId = Guid.Parse(reelDocument.UserId),
            Likes = reelDocument.Likes,
            Duration = reelDocument.Duration,
            LocationReel = (reelDocument.Location is not null) ?
            MediaMapper
            .MapLocationDocumentToLocationEntity(reelDocument.Location) : null,
            ReelId = reelDocument.Id
            .ToString(),
            Url = reelDocument.Url,
            Tags = reelDocument.Hashtags,
            Song = (reelDocument.Song is not null) ?
            MediaMapper
            .MapSongDocumentToSongPostEntity(reelDocument.Song) : null,
            Comments = (reelDocument.Comments is not null) ?
            reelDocument.Comments
            .Select(c => MediaMapper
            .MapCommentDocumentToCommentEntity(c))
            .ToList() :
            new List<CommentEntity>(),
            Mentions = (reelDocument.Mentions is not null) ? 
            reelDocument.Mentions
            .Select(m => MediaMapper.MapMentionDocumentToMentionEntity(m))
            .ToList() : new List<MentionEntity>()
        };

        public static ReelDocument MapReelEntityToReelDocument(ReelEntity reelEntity) => new()
        {
            Likes = reelEntity.Likes,
            Comments = (reelEntity.Comments is not null) ?
            reelEntity.Comments
            .Select(c => MediaMapper
            .MapCommentEntityToCommentDocument(c))
            .ToList() 
            : new List<CommentDocument>(),
            Hashtags = reelEntity.Tags,
            Description = reelEntity.Caption,
            UserId = reelEntity.UserId
            .ToString(),
            Url = reelEntity.Url,
            Duration = reelEntity.Duration,
            Location = (reelEntity.LocationReel is not null) ?
            MediaMapper
            .MapLocationEntityToMapLocationDocument(reelEntity.LocationReel) :
            null,
            PublishDate = reelEntity.DatePublication,
            Mentions = (reelEntity.Mentions is not null) ?
            reelEntity.Mentions
            .Select(m => MediaMapper
            .MapMentionEntityToMentionDocument(m))
            .ToList() 
            : new List<MentionDocument>(),
            Song = (reelEntity.Song is not null) ? SongMapper
            .MapSongMediaEntityToSongDocument(reelEntity.Song) : null
        };

        public static ReelTypeOut MapReelEntityToReelTypeOut(ReelEntity reelEntity) => new()
        {
             Caption = reelEntity.Caption,
             Comments = (reelEntity.Comments is not null) ? reelEntity.Comments
            .Select(c => MediaMapper
            .MapCommentEntityToCommentMediaType(c))
            .ToList() : new List<CommentTypeOut>(),
             DatePublication = reelEntity.DatePublication,
             Duration = reelEntity.Duration,
             Likes = reelEntity.Likes,
             LocationReel = (reelEntity.LocationReel is not null) ?
             MediaMapper
            .MapLocationEntityToLocationType(reelEntity.LocationReel)  : null,
             ReelId = reelEntity.ReelId,
             Url = reelEntity.Url,
             Mentions = (reelEntity.Mentions is not null) ? reelEntity.Mentions
            .Select(m => MediaMapper
            .MapMentionEntityToMentionType(m)).ToList() : new List<MentionType>(),
             Tags = reelEntity.Tags,
             Song = (reelEntity.Song is not null) ? 
            MediaMapper
            .MapSongPostEntityToSongPostType(reelEntity.Song) : null
        };

        public static ReelEntity MapReelTypeInToReelEntity(ReelTypeIn reelTypeIn) => new()
        {
            Caption = reelTypeIn.Caption,
            UserId = reelTypeIn.UserId,
            LocationReel = (reelTypeIn.Location is not null) ?
            MediaMapper
            .MapLocationTypeToLocationEntity(reelTypeIn.Location) : null,
            Mentions = (reelTypeIn.Mentions is not null) ?
            reelTypeIn.Mentions.Select(m => MediaMapper.MapMentionTypeToMentionEntity(m))
            .ToList() : new List<MentionEntity>(),   
            Duration = reelTypeIn.Duration,
            Tags = reelTypeIn.Tags,
            Song = (reelTypeIn.Music is not null) ? 
            SongMapper
            .MapSongPostTypeToSongPostEntity(reelTypeIn.Music) : null
        };

    }
}
