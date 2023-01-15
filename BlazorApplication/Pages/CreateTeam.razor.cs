using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Pages
{
    public partial class CreateTeam
    {
        private Team _team = new Team();

        public List<Competition> competitionList { get; set; } = new List<Competition>();
		public List<Participant> participantList { get; set; } = new List<Participant>();

        public int leaderId { get; set; } = 0;
        public int competitionId { get; set; } = 0;


        private SuccessNotification _notification;

        [Inject]
        public ICompetitionHttpRepository CompetitionRepo { get; set; }
		[Inject]
		public IParticipantHttpRepository ParticipantRepo { get; set; }

		[Inject]
        public ITeamHttpRepository TeamRepo { get; set; }

		protected async override System.Threading.Tasks.Task OnInitializedAsync()
		{
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

		private async void Create()
        {
            //_team.CreateDate = DateTime.Now;
            //_team.UpdateDate = DateTime.Now;
            //_team.CreateUserId = 1;
            //_team.UpdateUserId = 1;
            //_team.StatusId = (int)Enums.Status.Active;
            _team.TeamLeaderId = leaderId;
            _team.CompetitionId = competitionId;

            await TeamRepo.CreateTeam(_team);
            _notification.Show();
        }
    }
}
