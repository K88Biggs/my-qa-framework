using RestSharp;
using Newtonsoft.Json;
using PlaywrightTestFramework.Core.Configuration;

namespace PlaywrightTestFramework.Core.API;

public class ApiHelper
{
    private readonly RestClient _client;
    private readonly TestConfiguration _config;

    public ApiHelper(TestConfiguration config)
    {
        _config = config;
        var options = new RestClientOptions(_config.ApiBaseUrl)
        {
            Timeout = TimeSpan.FromMilliseconds(_config.ApiTimeout)
        };
        _client = new RestClient(options);
    }

    public async Task<RestResponse<T>> GetAsync<T>(string endpoint, Dictionary<string, string>? parameters = null)
    {
        var request = new RestRequest(endpoint, Method.Get);
        
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                request.AddParameter(param.Key, param.Value);
            }
        }

        return await _client.ExecuteAsync<T>(request);
    }

    public async Task<RestResponse<T>> PostAsync<T>(string endpoint, object body)
    {
        var request = new RestRequest(endpoint, Method.Post);
        request.AddJsonBody(body);
        return await _client.ExecuteAsync<T>(request);
    }

    public async Task<RestResponse<T>> PutAsync<T>(string endpoint, object body)
    {
        var request = new RestRequest(endpoint, Method.Put);
        request.AddJsonBody(body);
        return await _client.ExecuteAsync<T>(request);
    }

    public async Task<RestResponse> DeleteAsync(string endpoint)
    {
        var request = new RestRequest(endpoint, Method.Delete);
        return await _client.ExecuteAsync(request);
    }

    // API validation methods
    public bool ValidateResponse<T>(RestResponse<T> response, System.Net.HttpStatusCode expectedStatusCode)
    {
        return response.StatusCode == expectedStatusCode && response.Data != null;
    }

    public bool ValidateResponseTime(RestResponse response, int maxResponseTimeMs)
    {
        return response.ResponseTime.TotalMilliseconds <= maxResponseTimeMs;
    }
}
