using Instagram.App.UseCases.MediaCases.Types.Posts;
using Instagram.App.UseCases.MediaCases.Types.Reels;
using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.Domain.Entities.Media;
using Instagram.Domain.Entities.Shared.Media;
using Instagram.Infraestructure.Data.Models.Mongo.Post;
using Instagram.Infraestructure.Data.Models.Mongo.Shared.Media;
using Instagram.Infraestructure.Mappers.Shared.Media;
using Instagram.Infraestructure.Mappers.Song;

namespace Instagram.Infraestructure.Mappers
{
    public static class PostMapper
    {
        public static PostEntity MapPostTypeInToPostEntity(PostTypeIn postIn, List<string> mediaUrls) => new()
        {
            Caption = postIn.Caption,
            Hashtags = postIn.Tags,
            Images = mediaUrls,
            Song = (postIn.Music is not null) ?
            SongMapper
            .MapSongPostTypeToSongPostEntity(postIn.Music) : null,
            LocationPost = (postIn.Location is not null) ? 
            MediaMapper
            .MapLocationTypeToLocationEntity(postIn.Location) : null,
            UserId = postIn.UserId,
            Mentions = (postIn.Mentions is not null) ?
            postIn.Mentions.Select(m => MediaMapper.MapMentionTypeToMentionEntity(m))
            .ToList() : new List<MentionEntity>(),

        };
        public static PostEntity MapPostDocumentToPostEntity(PostDocument postDocument) => new()
        {
            Caption = postDocument.Description,
            DatePublication = postDocument.PublishDate,
            Hashtags = postDocument.Hashtags,
            Images = postDocument.Images,
            Likes = postDocument.Likes,
            PostId = postDocument.Id
            .ToString(),
            UserId = Guid.Parse(postDocument.UserId),
            Mentions = (postDocument.Mentions is not null) ? 
                postDocument.Mentions.Select(men => MediaMapper
                .MapMentionDocumentToMentionEntity(men))
            .ToList() : new List<MentionEntity>(),
            Song = (postDocument.Song is not null) ?
             MediaMapper
            .MapSongDocumentToSongPostEntity(postDocument.Song) : null,
            LocationPost = (postDocument.Location is not null) ?
             MediaMapper
            .MapLocationDocumentToLocationEntity(postDocument.Location) : null,
            Comments = (postDocument.Comments is not null) ? 
            postDocument.Comments
            .Select(c => MediaMapper
            .MapCommentDocumentToCommentEntity(c)).ToList() : new List<CommentEntity>()
        };

        public static PostDocument MapPostDomainToPostDb(PostEntity entity) => new()
        {
            Comments = ((entity.Comments?.Count > 0) ?
            entity.Comments.Select(c => MediaMapper
            .MapCommentEntityToCommentDocument(c))
            .ToList() :
            new List<CommentDocument>()),
                    PublishDate = entity.DatePublication,
                    Description = entity.Caption,
                    Hashtags = entity.Hashtags,
                    Images = entity.Images,
                    Likes = entity.Likes,
                    UserId = entity.UserId.ToString(),
                    Location = (entity.LocationPost is not null) ? MediaMapper
            .MapLocationEntityToMapLocationDocument(entity.LocationPost) : new LocationDocument(),
                    Song = (entity.Song is not null) ? SongMapper
            .MapSongMediaEntityToSongDocument(entity.Song) : null,
                    Mentions = (entity.Mentions is not null) ? entity.Mentions.
            Select(m => MediaMapper
                    .MapMentionEntityToMentionDocument(m))
            .ToList() : new List<MentionDocument>()
        };
        public static PostTypeOut MapPostEntityToPostTypeOut(PostEntity postEntity) => new()
        {
            Caption = postEntity.Caption,
            PostId = postEntity.PostId,
            Likes = postEntity.Likes,
            Hashtags = postEntity.Hashtags,
            DatePublication = postEntity.DatePublication,
            Mentions = (postEntity.Mentions is not null) ?
            postEntity.Mentions
            .Select(m => MediaMapper.
            MapMentionEntityToMentionType(m))
            .ToList() :
            new List<MentionType>(),
                    Images = postEntity.Images,
                    Song = (postEntity.Song is not null) ?
            MediaMapper.MapSongPostEntityToSongPostType(postEntity.Song) : null,
                    LocationPost = (postEntity.LocationPost is not null) ?
            MediaMapper.MapLocationEntityToLocationType(postEntity.LocationPost) : null,
                    Comments = (postEntity.Comments is not null) ?
            postEntity.Comments.Select(c => MediaMapper
            .MapCommentEntityToCommentMediaType(c))
            .ToList() :
            new List<CommentTypeOut>()
        };
    }
}
