using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorApplication.Pages
{
    public partial class CreateParticipant
    {
        private Participant _participant = new Participant();

        private SuccessNotification _notification;
        private ErrorBoundary? errorBoundary;

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

            try
            {
                PagingResponse<Team> teamListPaging = await TeamRepo.GetTeams(teamParameters);
                teamList = teamListPaging.Items;
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting a list of teams!", ex);
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
        private async void Create()
        {
            try
            {
                await ParticipantRepo.CreateParticipant(_participant);
                _notification.Show();
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while creating a new participant!", ex);
            }
        }
    }
}
