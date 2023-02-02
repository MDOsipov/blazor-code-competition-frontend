using BlazorApplication.Features;

namespace BlazorApplication.Interfaces
{
    public interface ITeamHttpRepository
    {
        Task<PagingResponse<Models.Team>> GetTeams(TeamParameters teamParameters);
		Task<PagingResponse<Models.Team>> GetTeamsByCompetitionId(TeamParameters teamParameters, string competitionId);
		Task<PagingResponse<Models.Team>> GetTeamsLimited(TeamParameters teamParameters);
		Task CreateTeam(Models.Team team);
        Task<Models.Team> GetTeamById(string id);
        Task UpdateTeam(Models.Team team);
        Task DeleteTeam(int id);
        Task<string> UploadTeamImage(MultipartFormDataContent content);
    }
}
