using BlazorApplication.Features;
using BlazorApplication.Models;

namespace BlazorApplication.HttpRepository
{
    public interface ICompetitionHttpRepository
    {
        Task<PagingResponse<Competition>> GetCompetitions(CompetitionParameters competitionParameters);
    }
}
