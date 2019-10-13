using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace Qart.Testing.Framework.Http
{
    public static class HttpTaskExtensions
    {
        public static Task<HttpResponseMessage> AssertSuccessAsync(this Task<HttpResponseMessage> task, ILogger logger)
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
                var exception = new AssertException("Response code does not indicate success.");
                exception.Data.Add("content", task.Result.Content.ReadAsStringAsync().Result);
                throw exception;
            }

            return task;
        }

        public static async Task<string> GetContentAssertSuccessAsync(this Task<HttpResponseMessage> task, ILogger logger)
        {
            var responseMessage = await task.AssertSuccessAsync(logger).ConfigureAwait(false);
            var message = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return message;
        }

        public static string GetContentAssertSuccess(this Task<HttpResponseMessage> task, ILogger logger)
        {
            return task.GetContentAssertSuccessAsync(logger).GetAwaiter().GetResult();
        }
    }
}
