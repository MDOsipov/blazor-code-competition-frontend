using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Pages
{
    public partial class UpdateTeam : ComponentBase
	{
		private Team _team = new Team();
        public List<Competition> competitionList { get; set; } = new List<Competition>();
        public List<Participant> participantList { get; set; } = new List<Participant>();
        public int leaderId { get; set; } = 0;
        public int competitionId { get; set; } = 0;

        private SuccessNotification _notification;

		[Inject]
		public ITeamHttpRepository TeamRepo { get; set; }
        [Inject]
        public ICompetitionHttpRepository CompetitionRepo { get; set; }
        [Inject]
        public IParticipantHttpRepository ParticipantRepo { get; set; }

        [Parameter]
		public string Id { get; set; } = "";

		protected async override Task OnInitializedAsync()
		{
			_team = await TeamRepo.GetTeamById(Id);

            leaderId = _team.TeamLeaderId;
            competitionId = _team.CompetitionId;

            CompetitionParameters competitionParameters = new CompetitionParameters
            {
                switchOff = true
            };

            var competitionPagingResponse = await CompetitionRepo.GetCompetitions(competitionParameters);
            competitionList = competitionPagingResponse.Items;

            ParticipantParameters participantParameters = new ParticipantParameters
            {
                switchOff = true
            };

            var participantPagingResponse = await ParticipantRepo.GetParticipants(participantParameters);
            participantList = participantPagingResponse.Items;
        }

		private async Task Update()
		{
            _team.TeamLeaderId = leaderId;
            _team.CompetitionId = competitionId;
            _team.StatusId = 1;

			await TeamRepo.UpdateTeam(_team);
			_notification.Show();
		}

	}
}
