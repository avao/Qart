using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace Qart.Testing.Framework.Http
{
    public static class HttpTaskExtensions
    {
        public static async Task<HttpResponseMessage> AssertSuccessAsync(this Task<HttpResponseMessage> task, ILogger logger)
        {
            if (task.IsCanceled)
            {
                logger.LogTrace("Task is cancelled");
                throw new AssertException("Request is cancelled.");
            }

            if (task.IsFaulted)
            {
                logger.LogTrace("Task is faulted");
                throw new AssertException("Request has failed.", task.Exception);
            }

            logger.LogTrace("Response code [{code}]", task.Result.StatusCode);
            if (!task.Result.IsSuccessStatusCode)
            {
                var content = await task.Result.Content.ReadAsStringAsync().ConfigureAwait(false);
                
                var exception = new AssertException($"Response code {task.Result.StatusCode} does not indicate success.");
                exception.Data.Add("content", content);

                logger.LogTrace("Returned {content}", content);

                throw exception;
            }

            return await task;
        }

        public static async Task<string> GetContentAssertSuccessAsync(this Task<HttpResponseMessage> task, ILogger logger)
        {
            var responseMessage = await task.AssertSuccessAsync(logger).ConfigureAwait(false);
            var message = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return message;
        }
    }
}
