using BlazorApplication.Features;
using BlazorApplication.Models;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Interfaces
{
    public interface IParticipantHttpRepository
    {
        Task<PagingResponse<Participant>> GetParticipants(ParticipantParameters participantParameters);
        Task CreateParticipant(Participant participant);
        Task<Participant> GetParticipantById(string id);
        Task UpdateParticipant(Participant participant);
        Task DeleteParticipant(int id);
    }
}
