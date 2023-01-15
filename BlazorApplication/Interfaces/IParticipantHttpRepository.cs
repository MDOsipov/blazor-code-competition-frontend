using BlazorApplication.Features;
using BlazorApplication.Models;

namespace BlazorApplication.Interfaces
{
    public interface IParticipantHttpRepository
    {
        Task<PagingResponse<Participant>> GetParticipants(ParticipantParameters participantParameters);
    }
}
