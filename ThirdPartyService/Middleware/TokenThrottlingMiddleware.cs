using ConsumerAT.MiddleWare;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;

namespace ConsumerAT.MiddleWare
{
    public class TokenThrottlingMiddleware
    {
        private readonly RequestDelegate _next;
        private TokenBucket _tokenBucket;

        public TokenThrottlingMiddleware(RequestDelegate next, int numberOfMessages, int seconds)
        {
            _next = next;
            _tokenBucket = new TokenBucket(numberOfMessages, seconds*1000);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (_tokenBucket.UseToken())
            {
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = 429;
                context.Response.ContentType = "application/json";
                var remainingTime = _tokenBucket.GetRemainingTime();
                context.Response.Headers.RetryAfter = remainingTime.ToString();
                // Create a custom response body
                string responseBody = $"Request throttled. Too many requests. Try again in {remainingTime} seconds";
                
                // Convert the string to a byte array and write it to the response stream
                byte[] data = Encoding.UTF8.GetBytes(responseBody);
                await context.Response.Body.WriteAsync(data, 0, data.Length);
            }
        }
    }

    public static class ThrottlingMiddlewareExtensions
    {
        public static void UseRequestRateLimiter(this IApplicationBuilder builder, int numberOfMessages, int seconds)
        {
            builder.UseMiddleware<TokenThrottlingMiddleware>(numberOfMessages, seconds);
        }
    }
}
