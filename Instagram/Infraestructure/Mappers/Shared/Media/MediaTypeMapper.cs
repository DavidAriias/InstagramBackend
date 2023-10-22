using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.MusicCase;
using Instagram.Domain.Entities.Shared.Media;

namespace Instagram.Infraestructure.Mappers.Shared.Media
{
    public static partial class MediaMapper
    {
        public static MentionType MapMentionEntityToMentionType(MentionEntity mentionEntity) => new()
        {
            UserId = mentionEntity.UserId
        };

        public static SongMediaType MapSongPostEntityToSongPostType(SongMediaEntity songPostEntity) => new()
        {
            Artist = songPostEntity.Artist,
            Selection = MapDurationSelectionEntityToDurationSelectionType(songPostEntity.Selection),
            Title = songPostEntity.Title,
            TotalDuration = songPostEntity.TotalDuration,
            Url = songPostEntity.Url
        };

        public static DurationSelectionType MapDurationSelectionEntityToDurationSelectionType(DurationSelectionEntity durationSelectionEntity) => new()
        {
            Begin = durationSelectionEntity.Begin,
            End = durationSelectionEntity.End,
        };

        public static LocationType MapLocationEntityToLocationType(LocationEntity locationEntity) => new()
        {
            Latitude = locationEntity.Latitude,
            Longitude = locationEntity.Longitude
        };

        public static ReplyTypeOut MapReplyEntityToReplyType(ReplyEntity replyEntity) => new()
        {
            UserId = Guid.Parse(replyEntity.UserId),
            Text = replyEntity.Text,
            Date = replyEntity.Date,
            ReplyId = replyEntity.ReplyId,
        };
        public static CommentTypeOut MapCommentEntityToCommentMediaType(CommentEntity commentEntity) => new()
        {
            Date = commentEntity.Date,
            Text = commentEntity.Text,
            UserId = commentEntity.UserId,
            Replies = (commentEntity.Replies is not null) ?
            commentEntity.Replies.Select(r => MapReplyEntityToReplyType(r)).ToList() :
            new List<ReplyTypeOut>()

        };
    }
}
