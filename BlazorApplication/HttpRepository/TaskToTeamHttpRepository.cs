using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Pages;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.WebUtilities;
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

		public TaskToTeamHttpRepository(IAccessTokenProvider tokenProvider, HttpClient client, IConfiguration configuration)
		{
			_tokenProvider = tokenProvider;
			_client = client;
			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
			_configuration = configuration;
			_backEndConnections = _configuration.GetSection("ConnectionStrings").Get<Models.BackEndConnections>();
		}
		public async System.Threading.Tasks.Task AddTaskToTeam(TaskToTeam taskToTeam)
		{
			var content = JsonSerializer.Serialize(taskToTeam);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

			await AddToken.RequestAuthToken(_tokenProvider, _client);

			try
			{
                var postResult = await _client.PostAsync(_backEndConnections.CSharpUri + "TaskToTeam", bodyContent);
                var postContent = await postResult.Content.ReadAsStringAsync();

                if (!postResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(postContent);
                }
            }
			catch(Exception ex)
			{
                throw new System.Exception("Oops! Something went wrong while adding a task to the team!", ex);
            }
        }

		public async System.Threading.Tasks.Task DeleteTeamTaskByTaskIdAndTeamId(string taskId, string teamId)
		{
			var queryStringParam = new Dictionary<string, string>
			{
				["taskId"] = taskId.ToString(),
				["teamId"] = teamId.ToString()
			};

			var url = Path.Combine(_backEndConnections.CSharpUri + "TaskToTeam");

			await AddToken.RequestAuthToken(_tokenProvider, _client);

			try
			{
                var deleteResult = await _client.DeleteAsync(QueryHelpers.AddQueryString(url, queryStringParam));
                var deleteContent = await deleteResult.Content.ReadAsStringAsync();

                if (!deleteResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(deleteContent);
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while removing a task from the team!", ex);
            }
        }

		public async System.Threading.Tasks.Task SubmitTask(string taskId, string teamId, SubmitTaskDataDto submitTaskDataDto)
		{
			var queryStringParam = new Dictionary<string, string>
			{
				["taskId"] = taskId.ToString(),
				["teamId"] = teamId.ToString()
			};

			var url = Path.Combine(_backEndConnections.CSharpUri + "TaskToTeam/submit");

			await AddToken.RequestAuthToken(_tokenProvider, _client);

			try
			{
                var bodyContent = new StringContent(JsonSerializer.Serialize(submitTaskDataDto), Encoding.UTF8, "application/json");

                var putResult = await _client.PutAsync(QueryHelpers.AddQueryString(url, queryStringParam), bodyContent);
                var putContent = await putResult.Content.ReadAsStringAsync();

                if (!putResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(putContent);
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while submitting a task!", ex);
            }
        }
	}
}
