using Instagram.App.Auth;
using Instagram.App.UseCases.Types.Shared;
using Instagram.App.UseCases.UserCase.Types;

namespace Instagram.App.UseCases.UserCase.SignUp
{
    public interface ISignUpCase
    {
        public Task<ResponseType<string>> CreateUser(UserTypeIn user);
    }
}
