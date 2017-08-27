using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Chat.WebServer
{
    /// <summary>
    /// 转发Http请求
    /// </summary>
    public class HandleForword
    {

        public async Task Handle(HttpContext context)
        {
            // req: url | method | headers | body
            // resp: headers | body

            try
            {
                var requestInfo = GetRequestInfo(context);
                var requestMessage = GetRequestMessage(requestInfo);
                var rep = await new HttpClient().SendAsync(requestMessage);
                var responseInfo = new ResponseInfo();
                foreach (var header in rep.Headers)
                {
                    responseInfo.Headers.Add(header.Key, new StringValues(header.Value.ToArray()));
                }
                responseInfo.Body = await rep.Content.ReadAsByteArrayAsync();
                await context.Response.Body.WriteAsync(responseInfo.Body, 0, responseInfo.Body.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static HttpRequestMessage GetRequestMessage(RequestInfo requestInfo)
        {
            var requestMessage = new HttpRequestMessage(new HttpMethod(requestInfo.HttpMethod), requestInfo.Url);
            //foreach (var header in requestInfo.Headers)
            //{
            //    //if (header.Key.ToLower() != "host")
            //    //{
            //        requestMessage.Headers.Add(header.Key, header.Value.ToList());
            //    //}
            //}
            if (requestInfo.HttpMethod.ToLower() == "post")
                requestMessage.Content = new StreamContent(requestInfo.Body);
            return requestMessage;
        }

        private static RequestInfo GetRequestInfo(HttpContext context)
        {
            var basePath = "www.baidu.com";
            var requestInfo = new RequestInfo();
            var request = context.Request;
            requestInfo.Url = "http://" + basePath + request.Path.Value + request.QueryString;
            requestInfo.HttpMethod = request.Method;
            request.Headers.Add("Host", basePath);
            //requestInfo.Headers = request.Headers;
            if (requestInfo.HttpMethod.ToLower() == "post")
            {
                requestInfo.Body = request.Body;
            }
            return requestInfo;
        }
    }


    public class RequestInfo
    {
        public string Url { get; set; }

        public string HttpMethod { get; set; }

        public IDictionary<string, StringValues> Headers { get; set; } = new Dictionary<string, StringValues>();

        public Stream Body { get; set; }
    }

    public class ResponseInfo
    {
        public IDictionary<string, StringValues> Headers { get; set; } = new Dictionary<string, StringValues>();

        public byte[] Body { get; set; }
    }
}
