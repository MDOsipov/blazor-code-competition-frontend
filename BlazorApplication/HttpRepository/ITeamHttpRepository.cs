using BlazorApplication.Features;
using BlazorApplication.Models;

namespace BlazorApplication.HttpRepository
{
	public interface ITeamHttpRepository
	{
		Task<PagingResponse<Team>> GetTeams(TeamParameters teamParameters);
	}
}
