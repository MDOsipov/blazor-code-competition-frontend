using BlazorApplication.Features;
using BlazorApplication.Models;
using BlazorApplication.Pages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Text.Json;
using BlazorApplication.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.HttpRepository
{
    class LocalParticipant
    {
        public int id { get; set; } = 0;
        public string firstName { get; set; } = "";
        public string lastName { get; set; } = "";
        public string email { get; set; }

        public int userId { get; set; } = 0;
        public int teamId { get; set; } = 0;
    }


    public class ParticipantHttpRepository : IParticipantHttpRepository
	{
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly HttpClient _client;
		private readonly JsonSerializerOptions _options;
        private readonly IConfiguration _configuration;
        private readonly BackEndConnections _backEndConnections;
        private readonly ILogger<ParticipantHttpRepository> _logger;
        public ParticipantHttpRepository(IAccessTokenProvider accessTokenProvider, HttpClient client, IConfiguration configuration, ILogger<ParticipantHttpRepository> logger)
		{
            _accessTokenProvider = accessTokenProvider;
            _client = client;
			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _configuration = configuration;
			_backEndConnections = _configuration.GetSection("ConnectionStrings").Get<BackEndConnections>();
            _logger = logger;
		}

		public async System.Threading.Tasks.Task AddTeamToParticipant(string teamId, string participantId)
		{
            _logger.LogInformation("Add team to participant http repository method is called");

            var queryStringParam = new Dictionary<string, string>
			{
				["teamId"] = teamId,
				["participantId"] = participantId
			};

			await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            try
            {
                var bodyContent = new StringContent("", Encoding.UTF8, "application/json");
                var putResult = await _client.PutAsync(QueryHelpers.AddQueryString(_backEndConnections.NodeJSUri + "participant", queryStringParam), bodyContent);
                var putContent = await putResult.Content.ReadAsStringAsync();

                if (!putResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(putContent);
                }

                _logger.LogInformation($"Success. A new team is added to the competition");
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while adding a new team to the participant!", ex);
            }
        }

		public async System.Threading.Tasks.Task CreateParticipant(Participant participant)
        {
            _logger.LogInformation("Create participant http repository method is called");

            var content = JsonSerializer.Serialize(participant);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

			await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            try
            {
                var postResult = await _client.PostAsync(_backEndConnections.NodeJSUri + "participant", bodyContent);
                var postContent = await postResult.Content.ReadAsStringAsync();

                if (!postResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(postContent);
                }

                _logger.LogInformation($"Success. A new participant is created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while creating a new participant!", ex);
            }
        }

        public async System.Threading.Tasks.Task DeleteParticipant(int id)
        {
            _logger.LogInformation("Delete participant http repository method is called");

            var url = Path.Combine(_backEndConnections.NodeJSUri + "participant", id.ToString());

			await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            try
            {
                var deleteResult = await _client.DeleteAsync(url);
                var deleteContent = await deleteResult.Content.ReadAsStringAsync();

                if (!deleteResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(deleteContent);
                }

                _logger.LogInformation($"Success. The participant is deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while deleting the participant!", ex);
            }
        }

        public async Task<Participant> GetParticipantById(string id)
        {
            _logger.LogInformation("Get participant by id http repository method is called");

            var url = Path.Combine(_backEndConnections.NodeJSUri + "participant", id);

			await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            try
            {
                var response = await _client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(content);
                }

                var localParticipant = JsonSerializer.Deserialize<LocalParticipant>(content, _options);

                var participant = new Participant
                {
                    id = localParticipant.id,
                    firstName = localParticipant.firstName,
                    lastName = localParticipant.lastName,
                    email = localParticipant.email,
                    userId = localParticipant.userId,
                    teamId = localParticipant.teamId,
                    SuccessRequest = true,
                };

                _logger.LogInformation($"Success. Participant: {JsonSerializer.Serialize(participant)}");
                return participant;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a participant by id!", ex);
            }
        }

        public async Task<PagingResponse<Participant>> GetParticipants(ParticipantParameters participantParameters)
		{
            _logger.LogInformation("Get participants http repository method is called");
            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = participantParameters.PageNumber.ToString(),
                ["switchOff"] = participantParameters.switchOff ? "1" : "0"
            };

            await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            try
            {
                var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.NodeJSUri + "participant/extended", queryStringParam));
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(content);
                }

                var pagingResponse = new PagingResponse<Participant>
                {
                    Items = JsonSerializer.Deserialize<List<Participant>>(content, _options),
                    MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
                };

                pagingResponse.SuccessRequest = true;
                _logger.LogInformation($"Success. Participants: {content}");
                return pagingResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a list of participants!", ex);
            }
        }

        public async Task<PagingResponse<Participant>> GetParticipantsByEmail(ParticipantParameters participantParameters, string email)
        {
            _logger.LogInformation("Get participants by email http repository method is called");

            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = participantParameters.PageNumber.ToString(),
                ["switchOff"] = participantParameters.switchOff ? "1" : "0"
            };

            await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            try
            {
                var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.NodeJSUri + "participant/byEmail/" + email, queryStringParam));
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(content);
                }

                var pagingResponse = new PagingResponse<Participant>
                {
                    Items = JsonSerializer.Deserialize<List<Participant>>(content, _options),
                    MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
                };

                pagingResponse.SuccessRequest = true;
                _logger.LogInformation($"Success. Participants: {content}");
                return pagingResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a list of participants by email!", ex);
            }
        }

        public async Task<PagingResponse<Participant>> GetParticipantsByTeamId(ParticipantParameters participantParameters, string teamId)
        {
            _logger.LogInformation("Get participants by team id http repository method is called");
            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = participantParameters.PageNumber.ToString(),
                ["switchOff"] = participantParameters.switchOff ? "1" : "0"
            };

            await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            try
            {
                var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.NodeJSUri + "participant/byTeamId/" + teamId, queryStringParam));
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(content);
                }

                var pagingResponse = new PagingResponse<Participant>
                {
                    Items = JsonSerializer.Deserialize<List<Participant>>(content, _options),
                    MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
                };

                pagingResponse.SuccessRequest = true;
                _logger.LogInformation($"Success. Participants: {content}");
                return pagingResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a list of participants by team id!", ex);
            }
        }

        public async Task<ResponseWithSuccess<Participant>> GetParticipantsLimited()
        {
            _logger.LogInformation("Get participants limited http repository method is called");
            await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            try
            {
                var response = await _client.GetAsync(_backEndConnections.NodeJSUri + "participant");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(content);
                }

                _logger.LogInformation($"Success. Participants: {content}");

                var responseWithStatues = new ResponseWithSuccess<Participant>
                {
                    Items = JsonSerializer.Deserialize<List<Participant>>(content, _options),
                    SuccessRequest = true,
                };

                return responseWithStatues;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a list (without wide info) of participants by team id!", ex);
            }
        }

        public async System.Threading.Tasks.Task RemoveTeamFromParticipant(string participantId)
        {
            _logger.LogInformation("Remove team from participant http repository method is called");
            var queryStringParam = new Dictionary<string, string>
            {
                ["participantId"] = participantId
            };

            await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            try
            {
                var bodyContent = new StringContent("", Encoding.UTF8, "application/json");
                var putResult = await _client.PutAsync(QueryHelpers.AddQueryString(_backEndConnections.NodeJSUri + "participant/remove/teamFromParticipant", queryStringParam), bodyContent);
                var putContent = await putResult.Content.ReadAsStringAsync();

                if (!putResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(putContent);
                }

                _logger.LogInformation($"Success. A team from the participant is removed");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while removing team from participant!", ex);
            }
        }

        public async System.Threading.Tasks.Task UpdateParticipant(Participant participant)
        {
            _logger.LogInformation("Update participant http repository method is called");

            var content = JsonSerializer.Serialize(participant);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var url = Path.Combine(_backEndConnections.NodeJSUri + "participant", participant.id.ToString());

			await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            try
            {
                var putResult = await _client.PutAsync(url, bodyContent);
                var putContent = await putResult.Content.ReadAsStringAsync();

                if (!putResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(putContent);
                }

                _logger.LogInformation($"Success. The participant is updated");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while updating the participant!", ex);
            }
        }
    }
}
