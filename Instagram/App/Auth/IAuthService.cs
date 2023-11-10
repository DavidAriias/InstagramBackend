using Instagram.App.UseCases.Types.Shared;

namespace Instagram.App.Auth
{
    public interface IAuthService
    {
        public Task<AuthTypeOut> AuthenticateAsync(string usernameOrEmail,string pass, AuthMethod method);
        public Task<ResponseType<string>> CloseSession(AuthTypeIn auth);
        public Task<AuthTypeOut> CheckStatus(AuthTypeIn auth);
    }
}
