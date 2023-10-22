using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.MusicCase;
using Instagram.Domain.Entities;
using Instagram.Domain.Entities.Shared.Media;
using Instagram.Infraestructure.Data.Models.Mongo.Shared.Media;

namespace Instagram.Infraestructure.Mappers.Song
{
    public static class SongMapper
    {
        public static IEnumerable<SongSearchType> MapSongSearchEntityToSongSearchType(IEnumerable<SongServiceEntity> response)
        {
            return response.Select(song => new SongSearchType
            {
                AlbumName = song.AlbumName,
                ArtistName = song.ArtistName,
                Duration = song.Duration,
                ImagesAlbum = song.ImagesAlbum,
                PreviewUrl = song.PreviewUrl,
                SongName = song.SongName,
                Uri = song.Uri
            });
        }

        public static SongMediaEntity MapSongPostTypeToSongPostEntity(SongMediaType songPostType) => new()
        {
            Artist = songPostType.Artist,
            Selection = MapDurationSelecionTypeToDurationSelectionEntity(songPostType.Selection),
            TotalDuration = songPostType.TotalDuration,
            Title = songPostType.Title,
            Url = songPostType.Url
        };

        public static DurationSelectionEntity MapDurationSelecionTypeToDurationSelectionEntity(DurationSelectionType durationSelectionType)
            => new()
            {
                Begin = durationSelectionType.Begin,
                End = durationSelectionType.End,
            };
        public static SongDocument MapSongMediaEntityToSongDocument(SongMediaEntity entity) => new()
        {
            Artist = entity.Artist,
            Title = entity.Title,
            TotalDuration = entity.TotalDuration,
            Url = entity.Url,
            Selection = MapDurationDomainToDurationDb(entity.Selection)
        };
        private static DurationSelectionDocument MapDurationDomainToDurationDb(DurationSelectionEntity selectionEntity) => new()
        {
            Start = selectionEntity.Begin,
            End = selectionEntity.End,
        };

    }
}
