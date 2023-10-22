namespace Instagram.Domain.Repositories.Interfaces.Blob
{
    public interface IVideoBlobService
    {
        public Task<string?> UploadPostVideoAsync(IFile file, Guid userId);
        public Task<string?> UploadReelAsync(IFile file, Guid userId);
        public Task<bool> DeleteVideoAsync(string url);
        public Task<string?> UploadStoryAsync(IFile file, Guid userId);
    }
}
