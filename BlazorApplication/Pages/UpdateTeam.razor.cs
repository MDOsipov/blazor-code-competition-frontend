using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Pages
{
    public partial class UpdateTeam : ComponentBase
	{
		private Team _team = new Team();
        public List<Competition> competitionList { get; set; } = new List<Competition>();
        public List<Participant> participantList { get; set; } = new List<Participant>();
        public int leaderId { get; set; } = 1;
        public int competitionId { get; set; } = 1;

        private SuccessNotification _notification;
        private ErrorBoundary? errorBoundary;

        [Inject]
		public ITeamHttpRepository TeamRepo { get; set; }
        [Inject]
        public ICompetitionHttpRepository CompetitionRepo { get; set; }
        [Inject]
        public IParticipantHttpRepository ParticipantRepo { get; set; }

        [Parameter]
		public string Id { get; set; } = "";
        private void AssignImageUrl(string imgUrl) => _team.IconImage = imgUrl;
        protected async override Task OnInitializedAsync()
		{
            await GetTeam();

            leaderId = _team.TeamLeaderId;
            if(_team.CompetitionId is not null)
            {
                competitionId = (int)_team.CompetitionId;
            }

            await GetCompetitions();
            await GetParticipants();
        }

        private async Task GetTeam()
        {
            try
            {
                _team = await TeamRepo.GetTeamById(Id);
            }
            catch(Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting the team to update!", ex);
            }
        }

        private async Task GetCompetitions()
        {
            CompetitionParameters competitionParameters = new CompetitionParameters
            {
                switchOff = true
            };

            try
            {
                var competitionPagingResponse = await CompetitionRepo.GetCompetitions(competitionParameters);
                competitionList = competitionPagingResponse.Items;
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting a list of competitions!", ex);
            }
        }

        private async Task GetParticipants()
        {
            ParticipantParameters participantParameters = new ParticipantParameters
            {
                switchOff = true
            };

            try
            {
                var participantPagingResponse = await ParticipantRepo.GetParticipants(participantParameters);
                participantList = participantPagingResponse.Items;
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting a list of participants!", ex);
            }
        }
        private async Task Update()
		{
            _team.TeamLeaderId = leaderId;
            _team.CompetitionId = competitionId;
            _team.StatusId = 1;

            try
            {
                await TeamRepo.UpdateTeam(_team);
                _notification.Show();
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while updating the team!", ex);
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
