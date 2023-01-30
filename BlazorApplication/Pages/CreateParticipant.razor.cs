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
        [Inject]
        public ILogger<CreateParticipant> Logger { get; set; }

		protected async override Task OnInitializedAsync()
        {
            await GetTeams();
        }

        private async Task GetTeams()
        {
            Logger.LogInformation("Get teams method is called");

            TeamParameters teamParameters = new TeamParameters
            {
                switchOff = true
            };

            try
            {
                PagingResponse<Team> teamListPaging = await TeamRepo.GetTeams(teamParameters);
                teamList = teamListPaging.Items;

                Logger.LogInformation($"Success. Teams: {JsonSerializer.Serialize(teamList)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting a list of teams!", ex);
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
            Logger.LogInformation("Create method is called");
            try
            {
                await ParticipantRepo.CreateParticipant(_participant);
                Logger.LogInformation($"Success. A new participant is created");
                _notification.Show();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while creating a new participant!", ex);
            }
        }
    }
}
