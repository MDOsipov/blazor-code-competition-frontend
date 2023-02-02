using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Pages;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Text;
using System.Text.Json;

namespace BlazorApplication.HttpRepository
{
	public class TaskToTeamHttpRepository : ITaskToTeamHttpRepository
	{
		private readonly IAccessTokenProvider _tokenProvider;
		private readonly HttpClient _client;
		private readonly JsonSerializerOptions _options;
		private readonly IConfiguration _configuration;
		private readonly Models.BackEndConnections _backEndConnections;
        private readonly ILogger<TaskToTeamHttpRepository> _logger;

        public TaskToTeamHttpRepository(IAccessTokenProvider tokenProvider, HttpClient client, IConfiguration configuration, ILogger<TaskToTeamHttpRepository> logger)
		{
			_tokenProvider = tokenProvider;
			_client = client;
			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
			_configuration = configuration;
			_backEndConnections = _configuration.GetSection("ConnectionStrings").Get<Models.BackEndConnections>();
			_logger = logger;
		}
		public async System.Threading.Tasks.Task AddTaskToTeam(TaskToTeam taskToTeam)
		{
            _logger.LogInformation("Add task to team http repository method is called");
            var content = JsonSerializer.Serialize(taskToTeam);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");			

			try
			{
				await AddToken.RequestAuthToken(_tokenProvider, _client);

				var postResult = await _client.PostAsync(_backEndConnections.CSharpUri + "TaskToTeam", bodyContent);
                var postContent = await postResult.Content.ReadAsStringAsync();

                if (!postResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(postContent);
                }

                _logger.LogInformation($"Success. A new task is added to the team");
            }
            catch (Exception ex)
			{
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while adding a task to the team!", ex);
            }
        }

		public async System.Threading.Tasks.Task DeleteTeamTaskByTaskIdAndTeamId(string taskId, string teamId)
		{
            _logger.LogInformation("Delete team task by task id and team id http repository method is called");

            var queryStringParam = new Dictionary<string, string>
			{
				["taskId"] = taskId.ToString(),
				["teamId"] = teamId.ToString()
			};

			var url = Path.Combine(_backEndConnections.CSharpUri + "TaskToTeam");			

			try
			{
				await AddToken.RequestAuthToken(_tokenProvider, _client);

				var deleteResult = await _client.DeleteAsync(QueryHelpers.AddQueryString(url, queryStringParam));
                var deleteContent = await deleteResult.Content.ReadAsStringAsync();

                if (!deleteResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(deleteContent);
                }

                _logger.LogInformation($"Success. A task is removed from the team");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while removing a task from the team!", ex);
            }
        }

		public async System.Threading.Tasks.Task SubmitTask(string taskId, string teamId, SubmitTaskDataDto submitTaskDataDto)
		{
            _logger.LogInformation("Submit task http repository method is called");

            var queryStringParam = new Dictionary<string, string>
			{
				["taskId"] = taskId.ToString(),
				["teamId"] = teamId.ToString()
			};

			var url = Path.Combine(_backEndConnections.CSharpUri + "TaskToTeam/submit");			

			try
			{
				await AddToken.RequestAuthToken(_tokenProvider, _client);

				var bodyContent = new StringContent(JsonSerializer.Serialize(submitTaskDataDto), Encoding.UTF8, "application/json");

                var putResult = await _client.PutAsync(QueryHelpers.AddQueryString(url, queryStringParam), bodyContent);
                var putContent = await putResult.Content.ReadAsStringAsync();

                if (!putResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(putContent);
                }

                _logger.LogInformation($"Success. The task is submitted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while submitting a task!", ex);
            }
        }

        public async System.Threading.Tasks.Task EvaluateTask(EvaluateTaskDataDto evaluateTaskDataDto)
        {
            _logger.LogInformation($"Evaluate task http repository method with parameters: evaluateTaskDataDto: {evaluateTaskDataDto} is called");

			var bodyContent = new StringContent(JsonSerializer.Serialize(evaluateTaskDataDto), Encoding.UTF8, "application/json");

            var url = Path.Combine(_backEndConnections.CSharpUri + "TaskToTeam/evaluate");

            try
            {
                await AddToken.RequestAuthToken(_tokenProvider, _client);

                var putResult = await _client.PutAsync(url, bodyContent);
                var putContent = await putResult.Content.ReadAsStringAsync();

                if (!putResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(putContent);
                }

                _logger.LogInformation($"Success. The task is evaluated");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while evaluating a task!", ex);
            }
        }

		public async Task<TaskToTeam> GetTaskToTeamByTaskIdAndTeamIdAndCompetitionId(string taskId, string teamId, string competitionId)
		{
			_logger.LogInformation($"Get task to team by task id and team id and competition id http repository method is called with parameters: task id: {taskId}, team id: {teamId}, competition id: {competitionId}");

			var url = Path.Combine(_backEndConnections.CSharpUri + "TaskToTeam/byTaskIdAndTeamIdAndCompetitionId/", taskId, teamId, competitionId);

			try
			{
				await AddToken.RequestAuthToken(_tokenProvider, _client);

				var response = await _client.GetAsync(url);
				var content = await response.Content.ReadAsStringAsync();
				
				if (!response.IsSuccessStatusCode)
				{
					throw new ApplicationException(content);
				}

				var taskToTeamList = JsonSerializer.Deserialize<List<TaskToTeam>>(content, _options);
				var taskToTeam = taskToTeamList.FirstOrDefault();
				_logger.LogInformation($"Success. Task to team: {JsonSerializer.Serialize(taskToTeam)}");

				return taskToTeam;
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error: {ex}");
				throw new Exception("Oops! Something went wrong while getting a task by id!", ex);
			}
		}
	}
}
