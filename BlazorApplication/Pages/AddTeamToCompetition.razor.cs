using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Pages
{
	public partial class AddTeamToCompetition
	{
		[Parameter]
		public string competitionIdStr { get; set; } = "";

		public string navUrlToSend { get; set; } = "";
		public int competitionId { get; set; } = 0;
		private SuccessNotification? _notification;
		public int newTeamId { get; set; } = 0;

		[Inject]
		public ITeamHttpRepository TeamRepo { get; set; }

		private TeamParameters _teamParameters = new TeamParameters()
		{
			switchOff = true
		};
		public List<Team> TeamList { get; set; } = new List<Team>();

		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
			if (competitionIdStr != "")
			{
				competitionId = Int32.Parse(competitionIdStr);
				navUrlToSend = "/competitionTeamsManagement/" + competitionIdStr.ToString();
			}
			await GetTeams();
		}

		protected async System.Threading.Tasks.Task GetTeams()
		{
			var pagingResponse = await TeamRepo.GetTeamsLimited(_teamParameters);
			TeamList = pagingResponse.Items;
			newTeamId = TeamList.FirstOrDefault().Id;
		}

		private async void Create()
		{
			Team newTeam = await TeamRepo.GetTeamById(newTeamId.ToString());
			newTeam.CompetitionId = competitionId;

            await TeamRepo.UpdateTeam(newTeam);

			_notification.Show(); 
		}
	}
}
