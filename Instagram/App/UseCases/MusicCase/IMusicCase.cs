namespace Instagram.App.UseCases.MusicCase
{
    public interface IMusicCase
    {
        public Task<IEnumerable<SongSearchType>?> SearchSongByName(string songName);
    }
}
