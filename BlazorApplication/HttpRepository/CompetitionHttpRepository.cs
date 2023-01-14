using BlazorApplication.Models;
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

        public async Task<ICollection<Competition>> GetCompetitions()
        {
            var response = await _client.GetAsync("http://localhost:6061/competition/extended");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            return JsonSerializer.Deserialize<List<Competition>>(content, _options);
        }
    }
}
