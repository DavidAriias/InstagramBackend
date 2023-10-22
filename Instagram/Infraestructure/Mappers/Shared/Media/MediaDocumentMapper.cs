using Instagram.Domain.Entities.Shared.Media;
using Instagram.Infraestructure.Data.Models.Mongo.Shared.Media;

namespace Instagram.Infraestructure.Mappers.Shared.Media
{
    public static partial class MediaMapper
    {
        public static MentionDocument MapMentionEntityToMentionDocument(MentionEntity mentionEntity)
        {
            return new MentionDocument
            {
                UserId = mentionEntity.UserId
            };
        }

        public static LocationDocument MapLocationEntityToMapLocationDocument(LocationEntity entity) => new()
        {
            Latitude = entity.Latitude,
            Longitude = entity.Longitude,
        };

        public static CommentDocument MapCommentEntityToCommentDocument(CommentEntity commentEntity) => new()
        {

            Date = commentEntity.Date,
            Text = commentEntity.Text,
            UserId = commentEntity.UserId.ToString(),
            Replies = (commentEntity.Replies?.Count > 0) ? 
            commentEntity.Replies.Select(r => MapReplyEntityToReplyDocument(r)).ToList() : new List<ReplyDocument>()
        };

        public static ReplyDocument MapReplyEntityToReplyDocument(ReplyEntity replyEntity) => new()
        {
            Date = replyEntity.Date,
            Text = replyEntity.Text,
            UserId = replyEntity.UserId.ToString(),

        };
    }
}
