using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

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
			try
			{
                var pagingResponse = await TeamRepo.GetTeamsLimited(_teamParameters);
                TeamList = pagingResponse.Items;
                newTeamId = TeamList.FirstOrDefault().Id;
            }
			catch (Exception ex)
			{
                throw new System.Exception("Oops! Something went wrong while getting a list of teams!", ex);
            }
        }

		private async void Create()
		{
			Team newTeam;
			try
			{
                newTeam = await TeamRepo.GetTeamById(newTeamId.ToString());
                newTeam.CompetitionId = competitionId;
            }
			catch (Exception ex)
			{
                throw new System.Exception("Oops! Something went wrong while getting a team to add!", ex);
            }
			
			try
			{
                await TeamRepo.UpdateTeam(newTeam);
				_notification.Show();
            }
            catch (Exception ex)
			{
                throw new System.Exception("Oops! Something went wrong while adding a team to competition!", ex);
            }
        }
	}
}
