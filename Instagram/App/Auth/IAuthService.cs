using Instagram.App.UseCases.Types.Shared;

namespace Instagram.App.Auth
{
    public interface IAuthService
    {
        public Task<AuthTypeOut> AuthenticateAsync(string usernameOrEmail,string pass, AuthMethod method);
        public Task<AuthTypeOut> GetNewRefreshToken(AuthTypeIn auth);
        public Task<AuthTypeOut> GetNewAccessToken(AuthTypeIn auth);
        public Task<ResponseType<string>> CloseSession(AuthTypeIn auth);
    }
}
