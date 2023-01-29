using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Pages
{
	public partial class TeamManagement
	{
		[Parameter]
		public string id { get; set; } = "";

		private string teamName = "";

        private ErrorBoundary? errorBoundary;

        [Inject]
		public ITeamHttpRepository teamRepo { get; set; }

		protected async override Task OnInitializedAsync()
		{
            await GetTeam();
        }
        private async Task GetTeam()
        {
            Team team;
            try
            {
                team = await teamRepo.GetTeamById(id);
                teamName = team.TeamName;
            }
            catch(Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting a team!", ex);
            }
        }
        protected override void OnParametersSet()
        {
            errorBoundary?.Recover();
        }
        private void ResetError()
        {
            errorBoundary?.Recover();
        }
    }
}
