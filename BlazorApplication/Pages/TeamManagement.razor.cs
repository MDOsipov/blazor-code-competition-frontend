using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics;
using System.Text.Json;
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

        [Inject]
        public ILogger<TeamManagement> Logger { get; set; }

		protected async override Task OnInitializedAsync()
		{
            await GetTeam();
        }
        private async Task GetTeam()
        {
            Logger.LogInformation("Get team method is called");
            Team team;
            try
            {
                team = await teamRepo.GetTeamById(id);
                teamName = team.TeamName;
                Logger.LogInformation($"Success. Team: {JsonSerializer.Serialize(team)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
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
