using Instagram.App.Auth;

namespace Instagram.App.UseCases.UserCase.SignIn
{
    public interface ISignInCase
    {
        public Task<AuthTypeOut?> SignInUser(string stringForMethod, string password, AuthMethod authMethod);
    }
}
