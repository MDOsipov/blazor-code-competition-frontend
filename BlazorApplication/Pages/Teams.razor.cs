using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace BlazorApplication.Pages
{
    public partial class Teams
	{
		public List<Team> TeamList { get; set; } = new List<Team>();
		public MetaData MetaData { get; set; } = new MetaData();
		private TeamParameters _teamParameters = new TeamParameters();

		[Inject]
		public ITeamHttpRepository TeamRepo { get; set; }

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
			var pagingResponse = await TeamRepo.GetTeams(_teamParameters);
			TeamList = pagingResponse.Items;
			MetaData = pagingResponse.MetaData;
			successResponse = pagingResponse.SuccessRequest;
			//Console.WriteLine("Team list:" + JsonSerializer.Serialize(TeamList));
		}

        private async System.Threading.Tasks.Task DeleteTeam(int id)
        {
            await TeamRepo.DeleteTeam(id);
            _teamParameters.PageNumber = 1;
            await GetTeams();
        }

    }
}
