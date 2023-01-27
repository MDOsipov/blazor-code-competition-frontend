using BlazorApplication.Features;
using BlazorApplication.Models;
using BlazorApplication.Pages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Text.Json;
using BlazorApplication.Interfaces;

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

		public ParticipantHttpRepository(IAccessTokenProvider accessTokenProvider,HttpClient client, IConfiguration configuration)
		{
            _accessTokenProvider = accessTokenProvider;
            _client = client;
			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _configuration = configuration;
			_backEndConnections = _configuration.GetSection("ConnectionStrings").Get<BackEndConnections>();
		}

		public async System.Threading.Tasks.Task AddTeamToParticipant(string teamId, string participantId)
		{
			var queryStringParam = new Dictionary<string, string>
			{
				["teamId"] = teamId,
				["participantId"] = participantId
			};

			await AddToken.RequestAuthToken(_accessTokenProvider, _client);

			var bodyContent = new StringContent("", Encoding.UTF8, "application/json");
			var putResult = await _client.PutAsync(QueryHelpers.AddQueryString(_backEndConnections.NodeJSUri + "participant", queryStringParam), bodyContent);
			var putContent = await putResult.Content.ReadAsStringAsync();

			if (!putResult.IsSuccessStatusCode)
			{
				throw new ApplicationException(putContent);
			}
		}

		public async System.Threading.Tasks.Task CreateParticipant(Participant participant)
        {
            var content = JsonSerializer.Serialize(participant);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

			await AddToken.RequestAuthToken(_accessTokenProvider, _client);
			var postResult = await _client.PostAsync(_backEndConnections.NodeJSUri + "participant", bodyContent);
            var postContent = await postResult.Content.ReadAsStringAsync();

            if (!postResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(postContent);
            }
        }

        public async System.Threading.Tasks.Task DeleteParticipant(int id)
        {
            var url = Path.Combine(_backEndConnections.NodeJSUri + "participant", id.ToString());

			await AddToken.RequestAuthToken(_accessTokenProvider, _client);
			var deleteResult = await _client.DeleteAsync(url);
            var deleteContent = await deleteResult.Content.ReadAsStringAsync();

            if (!deleteResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(deleteContent);
            }
        }

        public async Task<Participant> GetParticipantById(string id)
        {
            var url = Path.Combine(_backEndConnections.NodeJSUri + "participant", id);

			await AddToken.RequestAuthToken(_accessTokenProvider, _client);
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
                teamId = localParticipant.teamId
            };

            return participant;
        }

        public async Task<PagingResponse<Participant>> GetParticipants(ParticipantParameters participantParameters)
		{
            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = participantParameters.PageNumber.ToString(),
                ["switchOff"] = participantParameters.switchOff ? "1" : "0"
            };

            await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.NodeJSUri + "participant/extended", queryStringParam));			
			var content = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				throw new ApplicationException(content);
			}

            var pagingResponse = new PagingResponse<Models.Participant>
            {
                Items = JsonSerializer.Deserialize<List<Models.Participant>>(content, _options),
                MetaData = JsonSerializer.Deserialize<Models.MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
            };

            return pagingResponse;

        }

        public async Task<PagingResponse<Participant>> GetParticipantsByEmail(ParticipantParameters participantParameters, string email)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = participantParameters.PageNumber.ToString(),
                ["switchOff"] = participantParameters.switchOff ? "1" : "0"
            };

            await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.NodeJSUri + "participant/byEmail/"+email, queryStringParam));
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var pagingResponse = new PagingResponse<Models.Participant>
            {
                Items = JsonSerializer.Deserialize<List<Models.Participant>>(content, _options),
                MetaData = JsonSerializer.Deserialize<Models.MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
            };

            return pagingResponse;
        }

        public async Task<PagingResponse<Participant>> GetParticipantsByTeamId(ParticipantParameters participantParameters, string teamId)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = participantParameters.PageNumber.ToString(),
                ["switchOff"] = participantParameters.switchOff ? "1" : "0"
            };

            await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            var response = await _client.GetAsync(QueryHelpers.AddQueryString(_backEndConnections.NodeJSUri + "participant/byTeamId/" + teamId, queryStringParam));
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var pagingResponse = new PagingResponse<Models.Participant>
            {
                Items = JsonSerializer.Deserialize<List<Models.Participant>>(content, _options),
                MetaData = JsonSerializer.Deserialize<Models.MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
            };

            return pagingResponse;
        }

        public async Task<IEnumerable<Participant>> GetParticipantsLimited()
        {
           
            await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            var response = await _client.GetAsync(_backEndConnections.NodeJSUri + "participant");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var toReturn = JsonSerializer.Deserialize<List<Models.Participant>>(content, _options);
            return toReturn;
        }

        public async System.Threading.Tasks.Task RemoveTeamFromParticipant(string participantId)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["participantId"] = participantId
            };

            await AddToken.RequestAuthToken(_accessTokenProvider, _client);

            var bodyContent = new StringContent("", Encoding.UTF8, "application/json");
            var putResult = await _client.PutAsync(QueryHelpers.AddQueryString(_backEndConnections.NodeJSUri + "participant/remove/teamFromParticipant", queryStringParam), bodyContent);
            var putContent = await putResult.Content.ReadAsStringAsync();

            if (!putResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(putContent);
            }
        }

        public async System.Threading.Tasks.Task UpdateParticipant(Participant participant)
        {
            var content = JsonSerializer.Serialize(participant);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var url = Path.Combine(_backEndConnections.NodeJSUri + "participant", participant.id.ToString());

			await AddToken.RequestAuthToken(_accessTokenProvider, _client);
			var putResult = await _client.PutAsync(url, bodyContent);
            var putContent = await putResult.Content.ReadAsStringAsync();

            if (!putResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(putContent);
            }
        }
    }
}
