﻿using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Qart.Testing.Framework.Http
{
    public static class HttpClientExtensions
    {
        public static string GetEnsureSuccess(this HttpClient client, string relativeUri, ILogger logger)
        {
            return client.SendEnsureSuccessAsync(HttpMethod.Get, relativeUri, null, logger).GetAwaiter().GetResult();
        }

        public static string PostEnsureSuccess(this HttpClient client, string relativeUri, string content, ILogger logger)
        {
            return client.SendEnsureSuccessAsync(HttpMethod.Post, relativeUri, content, logger).GetAwaiter().GetResult();
        }

        public static string PutEnsureSuccess(this HttpClient client, string relativeUri, string content, ILogger logger)
        {
            return client.SendEnsureSuccessAsync(HttpMethod.Put, relativeUri, content, logger).GetAwaiter().GetResult();
        }

        public static string DeleteEnsureSuccess(this HttpClient client, string relativeUri, ILogger logger)
        {
            return client.SendEnsureSuccessAsync(HttpMethod.Delete, relativeUri, null, logger).GetAwaiter().GetResult();
        }

        private static StringContent CreateContent(string content)
        {
            var mediaType = content.StartsWith("<?xml")
                ? "application/xml"
                : "application/json";
            return new StringContent(content, Encoding.UTF8, mediaType);
        }

        private static Task<string> SendEnsureSuccessAsync(this HttpClient client, HttpMethod httpMethod, string relativeUri, string content, ILogger logger)
        {
            logger.LogDebug("Executing {HttpMethod} on {Url}", httpMethod, relativeUri);
            var request = new HttpRequestMessage(httpMethod, relativeUri);
            if (!string.IsNullOrEmpty(content))
            {
                logger.LogTrace("Sending {Content}", content);
                request.Content = CreateContent(content);
            }
            return client.SendAsync(request).GetContentAssertSuccessAsync(logger);
        }
    }
}
