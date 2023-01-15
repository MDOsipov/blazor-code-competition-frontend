using BlazorApplication.Features;
using BlazorApplication.Models;

namespace BlazorApplication.Interfaces
{
    public interface ICompetitionHttpRepository
    {
        Task<PagingResponse<Competition>> GetCompetitions(CompetitionParameters competitionParameters);
    }
}
