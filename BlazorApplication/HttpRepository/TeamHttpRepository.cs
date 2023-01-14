using BlazorApplication.Features;
using BlazorApplication.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorApplication.HttpRepository
{
	public class TeamHttpRepository : ITeamHttpRepository
	{
		private readonly HttpClient _client;
		private readonly JsonSerializerOptions _options;

		public TeamHttpRepository(HttpClient client)
		{
			_client = client;
			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive= true };	
		}

        public async System.Threading.Tasks.Task CreateTeam(Team team)
        {
			var content = JsonSerializer.Serialize(team);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

			var postResult = await _client.PostAsync("https://localhost:7192/Team", bodyContent);
			var postContent = await postResult.Content.ReadAsStringAsync();

            if (!postResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(postContent);
            }
        }

        public async System.Threading.Tasks.Task DeleteTeam(int id)
        {
            var url = Path.Combine("https://localhost:7192/Team", id.ToString());

            var deleteResult = await _client.DeleteAsync(url);
            var deleteContent = await deleteResult.Content.ReadAsStringAsync();

            if (!deleteResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(deleteContent);
            }
        }

        public async Task<Team> GetTeamById(string id)
		{
			var url = Path.Combine("https://localhost:7192/Team", id);

			var response = await _client.GetAsync(url);
			var content = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				throw new ApplicationException(content);
			}

			var team = JsonSerializer.Deserialize<Team>(content, _options);
			return team;
		}

		public async Task<PagingResponse<Team>> GetTeams(TeamParameters teamParameters)
		{
			var queryStringParam = new Dictionary<string, string>
			{
				["pageNumber"] = teamParameters.PageNumber.ToString()
			};

			var response = await _client.GetAsync(QueryHelpers.AddQueryString("https://localhost:7192/Team/extended", queryStringParam));
			var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Response " + JsonSerializer.Serialize(response));
			Console.WriteLine("X-pagination: " + response.Headers.GetValues("X-Pagination").First());

            if (!response.IsSuccessStatusCode)
			{
				throw new ApplicationException(content);
			}

			var pagingResponse = new PagingResponse<Team>
			{
				Items = JsonSerializer.Deserialize<List<Team>>(content, _options),
				MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
			};

			return pagingResponse;
		}

		public async System.Threading.Tasks.Task UpdateTeam(Team team)
		{
			var content = JsonSerializer.Serialize(team);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
			var url = Path.Combine("https://localhost:7192/Team", team.Id.ToString());

			var putResult = await _client.PutAsync(url, bodyContent);
			var putContent = await putResult.Content.ReadAsStringAsync();

			if (!putResult.IsSuccessStatusCode)
			{
				throw new ApplicationException(putContent);
			}
		}
	}
}
