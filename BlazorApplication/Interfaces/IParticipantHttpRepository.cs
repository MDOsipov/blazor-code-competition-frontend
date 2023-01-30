using BlazorApplication.Features;
using BlazorApplication.Models;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Interfaces
{
    public interface IParticipantHttpRepository
    {
        Task<PagingResponse<Participant>> GetParticipants(ParticipantParameters participantParameters);
        Task<ResponseWithSuccess<Participant>> GetParticipantsLimited();
        Task CreateParticipant(Participant participant);
        Task<Participant> GetParticipantById(string id);
        Task<PagingResponse<Participant>> GetParticipantsByEmail(ParticipantParameters participantParameters, string email);
        Task<PagingResponse<Participant>> GetParticipantsByTeamId(ParticipantParameters participantParameters, string teamId);
        Task AddTeamToParticipant(string teamId, string participantId);
        Task RemoveTeamFromParticipant(string participantId);
        Task UpdateParticipant(Participant participant);
        Task DeleteParticipant(int id);
    }
}
