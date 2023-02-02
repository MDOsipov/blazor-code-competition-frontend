using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

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
        private ErrorBoundary? errorBoundary;

        [Inject]
		public ITeamHttpRepository TeamRepo { get; set; }
		[Inject]
		public ILogger<AddTeamToCompetition> Logger { get; set; }

		private TeamParameters _teamParameters = new TeamParameters()
		{
			switchOff = true
		};
		public List<Team> TeamList { get; set; } = new List<Team>();
		public List<Team> CurrentTeamList { get; set; } = new List<Team>();
		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
			if (competitionIdStr != "")
			{
				competitionId = Int32.Parse(competitionIdStr);
				navUrlToSend = "/competitionTeamsManagement/" + competitionIdStr.ToString();
			}
			await GetCurrentTeams();
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
        protected async System.Threading.Tasks.Task GetTeams()
		{
            Logger.LogInformation("Get teams method is called");
            try
            {
                var pagingResponse = await TeamRepo.GetTeamsLimited(_teamParameters);
                TeamList = pagingResponse.Items;
				TeamList = TeamList.Where(tl => CurrentTeamList.Select(ctl => ctl.Id).ToList().IndexOf(tl.Id) == -1).ToList();
				newTeamId = TeamList.FirstOrDefault().Id;
                Logger.LogInformation($"Success. Team list: {JsonSerializer.Serialize(TeamList)}");
            }
            catch (Exception ex)
			{
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting a list of teams!", ex);
            }
        }

		protected async System.Threading.Tasks.Task GetCurrentTeams()
		{
			Logger.LogInformation("Get current teams method is called");
			try
			{
				var pagingResponse = await TeamRepo.GetTeamsByCompetitionId(_teamParameters, competitionIdStr);
				CurrentTeamList = pagingResponse.Items;
				Logger.LogInformation($"Success. Current team list: {JsonSerializer.Serialize(CurrentTeamList)}");
			}
			catch (Exception ex)
			{
				Logger.LogError($"Error: {ex}");
				throw new System.Exception("Oops! Something went wrong while getting a list of current teams!", ex);
			}
		}

		private async void Create()
		{
            Logger.LogInformation("Creat method is called");
            Team newTeam;
			try
			{
                newTeam = await TeamRepo.GetTeamById(newTeamId.ToString());
                newTeam.CompetitionId = competitionId;
            }
            catch (Exception ex)
			{
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting a team to add!", ex);
            }
			
			try
			{
                await TeamRepo.UpdateTeam(newTeam);
                Logger.LogInformation($"Success. A new team to the competition is added!");
                _notification.Show();
            }
            catch (Exception ex)
			{
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while adding a team to competition!", ex);
            }
        }
	}
}
