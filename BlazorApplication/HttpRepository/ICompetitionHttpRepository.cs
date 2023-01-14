using BlazorApplication.Features;
using BlazorApplication.Models;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.HttpRepository
{
    public interface ICompetitionHttpRepository
    {
        Task<PagingResponse<Competition>> GetCompetitions(CompetitionParameters competitionParameters);
		Task<IEnumerable<CompetitionStatus>> GetAllCompetitionStatuses();
		Task CreateCompetition(Competition competition);

        Task<Competition> GetCompetitionById(string id);
        Task UpdateCompetition(Competition competition);
        Task DeleteCompetition(int id);

    }
}
