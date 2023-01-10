using BlazorApplication.Models;
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

		public async Task<ICollection<Participant>> GetParticipants()
		{
			var response = await _client.GetAsync("http://localhost:6060/participant/extended");
			var content = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				throw new ApplicationException(content);
			}

			return JsonSerializer.Deserialize<List<Participant>>(content, _options);

		}
	}
}
