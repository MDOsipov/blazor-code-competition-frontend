using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json;

namespace BlazorApplication.Pages
{
    public partial class Teams
	{
		public List<Team> TeamList { get; set; } = new List<Team>();
		public MetaData MetaData { get; set; } = new MetaData();
		private TeamParameters _teamParameters = new TeamParameters();
        private ErrorBoundary? errorBoundary;

        [Inject]
		public ITeamHttpRepository TeamRepo { get; set; }

        [Inject]
        public ILogger<Teams> Logger { get; set; }

        [Parameter]
		public bool successResponse { get; set; }

		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
			await GetTeams();
		}
		private async System.Threading.Tasks.Task SelectedPage(int page)
		{
			_teamParameters.PageNumber = page;
			await GetTeams();	
		}

		protected async System.Threading.Tasks.Task GetTeams()
		{
            Logger.LogInformation("Get teams method is called");
            try
            {
                var pagingResponse = await TeamRepo.GetTeams(_teamParameters);
                TeamList = pagingResponse.Items;
                MetaData = pagingResponse.MetaData;
                successResponse = pagingResponse.SuccessRequest;
                Logger.LogInformation($"Success. Teams: {JsonSerializer.Serialize(TeamList)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting a list of teams!", ex);
            }
        }

        private async System.Threading.Tasks.Task DeleteTeam(int id)
        {
            Logger.LogInformation("Delete team method is called");
            try
            {
                await TeamRepo.DeleteTeam(id);
                _teamParameters.PageNumber = 1;
                Logger.LogInformation($"Success. The team is deleted");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while deleting a team!", ex);
            }
            await GetTeams();
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
