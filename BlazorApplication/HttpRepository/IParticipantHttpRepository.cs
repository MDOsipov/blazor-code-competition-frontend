using BlazorApplication.Features;
using BlazorApplication.Models;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.HttpRepository
{
	public interface IParticipantHttpRepository
	{
		Task<PagingResponse<Participant>> GetParticipants(ParticipantParameters participantParameters);
		Task CreateParticipant(Participant participant);
	}
}
