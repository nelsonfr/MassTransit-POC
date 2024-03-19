using Polly;
using System.Net;
using Types;

namespace ConsumerAT.Processor
{
    public class MessageProcessor : IMessageProcessor
    {
        private ILogger<MessageProcessor> _logger;

        public MessageProcessor(ILogger<MessageProcessor> logger)
        {
           _logger = logger;
        }
        public async Task ProcessMessage(MyMessage message)
        {
            var maxRetries = 4;
            var retryPolicy = Policy.Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(response =>
            {
                return response.StatusCode == HttpStatusCode.TooManyRequests; // 429 status code
            })            
            .WaitAndRetryAsync(maxRetries, // number of retries
                sleepDurationProvider:(retryCount, response, context) =>
                {

                    if (response == null) 
                        return TimeSpan.FromSeconds(Math.Pow(2, retryCount));

                    var retryAfterHeader = response.Result.Headers.GetValues("Retry-After").FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(retryAfterHeader))
                    {                                    
                        return  TimeSpan.FromSeconds(double.Parse(retryAfterHeader));                        
                    }
                   
                    return TimeSpan.FromSeconds(Math.Pow(2, retryCount));
                },
                
                onRetryAsync:(_, _, _, _) => { return Task.CompletedTask; }
                
            );

            using (var httpClient = new HttpClient())
            {
               
               var response = await retryPolicy.ExecuteAsync(async () => {
                   return await httpClient.GetAsync("https://localhost:7090/ThirdParty/dummyProcess");
                });

                //All polly retries are burned, and still too many requests, lets throw an exception to put the message back to the queue and let the consumer retry.
                if(response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    throw new Exception("");
                }
            }
        }
    }
}
