using System.Net;

namespace Instagram.App.UseCases.Types.Shared
{
    public class ResponseType<T>
    {
        public T? Value { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = null!;

        // Constructor estático para crear una respuesta exitosa
        public static ResponseType<T> CreateSuccessResponse(T value)
        {
            return new ResponseType<T>
            {
                Value = value,
                StatusCode = HttpStatusCode.OK,
                Message = "Operation was successful"
            };
        }

        // Constructor estático para crear una respuesta de error
        public static ResponseType<T> CreateErrorResponse(T value, HttpStatusCode statusCode)
        {
            return new ResponseType<T>
            {
                StatusCode = statusCode,
                Message = "Operation failed",
                Value = value
            };
        }
    }
}
