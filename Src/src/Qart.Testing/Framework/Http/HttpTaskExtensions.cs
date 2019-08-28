using System.Net.Http;
using System.Threading.Tasks;

namespace Qart.Testing.Framework.Http
{
    public static class HttpTaskExtensions
    {
        public static Task<HttpResponseMessage> EnsureSuccess(this Task<HttpResponseMessage> task)
        {
            if (task.IsCanceled)
                throw new AssertException("Request is cancelled.");

            if (task.IsFaulted)
                throw new AssertException("Request has failed.", task.Exception);

            if (!task.Result.IsSuccessStatusCode)
            {
                var exception = new AssertException("Response code does not indicate success.");
                exception.Data.Add("content", task.Result.Content.ReadAsStringAsync().Result);
                throw exception;
            }

            return task;
        }

        public static string GetContentEnsureSuccess(this Task<HttpResponseMessage> task)
        {
            return task.EnsureSuccess().Result.Content.ReadAsStringAsync().Result;
        }
    }
}
