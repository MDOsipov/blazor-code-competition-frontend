﻿using BlazorApplication.Features;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using static System.Net.WebRequestMethods;

namespace BlazorApplication.HttpRepository
{
	public class TaskHttpRepository : ITaskHttpRepository
	{
		private readonly IAccessTokenProvider _tokenProvider;
		private readonly HttpClient _client;
		private readonly JsonSerializerOptions _options;

		public TaskHttpRepository(IAccessTokenProvider tokenProvider, HttpClient client)
		{
			_tokenProvider = tokenProvider;
			_client = client;
			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
		}

		public async System.Threading.Tasks.Task CreateTask(Models.Task task)
		{
			var content = JsonSerializer.Serialize(task);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            await AddToken.RequestAuthToken(_tokenProvider, _client);

            var postResult = await _client.PostAsync("https://localhost:7192/Task", bodyContent);
			var postContent = await postResult.Content.ReadAsStringAsync();

			if(!postResult.IsSuccessStatusCode)
			{
				throw new ApplicationException(postContent);
			}
		}

        public async System.Threading.Tasks.Task DeleteProduct(int id)
        {
            var url = Path.Combine("https://localhost:7192/Task", id.ToString());

			var deleteResult = await _client.DeleteAsync(url);
			var deleteContent = await deleteResult.Content.ReadAsStringAsync();

            if (!deleteResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(deleteContent);
            }
        }

        public async Task<Models.Task> GetTaskById(string id)
        {
			var url = Path.Combine("https://localhost:7192/Task", id);

			var response = await _client.GetAsync(url);
			var content = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				throw new ApplicationException(content);
			}

			var task = JsonSerializer.Deserialize<Models.Task>(content, _options);
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

			if (taskParameters.OrderBy != string.Empty)
			{
				queryStringParam.Add("orderby", taskParameters.OrderBy);
			};

			await AddToken.RequestAuthToken(_tokenProvider, _client);

			var response = await _client.GetAsync(QueryHelpers.AddQueryString("https://localhost:7192/Task/extended", queryStringParam));

			var content = await response.Content.ReadAsStringAsync();
			
			if (!response.IsSuccessStatusCode) 
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

        public async System.Threading.Tasks.Task UpdateTask(Models.Task task)
        {
			var content = JsonSerializer.Serialize(task);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
			var url = Path.Combine("https://localhost:7192/Task", task.Id.ToString());

			var putResult = await _client.PutAsync(url, bodyContent);
			var putContent = await putResult.Content.ReadAsStringAsync();

			if(!putResult.IsSuccessStatusCode)
			{
				throw new ApplicationException(putContent);
			}
        }
        
        private async System.Threading.Tasks.Task RequestAuthToken()
		{
			var requestToken = await _tokenProvider.RequestAccessToken();
			requestToken.TryGetToken(out var token);
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
        }
    }

}
