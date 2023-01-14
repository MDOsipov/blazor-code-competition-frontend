using BlazorApplication.Features;
using BlazorApplication.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;

namespace BlazorApplication.HttpRepository
{
	public class ParticipantHttpRepository : IParticipantHttpRepository
	{
		private readonly HttpClient _client;
		private readonly JsonSerializerOptions _options;

		public ParticipantHttpRepository(HttpClient client)
		{
			_client = client;
			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
		}

		public async Task<PagingResponse<Participant>> GetParticipants(ParticipantParameters participantParameters)
		{
            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = participantParameters.PageNumber.ToString(),
                ["switchOff"] = participantParameters.switchOff ? "1" : "0"
            };

            var response = await _client.GetAsync(QueryHelpers.AddQueryString("http://localhost:6060/participant/extended", queryStringParam));
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

            return pagingResponse;

        }
	}
}
