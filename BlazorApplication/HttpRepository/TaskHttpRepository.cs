using BlazorApplication.Features;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Json;
using BlazorApplication.Interfaces;

namespace BlazorApplication.HttpRepository
{
    public class TaskHttpRepository : ITaskHttpRepository
	{
        private readonly IAccessTokenProvider _tokenProvider;
		private readonly HttpClient _client;
		private readonly JsonSerializerOptions _options;
		private readonly IConfiguration _configuration;
		private readonly Models.BackEndConnections _backEndConnections;

        public TaskHttpRepository(IAccessTokenProvider tokenProvider, HttpClient client, IConfiguration configuration)
        {
            _tokenProvider = tokenProvider;
            _client = client;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _configuration = configuration;
			_backEndConnections = _configuration.GetSection("ConnectionStrings").Get<Models.BackEndConnections>();
        }

        public async Task CreateTask(Models.Task task)
		{
			var content = JsonSerializer.Serialize(task);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            await AddToken.RequestAuthToken(_tokenProvider, _client);

			var postResult = await _client.PostAsync(_backEndConnections.CSharpUri + "Task", bodyContent);
			var postContent = await postResult.Content.ReadAsStringAsync();

			if (!postResult.IsSuccessStatusCode)
			{
				throw new ApplicationException(postContent);
			}
		}

		public async Task DeleteProduct(int id)
		{
			var url = Path.Combine(_backEndConnections.CSharpUri + "Task", id.ToString());

            await AddToken.RequestAuthToken(_tokenProvider, _client);

            var deleteResult = await _client.DeleteAsync(url);
			var deleteContent = await deleteResult.Content.ReadAsStringAsync();

			if (!deleteResult.IsSuccessStatusCode)
			{
				throw new ApplicationException(deleteContent);
			}
		}

        public async Task<PagingResponse<Models.TaskWithTimesDto>> GetSubmittedTasksByTeamId(TaskParameters taskParameters, string teamId)
        {
            Console.WriteLine(JsonSerializer.Serialize(taskParameters));
            Console.WriteLine("Switch off: " + (taskParameters.switchOff ? "1" : "0"));

            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = taskParameters.PageNumber.ToString(),
                ["switchOffString"] = taskParameters.switchOff ? "1" : "0"
            };

            await AddToken.RequestAuthToken(_tokenProvider, _client);

            var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.CSharpUri + "Task/submitted/byTeamId/" + teamId, queryStringParam));

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var pagingResponse = new PagingResponse<Models.TaskWithTimesDto>
            {
                Items = JsonSerializer.Deserialize<List<Models.TaskWithTimesDto>>(content, _options),
                MetaData = JsonSerializer.Deserialize<Models.MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
            };

			pagingResponse.SuccessRequest = true;

            return pagingResponse;
        }

        public async Task<Models.Task> GetTaskById(string id)
		{
			var url = Path.Combine(_backEndConnections.CSharpUri + "Task", id);

			await AddToken.RequestAuthToken(_tokenProvider, _client);
			//Console.WriteLine(url);
			var response = await _client.GetAsync(url);
			var content = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				throw new ApplicationException(content);
			}

			var task = JsonSerializer.Deserialize<Models.Task>(content, _options);

			task.SuccessRequest = true;

			return task;
		}

		public async Task<PagingResponse<Models.Task>> GetTasks(TaskParameters taskParameters)
		{
			var queryStringParam = new Dictionary<string, string> { };

            if (taskParameters.PageNumber > 0)
			{
				queryStringParam.Add("pageNumber", taskParameters.PageNumber.ToString());
			};

			if (taskParameters.SearchString != string.Empty)
			{
				queryStringParam.Add("searchString", taskParameters.SearchString);
			};

            queryStringParam.Add("switchOffString", taskParameters.switchOff ? "1" : "0");

            if (taskParameters.OrderBy != string.Empty)
			{
				queryStringParam.Add("orderby", taskParameters.OrderBy);
			};

			await AddToken.RequestAuthToken(_tokenProvider, _client);

			var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.CSharpUri + "Task/extended", queryStringParam));

			var content = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				throw new ApplicationException(content);
			}
			var pagingResponse = new PagingResponse<Models.Task>
			{
				Items = JsonSerializer.Deserialize<List<Models.Task>>(content, _options),
				MetaData = JsonSerializer.Deserialize<Models.MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
			};

			pagingResponse.SuccessRequest = true;

			return pagingResponse;
		}

		public async Task<PagingResponse<Models.Task>> GetTasksByCompetitionId(TaskParameters taskParameters, string id)
		{
			Console.WriteLine(JsonSerializer.Serialize(taskParameters));
			Console.WriteLine("Switch off: " + (taskParameters.switchOff ? "1" : "0"));

			var queryStringParam = new Dictionary<string, string>
			{
				["pageNumber"] = taskParameters.PageNumber.ToString(),
				["switchOffString"] = taskParameters.switchOff ? "1" : "0"
			};

			await AddToken.RequestAuthToken(_tokenProvider, _client);

			var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.CSharpUri + "Task/byCompetitionId/" + id, queryStringParam));

			var content = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				throw new ApplicationException(content);
			}
			var pagingResponse = new PagingResponse<Models.Task>
			{
				Items = JsonSerializer.Deserialize<List<Models.Task>>(content, _options),
				MetaData = JsonSerializer.Deserialize<Models.MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
			};

			pagingResponse.SuccessRequest = true;
			
			return pagingResponse;
		}

        public async Task<PagingResponse<Models.TaskWithTimesDto>> GetTasksByTeamId(TaskParameters taskParameters, string teamId)
        {
            //Console.WriteLine(JsonSerializer.Serialize(taskParameters));
            //Console.WriteLine("Switch off: " + (taskParameters.switchOff ? "1" : "0"));

            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = taskParameters.PageNumber.ToString(),
                ["switchOffString"] = taskParameters.switchOff ? "1" : "0"
            };

            await AddToken.RequestAuthToken(_tokenProvider, _client);

            var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.CSharpUri + "Task/byTeamId/" + teamId, queryStringParam));

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var pagingResponse = new PagingResponse<Models.TaskWithTimesDto>
            {
                Items = JsonSerializer.Deserialize<List<Models.TaskWithTimesDto>>(content, _options),
                MetaData = JsonSerializer.Deserialize<Models.MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
            };

			pagingResponse.SuccessRequest = true;

            return pagingResponse;
        }

        public async Task UpdateTask(Models.Task task)
		{
			var content = JsonSerializer.Serialize(task);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

			var url = Path.Combine(_backEndConnections.CSharpUri + "Task", task.Id.ToString());

            await AddToken.RequestAuthToken(_tokenProvider, _client);

            var putResult = await _client.PutAsync(url, bodyContent);
			var putContent = await putResult.Content.ReadAsStringAsync();

			if (!putResult.IsSuccessStatusCode)
			{
				throw new ApplicationException(putContent);
			}
		}

	}
}