using BlazorApplication.Features;
using BlazorApplication.Models;

namespace BlazorApplication.HttpRepository
{
	public interface IParticipantHttpRepository
	{
		Task<PagingResponse<Participant>> GetParticipants(ParticipantParameters participantParameters);
	}
}
