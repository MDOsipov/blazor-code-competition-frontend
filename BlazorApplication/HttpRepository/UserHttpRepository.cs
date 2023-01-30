using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using System.Net.NetworkInformation;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;

namespace BlazorApplication.HttpRepository
{
	public class UserHttpRepository : IUserHttpRepository
	{
		private readonly HttpClient _client;
		private readonly IConfiguration _configuration;
		private readonly Models.BackEndConnections _backEndConnections;
		private readonly JsonSerializerOptions _options;
        private readonly ILogger<UserHttpRepository> _logger;

        public UserHttpRepository(HttpClient client, IConfiguration configuration, ILogger<UserHttpRepository> logger)
		{
			_client = client;
			_configuration = configuration;
			_backEndConnections = _configuration.GetSection("ConnectionStrings").Get<Models.BackEndConnections>();
			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive= true };
			_logger = logger;
		}

		public async Task<PagingResponse<UserDto>> GetUsersExtended(UserParameters userParameters)
		{
            _logger.LogInformation("Get users extended http repository method is called");

            var queryStringParam = new Dictionary<string, string>
			{
				["pageNumber"] = userParameters.PageNumber.ToString(),
				["switchOffString"] = userParameters.switchOff ? "1" : "0"
			};

			try
			{
                var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.CSharpUri + "user/withRoles", queryStringParam));
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(content);
                }

                var pagingResponse = new PagingResponse<UserDto>
                {
                    Items = JsonSerializer.Deserialize<List<UserDto>>(content, _options),
                    MetaData = JsonSerializer.Deserialize<Models.MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
                };

                _logger.LogInformation($"Success. Users: {content}");

                return pagingResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting a list of users!", ex);
            }
        }
	}
}
