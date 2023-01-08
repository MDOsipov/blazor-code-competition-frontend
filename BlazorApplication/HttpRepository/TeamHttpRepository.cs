using BlazorApplication.Features;
using BlazorApplication.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;

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

		public async Task<PagingResponse<Team>> GetTeams(TeamParameters teamParameters)
		{
			var queryStringParam = new Dictionary<string, string>
			{
				["pageNumber"] = teamParameters.PageNumber.ToString()
			};

			var response = await _client.GetAsync(QueryHelpers.AddQueryString("Team/extended", queryStringParam));
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
