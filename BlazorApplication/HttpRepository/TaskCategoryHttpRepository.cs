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

        public async System.Threading.Tasks.Task CreateTaskCategory(TaskCategory taskCategory)
        {
            var content = JsonSerializer.Serialize(taskCategory);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            await AddToken.RequestAuthToken(_accessTokenProvider, _client);
            var postResult = await _client.PostAsync(_backEndConnections.CSharpUri + "TaskCategory", bodyContent);
            var postContent = await postResult.Content.ReadAsStringAsync();

            if (!postResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(postContent);
            }
        }

        public async System.Threading.Tasks.Task DeleteTaskCategory(int id)
        {
            var url = Path.Combine(_backEndConnections.CSharpUri + "TaskCategory", id.ToString());

            await AddToken.RequestAuthToken(_accessTokenProvider, _client);
            var deleteResult = await _client.DeleteAsync(url);
            var deleteContent = await deleteResult.Content.ReadAsStringAsync();

            if (!deleteResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(deleteContent);
            }
        }

        public async Task<PagingResponse<Models.TaskCategory>> GetTaskCategory(TaskCategoryParameters taskCategoryParameters)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = taskCategoryParameters.PageNumber.ToString(),
                ["switchOffString"] = taskCategoryParameters.switchOff ? "1" : "0"
            };

            await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.CSharpUri + "TaskCategory", queryStringParam));
            var content = await response.Content.ReadAsStringAsync();


            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            };

            var pagingResponse = new PagingResponse<Models.TaskCategory>
            {
                Items = JsonSerializer.Deserialize<List<Models.TaskCategory>>(content, _options),
                MetaData = JsonSerializer.Deserialize<Models.MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
            };

            return pagingResponse;
        }

        public async Task<TaskCategory> GetTaskCategoryById(string id)
        {
            var url = Path.Combine(_backEndConnections.CSharpUri + "TaskCategory", id);

            await AddToken.RequestAuthToken(_accessTokenProvider, _client);
            var response = await _client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var taskCategory = JsonSerializer.Deserialize<TaskCategory>(content, _options);

            Console.WriteLine("We got task category: " + JsonSerializer.Serialize(taskCategory));  

            return taskCategory;
        }

        public async System.Threading.Tasks.Task UpdateTaskCategory(TaskCategory taskCategory)
        {
            var content = JsonSerializer.Serialize(taskCategory);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var url = Path.Combine(_backEndConnections.CSharpUri + "TaskCategory", taskCategory.Id.ToString());

            await AddToken.RequestAuthToken(_accessTokenProvider, _client);
            var putResult = await _client.PutAsync(url, bodyContent);
            var putContent = await putResult.Content.ReadAsStringAsync();

            if (!putResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(putContent);
            }
        }
    }
}
