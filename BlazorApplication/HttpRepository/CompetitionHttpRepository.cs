using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;

namespace BlazorApplication.HttpRepository
{
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

        public async Task<PagingResponse<Models.Competition>> GetCompetitions(CompetitionParameters competitionParameters)
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
    }
}
