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
        private readonly ILogger<TaskCategoryHttpRepository> _logger;

        public TaskCategoryHttpRepository(IAccessTokenProvider accessTokenProvider, HttpClient client, IConfiguration configuration, ILogger<TaskCategoryHttpRepository> logger) 
        {
            _accessTokenProvider = accessTokenProvider;
            _client = client;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _configuration = configuration;
            _backEndConnections = _configuration.GetSection("ConnectionStrings").Get<Models.BackEndConnections>();
            _logger = logger;
        }

        public async System.Threading.Tasks.Task CreateTaskCategory(TaskCategory taskCategory)
        {
            _logger.LogInformation("Create task category http repository method is called");

            var content = JsonSerializer.Serialize(taskCategory);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            try
            {
                await AddToken.RequestAuthToken(_accessTokenProvider, _client);
                var postResult = await _client.PostAsync(_backEndConnections.CSharpUri + "TaskCategory", bodyContent);
                var postContent = await postResult.Content.ReadAsStringAsync();

                if (!postResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(postContent);
                }

                _logger.LogInformation($"Success. A new task category is created");
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while creating a new task category!", ex);
            }
        }

        public async System.Threading.Tasks.Task DeleteTaskCategory(int id)
        {
            _logger.LogInformation("Delete task category http repository method is called");

            var url = Path.Combine(_backEndConnections.CSharpUri + "TaskCategory", id.ToString());

            await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            try
            {
                var deleteResult = await _client.DeleteAsync(url);
                var deleteContent = await deleteResult.Content.ReadAsStringAsync();

                if (!deleteResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(deleteContent);
                }

                _logger.LogInformation($"Success. The task category is deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while deleting the task category!", ex);
            }
        }

        public async Task<PagingResponse<TaskCategory>> GetTaskCategory(TaskCategoryParameters taskCategoryParameters)
        {
            _logger.LogInformation("Get task category http repository method is called");

            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = taskCategoryParameters.PageNumber.ToString(),
                ["switchOffString"] = taskCategoryParameters.switchOff ? "1" : "0"
            };

            await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            try
            {
                var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.CSharpUri + "TaskCategory", queryStringParam));
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(content);
                };

            var pagingResponse = new PagingResponse<TaskCategory>
            {
                Items = JsonSerializer.Deserialize<List<TaskCategory>>(content, _options),
                MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
            };

                pagingResponse.SuccessRequest = true;
                _logger.LogInformation($"Success. Task category: {content}");
                return pagingResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting a list of task categories!", ex);
            }
        }

        public async Task<TaskCategory> GetTaskCategoryById(string id)
        {
            _logger.LogInformation("Get task category by id http repository method is called");

            var url = Path.Combine(_backEndConnections.CSharpUri + "TaskCategory", id);

            await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            try
            {
                var response = await _client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(content);
                }

                var taskCategory = JsonSerializer.Deserialize<TaskCategory>(content, _options);

                taskCategory.SuccessRequest = true;
                _logger.LogInformation($"Success. Task category: {content}");
                return taskCategory;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting a task category by id!", ex);
            }
        }

        public async System.Threading.Tasks.Task UpdateTaskCategory(TaskCategory taskCategory)
        {
            _logger.LogInformation("Update task category http repository method is called");

            var content = JsonSerializer.Serialize(taskCategory);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var url = Path.Combine(_backEndConnections.CSharpUri + "TaskCategory", taskCategory.Id.ToString());

            try
            {
                await AddToken.RequestAuthToken(_accessTokenProvider, _client);
                var putResult = await _client.PutAsync(url, bodyContent);
                var putContent = await putResult.Content.ReadAsStringAsync();

                if (!putResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(putContent);
                }

                _logger.LogInformation($"Success. The task category is updated");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while updating the task category!", ex);
            }
        }
    }
}
