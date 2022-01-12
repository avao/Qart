using Qart.Core.DataStore;
using Qart.Testing.Context;
using Qart.Testing.Framework;
using Qart.Testing.Framework.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Qart.Testing.ActionPipeline.Actions.Http
{
    public class HttpManyAction : HttpAction
    {
        public HttpManyAction(HttpMethod httpMethod, string url, string sourceKey = null, string targetKey = null, string mediaType = null, IHttpRequestMessageProcessor httpRequestMessageProcessor = null)
            : this(httpMethod, url, (testCase, pipelineContext) => pipelineContext.GetRequiredItem<IEnumerable<string>>(sourceKey), sourceKey, targetKey, mediaType, httpRequestMessageProcessor)
        { }

        public HttpManyAction(HttpMethod httpMethod, string path, string url, string sourceKey = null, string targetKey = null, string mediaType = null, IHttpRequestMessageProcessor httpRequestMessageProcessor = null)
            : this(httpMethod, url, (testCase, pipelineContext) => testCase.GetItemIds(path), sourceKey, targetKey, mediaType, httpRequestMessageProcessor)
        { }

        private HttpManyAction(HttpMethod httpMethod, string url, Func<TestCase, TestCaseContext, IEnumerable<string>> contentsFunc, string sourceKey, string targetKey, string mediaType, IHttpRequestMessageProcessor httpRequestMessageProcessor)
            : base(httpMethod
                  , url
                  , targetKey ?? sourceKey
                  , (testCase, testCaseContext) => contentsFunc(testCase, testCaseContext).Select(id => HttpClientExtensions.CreateContent(testCase.GetContent(id), mediaType)).ToArray()
                  , httpRequestMessageProcessor
                  , true)
        { }
    }
}
