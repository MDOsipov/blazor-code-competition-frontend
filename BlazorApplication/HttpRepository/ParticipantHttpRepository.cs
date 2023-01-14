using BlazorApplication.Features;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BlazorApplication.HttpRepository
{
	public class ParticipantHttpRepository : IParticipantHttpRepository
	{
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly HttpClient _client;
		private readonly JsonSerializerOptions _options;

		public ParticipantHttpRepository(IAccessTokenProvider accessTokenProvider,HttpClient client)
		{
            _accessTokenProvider = accessTokenProvider;
            _client = client;
			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
		}

		public async Task<ICollection<Participant>> GetParticipants()
		{
			await AddToken.RequestAuthToken(_accessTokenProvider, _client);
            var response = await _client.GetAsync("http://localhost:6061/participant/extended");
			var content = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				throw new ApplicationException(content);
			}

			return JsonSerializer.Deserialize<List<Participant>>(content, _options);
		}

        
    }
}
