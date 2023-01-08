using BlazorApplication.Features;
using BlazorApplication.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;

namespace BlazorApplication.HttpRepository
{
	public class TaskHttpRepository : ITaskHttpRepository
	{
		private readonly HttpClient _client;
		private readonly JsonSerializerOptions _options;

		public TaskHttpRepository(HttpClient client)
		{
			_client = client;
			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
		}

		public async Task<PagingResponse<Models.Task>> GetTasks(TaskParameters taskParameters)
		{
			var queryStringParam = new Dictionary<string, string>
			{
				["pageNumber"] = taskParameters.PageNumber.ToString()
			};

			var response = await _client.GetAsync(QueryHelpers.AddQueryString("Task/extended", queryStringParam));
			var content = await response.Content.ReadAsStringAsync();

			if(!response.IsSuccessStatusCode) 
			{
				throw new ApplicationException(content);
			}

			var pagingResponse = new PagingResponse<Models.Task>
			{
				Items = JsonSerializer.Deserialize<List<Models.Task>>(content, _options),
				MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
			};

			return pagingResponse;
		}
	}
}
