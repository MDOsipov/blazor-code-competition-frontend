using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Pages
{
    public partial class CreateTeam
    {
        private Team _team = new Team();

        public List<Competition> competitionList { get; set; } = new List<Competition>();
		public List<Participant> participantList { get; set; } = new List<Participant>();
        private ErrorBoundary? errorBoundary;
        public int leaderId { get; set; } = 1;
        public int competitionId { get; set; } = 1;


        private SuccessNotification _notification;

        [Inject]
        public ICompetitionHttpRepository CompetitionRepo { get; set; }
		[Inject]
		public IParticipantHttpRepository ParticipantRepo { get; set; }

        [Inject]
        public ILogger<CreateTeam> Logger { get; set; }

		[Inject]
        public ITeamHttpRepository TeamRepo { get; set; }

		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
            await GetCompetitions();
            await GetParticipants();
        }

        private async Task GetCompetitions()
        {
            Logger.LogInformation("Get competition method is called");
            CompetitionParameters competitionParameters = new CompetitionParameters
            {
                switchOff = true
            };

            try
            {
                var competitionPagingResponse = await CompetitionRepo.GetCompetitions(competitionParameters);
                competitionList = competitionPagingResponse.Items;
                Logger.LogInformation($"Success. Competitions: {JsonSerializer.Serialize(competitionList)}");
            }
            catch (Exception ex) 
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting competitions!", ex);
            }
        }

        private async Task GetParticipants()
        {
            Logger.LogInformation("Get participants method is called");
            ParticipantParameters participantParameters = new ParticipantParameters
            {
                switchOff = true
            };

            try 
            {
                var participantPagingResponse = await ParticipantRepo.GetParticipants(participantParameters);
                participantList = participantPagingResponse.Items;
                Logger.LogInformation($"Success. Participants: {JsonSerializer.Serialize(participantList)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting participants!", ex);
            }
        }

        private void AssignImageUrl(string imgUrl) => _team.IconImage = imgUrl; 

		private async void Create()
        {
            Logger.LogInformation("Create method is called");
            try
            {
                _team.CompetitionId = competitionId;
                _team.TeamLeaderId = leaderId;

                if (competitionId > 0 && leaderId > 0)
                {
                    await TeamRepo.CreateTeam(_team);
                    Logger.LogInformation($"Success. A new team is created");
                    _notification.Show();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while creating a new team!", ex);
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
