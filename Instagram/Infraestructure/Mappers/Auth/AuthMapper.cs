using Instagram.App.Auth;
using Instagram.Domain.Entities.Auth;
using Instagram.Infraestructure.Data.Models.SQL;

namespace Instagram.Infraestructure.Mappers.Auth
{
    public static class AuthMapper
    {
        public static RefreshToken MapAuthEntityToRefreshToken(AuthEntity authEntity) => new()
        {
            TokenValue = authEntity.RefreshToken,
            UserId = authEntity.UserId,
        };

        public static AuthEntity MapAuthTypeOutToAuthEntity(AuthTypeOut authType) => new()
        {
            AccessToken = authType.Token,
            RefreshToken = authType.RefreshToken,
            UserId = authType.UserId
        };

        public static AuthEntity MapAuthTypeInToAuthEntity(AuthTypeIn auth) => new()
        {
            AccessToken = auth.Token!,
            RefreshToken = auth.RefreshToken,
            UserId = auth.UserId
        };
    }
}
