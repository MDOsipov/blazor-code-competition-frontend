﻿using BlazorApplication.Features;
using BlazorApplication.Models;
using BlazorApplication.Pages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Json;

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

        public System.Threading.Tasks.Task DeleteParticipant(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Participant> GetParticipantById(string id)
        {
            var url = Path.Combine("http://localhost:6060/participant", id);

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

        public async System.Threading.Tasks.Task UpdateParticipant(Participant participant)
        {
            var content = JsonSerializer.Serialize(participant);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var url = Path.Combine("http://localhost:6060/participant", participant.id.ToString());

            var putResult = await _client.PutAsync(url, bodyContent);
            var putContent = await putResult.Content.ReadAsStringAsync();

            if (!putResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(putContent);
            }
        }
    }
}
