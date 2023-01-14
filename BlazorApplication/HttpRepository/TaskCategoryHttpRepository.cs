using BlazorApplication.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BlazorApplication.HttpRepository
{
    public class TaskCategoryHttpRepository : ITaskCategoryHttpRepository
    {
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _options;

        public TaskCategoryHttpRepository(IAccessTokenProvider accessTokenProvider, HttpClient client) 
        {
            _accessTokenProvider = accessTokenProvider;
            _client = client;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        public async Task<List<TaskCategory>> GetTaskCategory()
        {
            await RequestAuthToken();

            var response = await _client.GetAsync("https://localhost:7192/TaskCategory");

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            return JsonSerializer.Deserialize<List<TaskCategory>>(content, _options);
        }

        private async System.Threading.Tasks.Task RequestAuthToken()
        {
            var requestToken = await _accessTokenProvider.RequestAccessToken();
            requestToken.TryGetToken(out var token);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
        }
    }
}
