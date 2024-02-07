using System.Net.Http.Headers;
using System.Text;
using Bw.XCommonHttpclient.Models;

namespace Bw.XCommonHttpclient;

public class CommonHttpClient<THttpSeperator> : ICommonHttpClient<THttpSeperator>
{
    private readonly HttpClient _httpClient;

    public CommonHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<ResponseModel> GetAsync(GetRequestModel model)
    {
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        AddRequestHeader(_httpClient.DefaultRequestHeaders, model.HeaderStrings);
        string url = AddQueryStringsToEndOfUrl(model.Url, model.QueryStrings);
        var response = await _httpClient.GetAsync(url);
        ResponseModel responseModel = new()
        {
            Data = await response.Content.ReadAsStringAsync(),
            StatusCode = response.StatusCode
        };
        return responseModel;
    }


    public async Task<ResponseModel> SendAsync(PostRequestModel model)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        ResponseModel responseModel = new();

        //todo try catch ?
        try
        {
            AddRequestHeader(_httpClient.DefaultRequestHeaders, model.HeaderStrings);
            var requestMessage = AddRequestBody(model.BodyStrings, model.Url);
            var response = await _httpClient.SendAsync(requestMessage);
            responseModel.Data = await response.Content.ReadAsStringAsync();
            responseModel.StatusCode = response.StatusCode;
            //TODO after fixed when Exception Framework is Ready From .....
        }
        catch (Exception ex)
        {
            responseModel.Exception = ex;
        }
        return responseModel;
    }


    private void AddRequestHeader(HttpRequestHeaders requestHeaders, Dictionary<string, string> headerString)
    {
        if (headerString != null && headerString.Any())
        {
            foreach (var item in headerString)
            {
                requestHeaders.Add(item.Key, item.Value);
            }
        }
    }

    private HttpRequestMessage AddRequestBody(Dictionary<string, string> bodyStrings,
        string uri)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
        if (bodyStrings == null || !bodyStrings.Any())
            return requestMessage;


        var keyValues = bodyStrings
            .Select(item => new KeyValuePair<string, string>(item.Key, item.Value)).ToList();
        requestMessage.Content = new FormUrlEncodedContent(keyValues);
        return requestMessage;
    }

    public static string AddQueryStringsToEndOfUrl(string url,
        Dictionary<string, string>? queryStringDictionary)
    {
        if (queryStringDictionary == null)
            return url;
        if (!queryStringDictionary.Any())
            return url;

        url += "?" +
               queryStringDictionary.Aggregate(new StringBuilder(),
                   (sb, qs) => sb.AppendFormat("{0}{1}={2}",
                       sb.Length > 0 ? "&" : "", qs.Key, qs.Value),
                   sb => sb.ToString());
        return url;
    }

}
