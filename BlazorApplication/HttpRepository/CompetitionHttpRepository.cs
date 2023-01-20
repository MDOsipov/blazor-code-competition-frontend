using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.WebUtilities;
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
        private readonly Models.BackEndConnections _backEndConnections;
        private readonly IAccessTokenProvider _accessTokenProvider;

        public CompetitionHttpRepository(HttpClient client, IConfiguration configuration, IAccessTokenProvider accessTokenProvider)
        {
            _client = client;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _configuration = configuration;
            _backEndConnections = _configuration.GetSection("ConnectionStrings").Get<Models.BackEndConnections>();
			_accessTokenProvider = accessTokenProvider; 
		}

		public async System.Threading.Tasks.Task CreateCompetition(Competition competition)
        {
			var content = JsonSerializer.Serialize(competition);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

			await AddToken.RequestAuthToken(_accessTokenProvider, _client);
			var postResult = await _client.PostAsync(_backEndConnections.NodeJSUri + "competition", bodyContent);
			var postContent = await postResult.Content.ReadAsStringAsync();

			if (!postResult.IsSuccessStatusCode)
			{
				throw new ApplicationException(postContent);
			}
		}

		public async Task<IEnumerable<CompetitionStatus>> GetAllCompetitionStatuses()
		{
			await AddToken.RequestAuthToken(_accessTokenProvider, _client);

			var response = await _client.GetAsync(_backEndConnections.NodeJSUri + "competition/status/all");

			var content = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				throw new ApplicationException(content);
			}

            return JsonSerializer.Deserialize<List<CompetitionStatus>>(content, _options);
		}

        public async Task<Competition> GetCompetitionById(string id)
        {
            var url = Path.Combine(_backEndConnections.NodeJSUri + "competition", id);

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
                HashCode = localCompetition.hashCode
            };

            return competition;
        }

        public async Task<PagingResponse<Competition>> GetCompetitions(CompetitionParameters competitionParameters)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = competitionParameters.PageNumber.ToString(),
                ["switchOff"] = competitionParameters.switchOff ? "1" : "0"
            };

			await AddToken.RequestAuthToken(_accessTokenProvider, _client);
			var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.NodeJSUri + "competition/extended", queryStringParam));

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var pagingResponse = new PagingResponse<Models.Competition>
            {
                Items = JsonSerializer.Deserialize<List<Models.Competition>>(content, _options),
                MetaData = JsonSerializer.Deserialize<Models.MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
            };

            return pagingResponse;
        }

        public async System.Threading.Tasks.Task UpdateCompetition(Competition competition)
        {
            var content = JsonSerializer.Serialize(competition);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var url = Path.Combine(_backEndConnections.NodeJSUri + "competition", competition.id.ToString());

			await AddToken.RequestAuthToken(_accessTokenProvider, _client);
			var putResult = await _client.PutAsync(url, bodyContent);
            var putContent = await putResult.Content.ReadAsStringAsync();

            if (!putResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(putContent);
            }
        }

        public async System.Threading.Tasks.Task DeleteCompetition(int id)
        {
            var url = Path.Combine(_backEndConnections.NodeJSUri + "competition", id.ToString());

			await AddToken.RequestAuthToken(_accessTokenProvider, _client);
			var deleteResult = await _client.DeleteAsync(url);
            var deleteContent = await deleteResult.Content.ReadAsStringAsync();

            if (!deleteResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(deleteContent);
            }
        }
    }
}
