using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Pages
{
    public partial class CreateParticipant
    {
        private Participant _participant = new Participant();

        private SuccessNotification _notification;

		public List<Team> teamList { get; set; } = new List<Team>();

		[Inject]
        public IParticipantHttpRepository ParticipantRepo { get; set; }

		[Inject]
		public ITeamHttpRepository TeamRepo { get; set; }


		protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            TeamParameters teamParameters = new TeamParameters
            {
                switchOff = true
            };

			PagingResponse<Team> teamListPaging = await TeamRepo.GetTeams(teamParameters);
            teamList = teamListPaging.Items;
		}

        private async void Create()
        {
            await ParticipantRepo.CreateParticipant(_participant);
            _notification.Show();
        }
    }
}
