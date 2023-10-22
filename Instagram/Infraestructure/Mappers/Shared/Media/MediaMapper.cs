using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.Domain.Entities.Shared.Media;
using Instagram.Infraestructure.Data.Models.Mongo.Shared.Media;

namespace Instagram.Infraestructure.Mappers.Shared.Media
{
    public static partial class MediaMapper
    {
        public static LocationEntity MapLocationTypeToLocationEntity(LocationType location) => new()
        {
            Latitude = location.Latitude,
            Longitude = location.Longitude,
        };

        public static ReplyEntity MapReplyTypeToReplyEntity(ReplyType replyType) => new()
        {
            ReplyId = replyType.ReplyId!,
            Text = replyType.Text!,
            UserId = replyType.UserId.ToString(),

        };

        public static MentionEntity MapMentionTypeToMentionEntity(MentionType mentionType) => new()
        {
            UserId = mentionType.UserId
        };

        public static LocationEntity MapLocationDocumentToLocationEntity(LocationDocument location) => new()
        {
            Latitude = location.Latitude,
            Longitude = location.Longitude,
        };

        public static MentionEntity MapMentionDocumentToMentionEntity(MentionDocument mentionDocument) => new()
        {
            UserId = mentionDocument.UserId
        };

        public static DurationSelectionEntity MapDurationSelectionDocumentToDurationSelectionEntity(DurationSelectionDocument durationSelectionDocument) =>
        new()
        {
            Begin = durationSelectionDocument.Start,
            End = durationSelectionDocument.End
        };

        public static ReplyEntity MapReplyDocumentToReplyEntity(ReplyDocument replyDocument) => new()
        {
            Date = replyDocument.Date,
            ReplyId = replyDocument.ReplyId.ToString(),
            Text = replyDocument.Text,
            UserId = replyDocument.UserId,
        };
        public static CommentEntity MapCommentDocumentToCommentEntity(CommentDocument commentDocument) => new()
        {
            Date = commentDocument.Date,
            Replies = commentDocument.Replies?.Select(r => MapReplyDocumentToReplyEntity(r)).ToList(),
            Text = commentDocument.Text,
            UserId = Guid.Parse(commentDocument.UserId)
        };

        public static SongMediaEntity MapSongDocumentToSongPostEntity(SongDocument songDocument) => new()
        {
            Artist = songDocument.Artist,
            Title = songDocument.Title,
            TotalDuration = songDocument.TotalDuration,
            Url = songDocument.Url,
            Selection = MapDurationSelectionDocumentToDurationSelectionEntity(songDocument.Selection)
        };
    }
}
