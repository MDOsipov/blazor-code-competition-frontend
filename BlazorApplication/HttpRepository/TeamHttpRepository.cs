using BlazorApplication.Features;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Json;

namespace BlazorApplication.HttpRepository
{
	public class TeamHttpRepository : ITeamHttpRepository
	{
        private readonly IAccessTokenProvider _tokenProvider;
        private readonly HttpClient _client;
		private readonly JsonSerializerOptions _options;

		public TeamHttpRepository(IAccessTokenProvider tokenProvider,HttpClient client)
		{
            _tokenProvider = tokenProvider;
            _client = client;
			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive= true };	
		}

        public async System.Threading.Tasks.Task CreateTeam(Team team)
        {
			var content = JsonSerializer.Serialize(team);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            await AddToken.RequestAuthToken(_tokenProvider, _client);

            var postResult = await _client.PostAsync("https://localhost:7192/Team", bodyContent);
			var postContent = await postResult.Content.ReadAsStringAsync();

            if (!postResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(postContent);
            }
        }

        public async Task<PagingResponse<Team>> GetTeams(TeamParameters teamParameters)
		{
			var queryStringParam = new Dictionary<string, string>
			{
				["pageNumber"] = teamParameters.PageNumber.ToString()
			};

            await AddToken.RequestAuthToken(_tokenProvider, _client);

            var response = await _client.GetAsync(QueryHelpers.AddQueryString("https://localhost:7192/Team/extended", queryStringParam));
			var content = await response.Content.ReadAsStringAsync();	

			if(!response.IsSuccessStatusCode)
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
	}
}
