using BlazorApplication.Features;
using BlazorApplication.Models;
using BlazorApplication.Pages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
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

        public async System.Threading.Tasks.Task CreateParticipant(Participant participant)
        {
            var content = JsonSerializer.Serialize(participant);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            var postResult = await _client.PostAsync("http://localhost:6060/participant", bodyContent);
            var postContent = await postResult.Content.ReadAsStringAsync();

            if (!postResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(postContent);
            }
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
