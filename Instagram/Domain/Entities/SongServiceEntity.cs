using SpotifyAPI.Web;

namespace Instagram.Domain.Entities
{
    public class SongServiceEntity
    {
        public string SongName { get; set; } = null!;
        public string Uri { get; set; } = null!;
        public string ArtistName { get; set; } = null!;
        public string PreviewUrl { get; set; } = null!;
        public decimal Duration { get; set; }
        public List<Image> ImagesAlbum { get; set; } = null!;
        public string AlbumName { get; set; } = null!;
    }
}
