using System;
using System.IO;
using System.Net;
using System.Text;
using PatchKit.Network;
using UnityEngine;

namespace PatchKit.Api
{
    internal class ApiResponse : IApiResponse
    {
        public ApiResponse(IHttpResponse httpResponse)
        {
            HttpResponse = httpResponse;

            var responseStream = HttpResponse.ContentStream;

            if (HttpResponse.CharacterSet == null || responseStream == null)
            {
                throw new WebException("Invalid response from API server.");
            }

            var responseEncoding = Encoding.GetEncoding(HttpResponse.CharacterSet);

            using (var streamReader = new StreamReader(responseStream, responseEncoding))
            {
                Body = streamReader.ReadToEnd();
            }
        }

        public IHttpResponse HttpResponse { get; private set; }

        public string Body { get; private set; }

        public T GetJson<T>()
        {
            return JsonUtility.FromJson<T>(Body);
        }

        void IDisposable.Dispose()
        {
            HttpResponse.Dispose();
        }
    }
}
