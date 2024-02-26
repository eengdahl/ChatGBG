using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using OpenAI_API;
using System;
using Newtonsoft.Json;
using System.IO;

public class HTTPCalls : MonoBehaviour
{
    // public OpenAIAPI api;
    private MyChatEndPoint _endpoint;
    private const string UserAgent = "okgodoit/dotnet_openai_api";
    public string Endpoint = "https://api.openai.com/v1/completions"; //were answers from chatGPT is beeing sent
    public OpenAIAPI _Api;

    public string ApiUrlFormat { get; set; } = "https://api.openai.com/{0}/{1}";





    void Start()
    {
        _Api = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY")); //collect my apikey
    }

    public string Url { get { return string.Format("https://api.openai.com/v1/chat/completions", _Api.ApiVersion, Endpoint); } }

    protected HttpClient GetClient()
    {
        HttpClient client;
        client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _Api.Auth.ApiKey);
        client.DefaultRequestHeaders.Add("api-key", _Api.Auth.ApiKey);
        client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
        if (!string.IsNullOrEmpty(_Api.Auth.OpenAIOrganization)) client.DefaultRequestHeaders.Add("OpenAI-Organization", _Api.Auth.OpenAIOrganization);

        return client;
    }


    public async Task<HttpResponseMessage> HttpRequestRaw(string url = null, HttpMethod verb = null, object postData = null, bool streaming = false)
    {
        if (string.IsNullOrEmpty(url))
            url = this.Url;

        if (verb == null)
            verb = HttpMethod.Get;

        using var client = GetClient();

        HttpResponseMessage response = null;
        string resultAsString = null;
        HttpRequestMessage req = new HttpRequestMessage(verb, url);

        if (postData != null)
        {


            if (postData is HttpContent)
            {
                req.Content = postData as HttpContent;
            }
            else
            {
                string jsonContent = JsonConvert.SerializeObject(postData, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                var stringContent = new StringContent(jsonContent, UnicodeEncoding.UTF8, "application/json");

                req.Content = stringContent;
                UnityEngine.Debug.Log(stringContent.ToString());
            }
        }
        response = await client.SendAsync(req, streaming ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead);

        if (response.IsSuccessStatusCode)
        {

            return response;
        }
        else
        {
            try
            {
                resultAsString = await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                resultAsString = "Additionally, the following error was thrown when attemping to read the response content: " + e.ToString();
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new AuthenticationException("OpenAI rejected your authorization, most likely due to an invalid API Key.  Full API response follows: " + resultAsString);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException("OpenAI had an internal server error, which can happen occasionally.  Please retry your request.  " + GetErrorMessage(resultAsString, response, Endpoint, url));
            }
            else
            {
                throw new HttpRequestException(GetErrorMessage(resultAsString, response, Endpoint, url));
            }
        }
    }

    internal async Task<string> HttpGetContent<T>(string url = null)
    {
        var response = await HttpRequestRaw(url);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<T> HttpRequest<T>(string url = null, HttpMethod verb = null, object postData = null) where T : ApiResultBase
    {
        var response = await HttpRequestRaw(url, verb, postData);
        string resultAsString = await response.Content.ReadAsStringAsync();

        var res = JsonConvert.DeserializeObject<T>(resultAsString);
        try
        {
            res.Organization = response.Headers.GetValues("Openai-Organization").FirstOrDefault();
            res.RequestId = response.Headers.GetValues("X-Request-ID").FirstOrDefault();
            res.ProcessingTime = TimeSpan.FromMilliseconds(int.Parse(response.Headers.GetValues("Openai-Processing-Ms").First()));
            res.OpenaiVersion = response.Headers.GetValues("Openai-Version").FirstOrDefault();
            if (string.IsNullOrEmpty(res.Model))
                res.Model = response.Headers.GetValues("Openai-Model").FirstOrDefault();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log($"Issue parsing metadata of OpenAi Response.  Url: {url}, Error: {e.ToString()}, Response: {resultAsString}.  This is probably ignorable.");
        }

        return res;
    }

    internal async Task<T> HttpGet<T>(string url = null) where T : ApiResultBase
    {
        return await HttpRequest<T>(url, HttpMethod.Get);
    }


    internal async Task<T> HttpPost<T>(string url = null, object postData = null) where T : ApiResultBase
    {
        return await HttpRequest<T>(url, HttpMethod.Post, postData);
    }

    internal async Task<T> HttpDelete<T>(string url = null, object postData = null) where T : ApiResultBase
    {
        return await HttpRequest<T>(url, HttpMethod.Delete, postData);
    }
    internal async Task<T> HttpPut<T>(string url = null, object postData = null) where T : ApiResultBase
    {
        return await HttpRequest<T>(url, HttpMethod.Put, postData);
    }


    //returns string with errors
    protected string GetErrorMessage(string resultAsString, HttpResponseMessage response, string name, string description = "")
    {
        return $"Error at {name} ({description}) with HTTP status code: {response.StatusCode}. Content: {resultAsString ?? "<no content>"}";
    }

    protected async IAsyncEnumerable<T> HttpStreamingRequest<T>(string url = null, HttpMethod verb = null, object postData = null) where T : ApiResultBase
    {
        var response = await HttpRequestRaw(url, verb, postData, true);

        string organization = null;
        string requestId = null;
        TimeSpan processingTime = TimeSpan.Zero;
        string openaiVersion = null;
        string modelFromHeaders = null;

        try
        {
            organization = response.Headers.GetValues("Openai-Organization").FirstOrDefault();
            requestId = response.Headers.GetValues("X-Request-ID").FirstOrDefault();
            processingTime = TimeSpan.FromMilliseconds(int.Parse(response.Headers.GetValues("Openai-Processing-Ms").First()));
            openaiVersion = response.Headers.GetValues("Openai-Version").FirstOrDefault();
            modelFromHeaders = response.Headers.GetValues("Openai-Model").FirstOrDefault();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log($"Issue parsing metadata of OpenAi Response.  Url: {url}, Error: {e.ToString()}.  This is probably ignorable.");
        }

        string resultAsString = "";

        using (var stream = await response.Content.ReadAsStreamAsync())
        using (StreamReader reader = new StreamReader(stream))
        {
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                resultAsString += line + Environment.NewLine;

                if (line.StartsWith("data:"))
                    line = line.Substring("data:".Length);

                line = line.TrimStart();

                if (line == "[DONE]")
                {
                    yield break;
                }
                else if (line.StartsWith(":"))
                { }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    var res = JsonConvert.DeserializeObject<T>(line);

                    res.Organization = organization;
                    res.RequestId = requestId;
                    res.ProcessingTime = processingTime;
                    res.OpenaiVersion = openaiVersion;
                    if (string.IsNullOrEmpty(res.Model))
                        res.Model = modelFromHeaders;

                    yield return res;
                }
            }
        }
    }


}
