using BlazorApplication.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Text.Json;
using BlazorApplication.Interfaces;


namespace BlazorApplication.HttpRepository
{
    public class ParticipantHttpRepository : IParticipantHttpRepository
	{
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly HttpClient _client;
		private readonly JsonSerializerOptions _options;
        private readonly IConfiguration _configuration;
        private readonly Models.BackEndConnections _backEndConnections;

		public ParticipantHttpRepository(IAccessTokenProvider accessTokenProvider,HttpClient client, IConfiguration configuration)
		{
            _accessTokenProvider = accessTokenProvider;
            _client = client;
			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _configuration = configuration;
            _backEndConnections = _configuration.GetSection("ConnectionStrings").Get<Models.BackEndConnections>();
        }

		public async Task<PagingResponse<Models.Participant>> GetParticipants(ParticipantParameters participantParameters)
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
	}
}
