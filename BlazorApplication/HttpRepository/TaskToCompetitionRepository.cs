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
        private readonly Models.BackEndConnections _backEndConnections;
        private readonly ILogger<TaskToCompetitionRepository> _logger;

        public TaskToCompetitionRepository(IAccessTokenProvider tokenProvider, HttpClient client, IConfiguration configuration, ILogger<TaskToCompetitionRepository> logger)
        {
            _tokenProvider = tokenProvider;
            _client = client;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _configuration = configuration;
            _backEndConnections = _configuration.GetSection("ConnectionStrings").Get<Models.BackEndConnections>();
            _logger = logger;
        }
        public async System.Threading.Tasks.Task AddTaskToCompetition(TaskToCompetition taskToCompetition)
        {
            _logger.LogInformation("Add task to competition http repository method is called");

            var content = JsonSerializer.Serialize(taskToCompetition);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            await AddToken.RequestAuthToken(_tokenProvider, _client);

            try
            {
                var postResult = await _client.PostAsync(_backEndConnections.CSharpUri + "TaskToCompetition", bodyContent);
                var postContent = await postResult.Content.ReadAsStringAsync();

                if (!postResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(postContent);
                }

                _logger.LogInformation($"Success. A new task is added to the competition");
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while adding a task to the competition!", ex);
            }
        }

        public async System.Threading.Tasks.Task DeleteTaskToCompetition(int taskId, int competitionId)
        {
            _logger.LogInformation("Delete task to competition http repository method is called");

            var queryStringParam = new Dictionary<string, string>
            {
                ["taskId"] = taskId.ToString(),
                ["competitionId"] = competitionId.ToString()
            };

            var url = Path.Combine(_backEndConnections.CSharpUri + "TaskToCompetition");

            await AddToken.RequestAuthToken(_tokenProvider, _client);

            try
            {
                var deleteResult = await _client.DeleteAsync(QueryHelpers.AddQueryString(url, queryStringParam));
                var deleteContent = await deleteResult.Content.ReadAsStringAsync();

                if (!deleteResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(deleteContent);
                }

                _logger.LogInformation($"Success. A task is removed from the competition");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while removing a task from the competition!", ex);
            }
        }
    }
}
