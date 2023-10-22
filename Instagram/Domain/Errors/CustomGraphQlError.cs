using HotChocolate.Execution;
using Instagram.config.helpers;
using System.Net;

namespace Instagram.Domain.Errors
{
    public class CustomGraphQlError : QueryException
    {
        public HttpStatusCode StatusCode { get; }
        public CustomGraphQlError(string message, HttpStatusCode statusCode)
        : base(ErrorBuilder.New()
                            .SetMessage(message)
                            .SetCode(statusCode.ToString())
                            .Build())
        {
            StatusCode = statusCode;
        }
                
    }
}
