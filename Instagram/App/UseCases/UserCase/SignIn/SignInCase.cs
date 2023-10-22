using Instagram.App.Auth;

namespace Instagram.App.UseCases.UserCase.SignIn
{
    public class SignInCase : ISignInCase
    {
        private readonly IAuthService _authService;
        public SignInCase(IAuthService authService) 
        {
            _authService = authService;
        }
        public async Task<AuthTypeOut?> SignInUser(string stringForMethod, string password, AuthMethod authMethod)
        {
           return await _authService.AuthenticateAsync(stringForMethod, password, authMethod);
        }
    }
}
