using BlazorApplication.Features;
using BlazorApplication.Models;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Interfaces
{
    public interface ICompetitionHttpRepository
    {
        Task<PagingResponse<Competition>> GetCompetitions(CompetitionParameters competitionParameters);
		Task<PagingResponse<Competition>> GetCompetitionsByAdminId(string adminId, CompetitionParameters competitionParameters);
		Task<ResponseWithSuccess<CompetitionStatus>> GetAllCompetitionStatuses();
		Task CreateCompetition(Competition competition);
        Task<Competition> GetCompetitionById(string id);
        Task UpdateCompetition(Competition competition);
        Task DeleteCompetition(int id);

    }
}
