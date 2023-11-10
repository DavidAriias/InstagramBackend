using System.Net;

namespace Instagram.App.Auth
{
    public class AuthTypeOut
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = null!;
        public long ExpiresIn { get; set; }
        public string RefreshToken { get; set; } = null!;
        public long RefreshTokenExpireIn { get; set; }
        public string? Error { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        // Constructor
        public AuthTypeOut(
            Guid userId,
            string token,
            long expiresIn,
            string refreshToken,
            long refreshTokenExpireIn,
            string? error,
            HttpStatusCode statusCode
            )
        {
            UserId = userId;
            Token = token;
            ExpiresIn = expiresIn;
            RefreshToken = refreshToken;
            RefreshTokenExpireIn = refreshTokenExpireIn;
            Error = error;
            StatusCode = statusCode;
        }

        // Constructor para una respuesta exitosa
        public static AuthTypeOut CreateSuccess(
            Guid userId,
            string token,
            long expiresIn,
            string refreshToken,
            long refreshTokenExpireIn,
            HttpStatusCode statusCode
           )
        {
            return new AuthTypeOut(userId, token, expiresIn, refreshToken, refreshTokenExpireIn, null, statusCode);
        }

        // Constructor para un mensaje de error
        public static AuthTypeOut CreateError(string errorMessage, HttpStatusCode statusCode)
        {
            return new AuthTypeOut(Guid.Empty, "", 0, "", 0, errorMessage, statusCode);
        }
    }

}
