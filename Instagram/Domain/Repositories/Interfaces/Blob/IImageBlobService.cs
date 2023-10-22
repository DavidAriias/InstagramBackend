using Instagram.Domain.Entities;

namespace Instagram.Domain.Repositories.Interfaces.Blob
{
    public interface IImageBlobService
    {
        public Task<string?> UploadPostImageAsync(IFile file, Guid userId);
        public Task<string> UploadProfileImageAsync(IFile file, Guid userId);
        public Task<string> UpdateProfileImageAsync(IFile file, string imageUrl);
        public Task<bool> DeleteImageAsync(string imageUrl);
        public Task<string?> UploadStoryAsync(IFile file, Guid userId);
    }
}
