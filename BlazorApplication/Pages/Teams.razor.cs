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
			try
			{
                var pagingResponse = await TeamRepo.GetTeams(_teamParameters);
                TeamList = pagingResponse.Items;
                MetaData = pagingResponse.MetaData;
                Console.WriteLine("Team list:" + JsonSerializer.Serialize(TeamList));
            }
            catch(Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting a list of teams!", ex);
            }
        }

        private async System.Threading.Tasks.Task DeleteTeam(int id)
        {
            try
            {
                await TeamRepo.DeleteTeam(id);
                _teamParameters.PageNumber = 1;
            }
            catch (Exception ex)
            {
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
