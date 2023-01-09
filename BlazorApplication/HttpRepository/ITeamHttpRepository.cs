using BlazorApplication.Features;
using BlazorApplication.Models;

namespace BlazorApplication.HttpRepository
{
	public interface ITeamHttpRepository
	{
		Task<PagingResponse<Team>> GetTeams(TeamParameters teamParameters);
        System.Threading.Tasks.Task CreateTeam(Team team);
	}
}
