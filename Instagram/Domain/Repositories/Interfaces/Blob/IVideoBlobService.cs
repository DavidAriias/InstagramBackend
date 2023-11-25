namespace Instagram.Domain.Repositories.Interfaces.Blob
{
    public interface IVideoBlobService
    {
        public Task<string?> UploadPostVideoAsync(IFormFile file, Guid userId);
        public Task<string?> UploadReelAsync(IFormFile file, Guid userId);
        public Task<bool> DeleteVideoAsync(string url);
        public Task<string?> UploadStoryAsync(IFormFile file, Guid userId);
    }
}
