using BlazorApplication.Features;
using BlazorApplication.Interfaces;
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
		private readonly IConfiguration _configuration;
		private readonly Models.BackEndConnections _backEndConnections;

		public TeamHttpRepository(IAccessTokenProvider tokenProvider,HttpClient client, IConfiguration configuration)
		{
            _tokenProvider = tokenProvider;
            _client = client;
			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive= true };
			_configuration = configuration;
			_backEndConnections = _configuration.GetSection("ConnectionStrings").Get<Models.BackEndConnections>();
		}

        public async Task CreateTeam(Models.Team team)
        {
			var content = JsonSerializer.Serialize(team);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            await AddToken.RequestAuthToken(_tokenProvider, _client);

			Console.WriteLine("Team send: ");
            Console.WriteLine(JsonSerializer.Serialize(team));


            var postResult = await _client.PostAsync(_backEndConnections.CSharpUri + "Team", bodyContent);
			var postContent = await postResult.Content.ReadAsStringAsync();

            if (!postResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(postContent);
            }
        }

        public async Task DeleteTeam(int id)
        {
            var url = Path.Combine(_backEndConnections.CSharpUri + "Team", id.ToString());

            await AddToken.RequestAuthToken(_tokenProvider, _client);

            var deleteResult = await _client.DeleteAsync(url);
            var deleteContent = await deleteResult.Content.ReadAsStringAsync();

            if (!deleteResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(deleteContent);
            }
        }

        public async Task<Models.Team> GetTeamById(string id)
		{
			var url = Path.Combine(_backEndConnections.CSharpUri + "Team", id);

            await AddToken.RequestAuthToken(_tokenProvider, _client);

            var response = await _client.GetAsync(url);
			var content = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				throw new ApplicationException(content);
			}

			var team = JsonSerializer.Deserialize<Models.Team>(content, _options);

			team.SuccessRequest = true;

			return team;
		}

		public async Task<PagingResponse<Models.Team>> GetTeams(TeamParameters teamParameters)
		{
			Console.WriteLine(JsonSerializer.Serialize(teamParameters));
            Console.WriteLine("Switch off: " + (teamParameters.switchOff ? "1" : "0"));

            var queryStringParam = new Dictionary<string, string>
			{
				["pageNumber"] = teamParameters.PageNumber.ToString(),
				["switchOffString"] = teamParameters.switchOff ? "1" : "0"
			};

            await AddToken.RequestAuthToken(_tokenProvider, _client);

            var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.CSharpUri + "Team/extended", queryStringParam));
			var content = await response.Content.ReadAsStringAsync();	

            Console.WriteLine("Response " + JsonSerializer.Serialize(response));
			Console.WriteLine("X-pagination: " + response.Headers.GetValues("X-Pagination").First());

            if (!response.IsSuccessStatusCode)
			{
				throw new ApplicationException(content);
			}


			var pagingResponse = new PagingResponse<Models.Team>
			{
				Items = JsonSerializer.Deserialize<List<Models.Team>>(content, _options),
				MetaData = JsonSerializer.Deserialize<Models.MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
			};

			//Console.WriteLine("Content:\n " + JsonSerializer.Serialize(pagingResponse.Items));

			pagingResponse.SuccessRequest = true;

			return pagingResponse;
		}

		public async Task<PagingResponse<Models.Team>> GetTeamsLimited(TeamParameters teamParameters)
		{
			Console.WriteLine(JsonSerializer.Serialize(teamParameters));
			Console.WriteLine("Switch off: " + (teamParameters.switchOff ? "1" : "0"));

			var queryStringParam = new Dictionary<string, string>
			{
				["pageNumber"] = teamParameters.PageNumber.ToString(),
				["switchOffString"] = teamParameters.switchOff ? "1" : "0"
			};

			await AddToken.RequestAuthToken(_tokenProvider, _client);

			var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.CSharpUri + "Team", queryStringParam));
			var content = await response.Content.ReadAsStringAsync();

			Console.WriteLine("Response " + JsonSerializer.Serialize(response));
			Console.WriteLine("X-pagination: " + response.Headers.GetValues("X-Pagination").First());

			if (!response.IsSuccessStatusCode)
			{
				throw new ApplicationException(content);
			}


			var pagingResponse = new PagingResponse<Models.Team>
			{
				Items = JsonSerializer.Deserialize<List<Models.Team>>(content, _options),
				MetaData = JsonSerializer.Deserialize<Models.MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
			};

			//Console.WriteLine("Content:\n " + JsonSerializer.Serialize(pagingResponse.Items));

			pagingResponse.SuccessRequest = true;

			return pagingResponse;
		}

		public async Task UpdateTeam(Models.Team team)
		{
			var content = JsonSerializer.Serialize(team);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
			var url = Path.Combine(_backEndConnections.CSharpUri + "Team", team.Id.ToString());

            await AddToken.RequestAuthToken(_tokenProvider, _client);

            var putResult = await _client.PutAsync(url, bodyContent);
			var putContent = await putResult.Content.ReadAsStringAsync();

			if (!putResult.IsSuccessStatusCode)
			{
				throw new ApplicationException(putContent);
			}
		}

        public async Task<string> UploadTeamImage(MultipartFormDataContent content)
        {
            var postResult = await _client.PostAsync(_backEndConnections.CSharpUri + "api/upload", content);
            var postContent = await postResult.Content.ReadAsStringAsync();


            if (!postResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(postContent);
            }
            else
            {
                var imgUrl = Path.Combine(_backEndConnections.CSharpUri, postContent);
                return imgUrl;
            }
        }
    }
}
