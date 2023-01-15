using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Text.Json;

namespace BlazorApplication.HttpRepository
{
    public class TaskCategoryHttpRepository : ITaskCategoryHttpRepository
    {
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _options;
        private readonly IConfiguration _configuration;
        private readonly Models.BackEndConnections _backEndConnections;

        public TaskCategoryHttpRepository(IAccessTokenProvider accessTokenProvider, HttpClient client, IConfiguration configuration) 
        {
            _accessTokenProvider = accessTokenProvider;
            _client = client;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _configuration = configuration;
            _backEndConnections = _configuration.GetSection("ConnectionStrings").Get<Models.BackEndConnections>();
        }
        public async Task<List<Models.TaskCategory>> GetTaskCategory()
        {
            await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            var response = await _client.GetAsync(_backEndConnections.CSharpUri + "TaskCategory");

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            };

            return JsonSerializer.Deserialize<List<Models.TaskCategory>>(content, _options);
        }
    }
}
