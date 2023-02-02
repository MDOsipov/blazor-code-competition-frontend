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
        private readonly ILogger<TeamHttpRepository> _logger;

        public TeamHttpRepository(IAccessTokenProvider tokenProvider,HttpClient client, IConfiguration configuration, ILogger<TeamHttpRepository> logger)
		{
            _tokenProvider = tokenProvider;
            _client = client;
			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive= true };
			_configuration = configuration;
			_backEndConnections = _configuration.GetSection("ConnectionStrings").Get<Models.BackEndConnections>();
            _logger = logger;
		}

        public async Task CreateTeam(Models.Team team)
        {
            _logger.LogInformation("Create team http repository method is called");

            var content = JsonSerializer.Serialize(team);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");            
			
			try
			{
				await AddToken.RequestAuthToken(_tokenProvider, _client);

				var postResult = await _client.PostAsync(_backEndConnections.CSharpUri + "Team", bodyContent);
                var postContent = await postResult.Content.ReadAsStringAsync();

                if (!postResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(postContent);
                }

                _logger.LogInformation($"Success. A new team is created");
            }
            catch (Exception ex) 
			{
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while creating a new team!", ex);
            }
        }

        public async Task DeleteTeam(int id)
        {
            _logger.LogInformation("Delete team http repository method is called");

            var url = Path.Combine(_backEndConnections.CSharpUri + "Team", id.ToString());

            try 
            { 
				await AddToken.RequestAuthToken(_tokenProvider, _client);

			    var deleteResult = await _client.DeleteAsync(url);
                var deleteContent = await deleteResult.Content.ReadAsStringAsync();

                if (!deleteResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(deleteContent);
                }

                _logger.LogInformation($"Success. The team is deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while deleting the team!", ex);
            }
        }

        public async Task<Models.Team> GetTeamById(string id)
		{
            _logger.LogInformation("Get team by id http repository method is called");

            var url = Path.Combine(_backEndConnections.CSharpUri + "Team", id);            

			try
			{
				await AddToken.RequestAuthToken(_tokenProvider, _client);

				var response = await _client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(content);
                }

                var team = JsonSerializer.Deserialize<Models.Team>(content, _options);

                _logger.LogInformation($"Success. Team: {content}");

                return team;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a team by id!", ex);
            }
        }

		public async Task<PagingResponse<Models.Team>> GetTeams(TeamParameters teamParameters)
		{
            _logger.LogInformation("Get teams http repository method is called");

            var queryStringParam = new Dictionary<string, string>
			{
				["pageNumber"] = teamParameters.PageNumber.ToString(),
				["switchOffString"] = teamParameters.switchOff ? "1" : "0"
			};

			try
			{
				await AddToken.RequestAuthToken(_tokenProvider, _client);

				var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.CSharpUri + "Team/extended", queryStringParam));
                var content = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(content);
                }

                var pagingResponse = new PagingResponse<Models.Team>
                {
                    Items = JsonSerializer.Deserialize<List<Models.Team>>(content, _options),
                    MetaData = JsonSerializer.Deserialize<Models.MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
                };

                _logger.LogInformation($"Success. Teams: {content}");

                return pagingResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a list of teams!", ex);
            }
        }

		public async Task<PagingResponse<Models.Team>> GetTeamsByCompetitionId(TeamParameters teamParameters, string competitionId)
		{
			_logger.LogInformation($"Get teams by competition id with team parameters {teamParameters} and competition id {competitionId} http repository method is called");

			var queryStringParam = new Dictionary<string, string>
			{
				["pageNumber"] = teamParameters.PageNumber.ToString(),
				["switchOffString"] = teamParameters.switchOff ? "1" : "0"
			};

			try
			{
				await AddToken.RequestAuthToken(_tokenProvider, _client);

				var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.CSharpUri + "Team/extended/byCompetitionId/" + competitionId, queryStringParam));
				var content = await response.Content.ReadAsStringAsync();

				if (!response.IsSuccessStatusCode)
				{
					throw new ApplicationException(content);
				}

				var pagingResponse = new PagingResponse<Models.Team>
				{
					Items = JsonSerializer.Deserialize<List<Models.Team>>(content, _options),
					MetaData = JsonSerializer.Deserialize<Models.MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
				};

				_logger.LogInformation($"Success. Teams: {content}");

				return pagingResponse;
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error: {ex}");
				throw new Exception("Oops! Something went wrong while getting a list of teams by competition id!", ex);
			}
		}

		public async Task<PagingResponse<Models.Team>> GetTeamsLimited(TeamParameters teamParameters)
		{
            _logger.LogInformation("Get teams limited http repository method is called");

            var queryStringParam = new Dictionary<string, string>
			{
				["pageNumber"] = teamParameters.PageNumber.ToString(),
				["switchOffString"] = teamParameters.switchOff ? "1" : "0"
			};			

            try
            {
				await AddToken.RequestAuthToken(_tokenProvider, _client);

				var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.CSharpUri + "Team", queryStringParam));
                var content = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(content);
                }

                var pagingResponse = new PagingResponse<Models.Team>
                {
                    Items = JsonSerializer.Deserialize<List<Models.Team>>(content, _options),
                    MetaData = JsonSerializer.Deserialize<Models.MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
                };

                _logger.LogInformation($"Success. Teams: {content}");

                return pagingResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a list of teams (with limited info)!", ex);
            }
        }

		public async Task UpdateTeam(Models.Team team)
		{
            _logger.LogInformation("Update team http repository method is called");

            var content = JsonSerializer.Serialize(team);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
			var url = Path.Combine(_backEndConnections.CSharpUri + "Team", team.Id.ToString());

            try 
            {
				await AddToken.RequestAuthToken(_tokenProvider, _client);

				var putResult = await _client.PutAsync(url, bodyContent);
                var putContent = await putResult.Content.ReadAsStringAsync();

                if (!putResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(putContent);
                }

                _logger.LogInformation($"Success. The team is updated");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while updating the team!", ex);
            }
        }

        public async Task<string> UploadTeamImage(MultipartFormDataContent content)
        {
            _logger.LogInformation("Upload team image http repository method is called");

            try
            {
				await AddToken.RequestAuthToken(_tokenProvider, _client);

				var postResult = await _client.PostAsync(_backEndConnections.CSharpUri + "api/upload", content);
                var postContent = await postResult.Content.ReadAsStringAsync();


                if (!postResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(postContent);
                }
                else
                {
                    var imgUrl = Path.Combine(_backEndConnections.CSharpUri, postContent);
                    _logger.LogInformation($"Success. An image is uploaded");
                    return imgUrl;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while uploading an image to the team!", ex);
            }
        }
    }
}
