using BlazorApplication.Features;
using BlazorApplication.Interfaces;
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
    }

    public class CompetitionHttpRepository : ICompetitionHttpRepository
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _options;
        private readonly IConfiguration _configuration;
        private readonly Models.BackEndConnections _backEndConnections;

        public CompetitionHttpRepository(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _configuration = configuration;
            _backEndConnections = _configuration.GetSection("ConnectionStrings").Get<Models.BackEndConnections>();
        }

		public async System.Threading.Tasks.Task CreateCompetition(Competition competition)
        {
			var content = JsonSerializer.Serialize(competition);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

			var postResult = await _client.PostAsync("http://localhost:6060/competition", bodyContent);
			var postContent = await postResult.Content.ReadAsStringAsync();

			if (!postResult.IsSuccessStatusCode)
			{
				throw new ApplicationException(postContent);
			}
		}

		public async Task<IEnumerable<CompetitionStatus>> GetAllCompetitionStatuses()
		{
			var response = await _client.GetAsync("http://localhost:6060/competition/status/all");

			var content = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				throw new ApplicationException(content);
			}

            return JsonSerializer.Deserialize<List<CompetitionStatus>>(content, _options);
		}

        public async Task<Competition> GetCompetitionById(string id)
        {
            var url = Path.Combine("http://localhost:6060/competition", id);

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
                CompetitionStatusId = localCompetition.competitionStatus
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
            var url = Path.Combine("http://localhost:6060/competition", competition.id.ToString());

            var putResult = await _client.PutAsync(url, bodyContent);
            var putContent = await putResult.Content.ReadAsStringAsync();

            if (!putResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(putContent);
            }
        }

        public async System.Threading.Tasks.Task DeleteCompetition(int id)
        {
            var url = Path.Combine("http://localhost:6060/competition", id.ToString());

            var deleteResult = await _client.DeleteAsync(url);
            var deleteContent = await deleteResult.Content.ReadAsStringAsync();

            if (!deleteResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(deleteContent);
            }
        }
    }
}
