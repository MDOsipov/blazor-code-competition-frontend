using BlazorApplication.Features;
using BlazorApplication.Models;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.HttpRepository
{
	public interface ITeamHttpRepository
	{
		Task<PagingResponse<Team>> GetTeams(TeamParameters teamParameters);
        Task CreateTeam(Team team);
		Task<Team> GetTeamById(string id);
		Task UpdateTeam(Team team);
		Task DeleteTeam(int id);
	}
}
