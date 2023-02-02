using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Pages;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorApplication.HttpRepository
{
    class LocalCompetition
    {
        public int id { get; set; } = 0;
        public string competitionName { get; set; } = "";
        public int maxTaskPerGroups { get; set; } = 0;
        public int competitionStatus { get; set; } = 0;
        public string competitionAdministratorId { get; set; } = "";
        public string hashCode { get; set; } = "";

    }

    public class CompetitionHttpRepository : ICompetitionHttpRepository
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _options;
        private readonly IConfiguration _configuration;
        private readonly BackEndConnections _backEndConnections;
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly ILogger<CompetitionHttpRepository> _logger;

        public CompetitionHttpRepository(HttpClient client, IConfiguration configuration, IAccessTokenProvider accessTokenProvider, ILogger<CompetitionHttpRepository> logger)
        {
            _client = client;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _configuration = configuration;
            _backEndConnections = _configuration.GetSection("ConnectionStrings").Get<BackEndConnections>();
			_accessTokenProvider = accessTokenProvider;
            _logger = logger;
		}

		public async System.Threading.Tasks.Task CreateCompetition(Competition competition)
        {
            _logger.LogInformation("Get competition http repository method is called");
            var content = JsonSerializer.Serialize(competition);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");			

            try 
            {
				await AddToken.RequestAuthToken(_accessTokenProvider, _client);

				var postResult = await _client.PostAsync(_backEndConnections.NodeJSUri + "competition", bodyContent);
                var postContent = await postResult.Content.ReadAsStringAsync();

                if (!postResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(postContent);
                }

                _logger.LogInformation($"Success. Competition is created");
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while creating a new competition!", ex);
            }
        }

		public async Task<List<CompetitionStatus>> GetAllCompetitionStatuses()
		{
            _logger.LogInformation("Get all competition statuses http repository method is called");            

            try
            {
				await AddToken.RequestAuthToken(_accessTokenProvider, _client);

				var response = await _client.GetAsync(_backEndConnections.NodeJSUri + "competition/status/all");

                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(content);
                }

                _logger.LogInformation($"Success. Competition statuses: {content}");

				return JsonSerializer.Deserialize<List<CompetitionStatus>>(content, _options); ;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a list of competition statuses!", ex);
            }
        }

        public async Task<Competition> GetCompetitionById(string id)
        {
            _logger.LogInformation("Get competition by id http repository method is called");

            var url = Path.Combine(_backEndConnections.NodeJSUri + "competition", id);			

            try
            {
				await AddToken.RequestAuthToken(_accessTokenProvider, _client);

				var response = await _client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(content);
                }

                var localCompetition = JsonSerializer.Deserialize<LocalCompetition>(content, _options);
                var competition = new Competition
                {
                    id = localCompetition.id,
                    CompetitionName = localCompetition.competitionName,
                    maxTaskPerGroups = localCompetition.maxTaskPerGroups,
                    CompetitionStatusId = localCompetition.competitionStatus,
                    CompetitionAdministratorId = localCompetition.competitionAdministratorId,
                    HashCode = localCompetition.hashCode,
                };

                _logger.LogInformation($"Success. Competition: {JsonSerializer.Serialize(competition)}");
                return competition;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a list of competitions by id!", ex);
            }
        }

        public async Task<PagingResponse<Competition>> GetCompetitions(CompetitionParameters competitionParameters)
        {
            _logger.LogInformation("Get competitions http repository method is called");

            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = competitionParameters.PageNumber.ToString(),
                ["switchOff"] = competitionParameters.switchOff ? "1" : "0"
            };			

            try
            {
				await AddToken.RequestAuthToken(_accessTokenProvider, _client);

				var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.NodeJSUri + "competition/extended", queryStringParam));

                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(content);
                }

                var pagingResponse = new PagingResponse<Competition>
                {
                    Items = JsonSerializer.Deserialize<List<Competition>>(content, _options),
                    MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
                };

                _logger.LogInformation($"Success. Competitions: {content}");

                return pagingResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a list of competitions!", ex);
            }
        }

        public async System.Threading.Tasks.Task UpdateCompetition(Competition competition)
        {
            _logger.LogInformation("Update competition http repository method is called");

            var content = JsonSerializer.Serialize(competition);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var url = Path.Combine(_backEndConnections.NodeJSUri + "competition", competition.id.ToString());			

            try
            {
				await AddToken.RequestAuthToken(_accessTokenProvider, _client);

				var putResult = await _client.PutAsync(url, bodyContent);
                var putContent = await putResult.Content.ReadAsStringAsync();

                if (!putResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(putContent);
                }

                _logger.LogInformation($"Success. Competition is updated");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while updating the competition!", ex);
            }
        }

        public async System.Threading.Tasks.Task DeleteCompetition(int id)
        {
            _logger.LogInformation("Delete competition http repository method is called");

            var url = Path.Combine(_backEndConnections.NodeJSUri + "competition", id.ToString());			

            try
            {
				await AddToken.RequestAuthToken(_accessTokenProvider, _client);

				var deleteResult = await _client.DeleteAsync(url);
                var deleteContent = await deleteResult.Content.ReadAsStringAsync();

                if (!deleteResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(deleteContent);
                }

                _logger.LogInformation($"Success. Competition is deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while deleting the competition!", ex);
            }
        }

		public async Task<PagingResponse<Competition>> GetCompetitionsByAdminId(string adminId, CompetitionParameters competitionParameters)
		{
            _logger.LogInformation("Get competition by admin id http repository method is called");

            var queryStringParam = new Dictionary<string, string>
			{
				["pageNumber"] = competitionParameters.PageNumber.ToString(),
				["switchOff"] = competitionParameters.switchOff ? "1" : "0"
			};			

            try
            {
				await AddToken.RequestAuthToken(_accessTokenProvider, _client);

				var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.NodeJSUri + "competition/byAdmin/" + adminId, queryStringParam));

                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(content);
                }

                var pagingResponse = new PagingResponse<Competition>
                {
                    Items = JsonSerializer.Deserialize<List<Competition>>(content, _options),
                    MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
                };

                _logger.LogInformation($"Success. Competitions: {content}");
                return pagingResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting the competitions by admin id!", ex);
            }
        }

		public async Task<PagingResponse<Competition>> GetRunningCompetitionsByAdminId(string adminId, CompetitionParameters competitionParameters)
        {
			_logger.LogInformation("Get running competitions by admin id http repository method is called");

			var queryStringParam = new Dictionary<string, string>
			{
				["pageNumber"] = competitionParameters.PageNumber.ToString(),
				["switchOff"] = competitionParameters.switchOff ? "1" : "0"
			};

			try
			{
				await AddToken.RequestAuthToken(_accessTokenProvider, _client);

				var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.NodeJSUri + "competition/running/byAdmin/" + adminId, queryStringParam));

				var content = await response.Content.ReadAsStringAsync();

				if (!response.IsSuccessStatusCode)
				{
					throw new ApplicationException(content);
				}

				var pagingResponse = new PagingResponse<Competition>
				{
					Items = JsonSerializer.Deserialize<List<Competition>>(content, _options),
					MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
				};

				_logger.LogInformation($"Success. Competitions: {content}");
				return pagingResponse;
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error: {ex}");
				throw new Exception("Oops! Something went wrong while getting running competitions by admin id!", ex);
			}
		}
	}
}
