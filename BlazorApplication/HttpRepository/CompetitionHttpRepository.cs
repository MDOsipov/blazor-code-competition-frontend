using BlazorApplication.Features;
using BlazorApplication.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace BlazorApplication.HttpRepository
{
    public class CompetitionHttpRepository : ICompetitionHttpRepository
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _options;

        public CompetitionHttpRepository(HttpClient client)
        {
            _client = client;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<PagingResponse<Competition>> GetCompetitions(CompetitionParameters competitionParameters)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = competitionParameters.PageNumber.ToString()
            };

            var response = await _client.GetAsync(QueryHelpers.AddQueryString("http://localhost:6060/competition/extended", queryStringParam));

            Console.WriteLine(2323232);

            Console.WriteLine("Response " + JsonSerializer.Serialize(response));

            Console.WriteLine("X-pagination: " + response.Headers.GetValues("X-Pagination").First());



            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Content " + content);


            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var pagingResponse = new PagingResponse<Competition>
            {
                Items = JsonSerializer.Deserialize<List<Competition>>(content, _options),
                MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
            };

            return pagingResponse;
        }
    }
}
