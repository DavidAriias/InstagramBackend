using Instagram.App.UseCases.UserCase.Types;

namespace Instagram.App.UseCases.UserCase.GetProfile
{
    public interface IGetProfileCase
    {
        public Task<UserTypeOut?> GetProfile(Guid userId);
    }
}
