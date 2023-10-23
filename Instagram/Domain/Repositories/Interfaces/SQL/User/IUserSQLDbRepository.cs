using Instagram.Domain.Entities.User;

namespace Instagram.Domain.Repositories.Interfaces.SQL.User
{
    public interface IUserSQLDbRepository
    {
        public Task<UserEntity?> CreateUser(UserEntity user);
        public Task<UserEntity?> CreateUserDescription(UserEntity user);
        public Task<UserEntity?> FindUserById(Guid id);
        public Task<UserEntity?> FindUserByUsername(string username);
        public Task<UserEntity?> FindUserByEmail(string email);
        public Task<UserEntity?> FindUserByPhoneNumber(string phoneNumber);
        public Task<string?> FindImageProfileById(Guid userId);
        public Task<bool> UpdateUsername(string username, Guid userId);
        public Task<bool> UpdateBirthday(DateOnly birthday, Guid userId);
        public Task<bool> UpdatePassword(string pass, Guid userId);
        public Task<bool> UpdateName(string name, Guid userId);
        public Task<bool> UpdateLink(LinkEntity link, Guid userId);
        public Task<bool> UpdateBiography(string bio, Guid userId);
        public Task<bool> UpdatePronoun(string pronoun, Guid userId);
        public Task<bool> UpdateImageProfile(Guid userId, string url);
        public Task<DateTime> GetLastBirthdayChangeDate(Guid userId);
        public int GetNumberChangesOnName(Guid userId);
        public Task<UserEntity?> GetAllDataAboutUserById(Guid userId);
        public Task<bool> UpdateIsPrivateProfile(Guid userId);
        public Task<bool> UpdateIsVerificateProfile(Guid userId);
    }
}
