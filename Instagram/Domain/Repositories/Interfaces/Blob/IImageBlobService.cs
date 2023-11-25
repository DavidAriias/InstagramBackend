using Instagram.Domain.Entities;

namespace Instagram.Domain.Repositories.Interfaces.Blob
{
    public interface IImageBlobService
    {
        public Task<string?> UploadPostImageAsync(IFormFile file, Guid userId);
        public Task<string> UploadProfileImageAsync(IFormFile file, Guid userId);
        public Task<string> UpdateProfileImageAsync(IFormFile file, string imageUrl);
        public Task<bool> DeleteImageAsync(string imageUrl);
        public Task<string?> UploadStoryAsync(IFormFile file, Guid userId);
    }
}
