using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.WebUtilities;

namespace BlazorApplication.HttpRepository
{
    public class TaskToCompetitionRepository : ITaskToCompetitionRepository
    {
        private readonly IAccessTokenProvider _tokenProvider;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _options;
        private readonly IConfiguration _configuration;
        private readonly BackEndConnections _backEndConnections;

        public TaskToCompetitionRepository(IAccessTokenProvider tokenProvider, HttpClient client, IConfiguration configuration)
        {
            _tokenProvider = tokenProvider;
            _client = client;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _configuration = configuration;
            _backEndConnections = _configuration.GetSection("ConnectionStrings").Get<BackEndConnections>();
        }
        public async System.Threading.Tasks.Task AddTaskToCompetition(TaskToCompetition taskToCompetition)
        {
            var content = JsonSerializer.Serialize(taskToCompetition);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            await AddToken.RequestAuthToken(_tokenProvider, _client);

            var postResult = await _client.PostAsync(_backEndConnections.CSharpUri + "TaskToCompetition", bodyContent);
            var postContent = await postResult.Content.ReadAsStringAsync();

            if (!postResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(postContent);
            }
        }

        public async System.Threading.Tasks.Task DeleteTaskToCompetition(int taskId, int competitionId)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["taskId"] = taskId.ToString(),
                ["competitionId"] = competitionId.ToString()
            };

            var url = Path.Combine(_backEndConnections.CSharpUri + "TaskToCompetition");

            await AddToken.RequestAuthToken(_tokenProvider, _client);

            var deleteResult = await _client.DeleteAsync(QueryHelpers.AddQueryString(url, queryStringParam));
            var deleteContent = await deleteResult.Content.ReadAsStringAsync();

            if (!deleteResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(deleteContent);
            }
        }
    }
}
