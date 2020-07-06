using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using PatchKit.Network;
using UnityEngine;
using UnityEngine.Networking;

namespace PatchKit.UnityEditorExtension.Connection
{
public class UnityHttpClient : IHttpClient
{
    private class WWWResult
    {
        public bool IsDone { get; set; }

        public string Text { get; set; }

        public Dictionary<string, string> ResponseHeaders { get; set; }
    }

    private const string ResponseEncoding = "iso-8859-2";

    private IEnumerator GetWWW(HttpGetRequest getRequest, WWWResult result)
    {
#if UNITY_2018_3_OR_NEWER
        using (var www = UnityWebRequest.Get(getRequest.Address.ToString()))
        {
            yield return www.SendWebRequest();
            while (!www.isDone)
            {
                // Wait
            }

            result.IsDone = www.isDone;
            result.ResponseHeaders = www.GetResponseHeaders();
            result.ResponseHeaders.Add(
                "STATUS",
                www.responseCode.ToString());
            if (www.downloadHandler != null)
            {
                result.Text = www.downloadHandler.text;
            }
        }
#else
        var www = new WWW(getRequest.Address.ToString());

        yield return www;

        lock (result)
        {
            result.IsDone = www.isDone;

            if (www.isDone)
            {
                result.ResponseHeaders = www.responseHeaders;
                result.Text = www.text;
            }
        }
#endif
    }

    public IHttpResponse Get(HttpGetRequest getRequest)
    {
        try
        {
            if (getRequest.Range != null)
            {
                throw new NotImplementedException();
            }

            var result = new WWWResult();

#if UNITY_2018_3_OR_NEWER
            using (var www = UnityWebRequest.Get(getRequest.Address.ToString()))
            {
                www.SendWebRequest();
                while (!www.isDone)
                {
                    // Wait
                }

                result.IsDone = www.isDone;
                result.ResponseHeaders = www.GetResponseHeaders();
                result.ResponseHeaders.Add(
                    "STATUS",
                    www.responseCode.ToString());
                if (www.downloadHandler != null)
                {
                    result.Text = www.downloadHandler.text;
                }
            }
#else
        using (var www = new WWW(getRequest.Address.ToString()))
        {
            while (!www.isDone)
            {
                // Wait
            }
            result.IsDone = www.isDone;
            result.ResponseHeaders = www.responseHeaders;
            result.Text = www.text;
        }
#endif

            lock (result)
            {
                if (!result.IsDone)
                {
                    throw new WebException(
                        "Timeout after " + getRequest.Timeout,
                        WebExceptionStatus.Timeout);
                }

                var statusCode = ReadStatusCode(result);
                return new UnityHttpResponse(
                    result.Text,
                    statusCode,
                    ResponseEncoding);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private HttpStatusCode ReadStatusCode(WWWResult result)
    {
        if (result.ResponseHeaders == null ||
            !result.ResponseHeaders.ContainsKey("STATUS"))
        {
            // Based on tests, if response doesn't contain status it has probably timed out.
            throw new WebException("Timeout.", WebExceptionStatus.Timeout);
        }

        var status = result.ResponseHeaders["STATUS"];
        var s = status.Split(' ');

#if UNITY_2018_3_OR_NEWER
        if (s.Length < 1 || !int.TryParse(s[0], out int statusCode))
        {
            throw new WebException("Timeout.", WebExceptionStatus.Timeout);
        }
#else
        if (s.Length < 3 || !int.TryParse(s[1], out int statusCode))
        {
            throw new WebException("Timeout.", WebExceptionStatus.Timeout);
        }
#endif
        return (HttpStatusCode) statusCode;
    }

    public IHttpResponse Post(HttpPostRequest postRequest)
    {
        try
        {
            var result = new WWWResult();

#if UNITY_2018_3_OR_NEWER
            using (var www = UnityWebRequest.Post(
                postRequest.Address.ToString(),
                postRequest.FormData))
            {
                www.SendWebRequest();
                while (!www.isDone)
                {
                    // Wait
                }

                result.IsDone = www.isDone;
                result.ResponseHeaders = www.GetResponseHeaders();
                result.ResponseHeaders.Add(
                    "STATUS",
                    www.responseCode.ToString());
                if (www.downloadHandler != null)
                {
                    result.Text = www.downloadHandler.text;
                }
            }
#else
            using (var www = new WWW(
                postRequest.Address.ToString(),
                Encoding.UTF8.GetBytes(postRequest.FormData)))
            {
                while (!www.isDone)
                {
                    // Wait
                }

                result.IsDone = www.isDone;
                result.ResponseHeaders = www.responseHeaders;
                result.Text = www.text;
            }
#endif
            lock (result)
            {
                if (!result.IsDone)
                {
                    throw new WebException(
                        "Timeout after " + postRequest.Timeout,
                        WebExceptionStatus.Timeout);
                }

                var statusCode = ReadStatusCode(result);

                return new UnityHttpResponse(
                    result.Text,
                    statusCode,
                    ResponseEncoding);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
}
}