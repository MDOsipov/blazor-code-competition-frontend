using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json;

namespace BlazorApplication.Pages
{
    public partial class UpdateParticipant
    {
        private Participant _participant = new Participant();
        private SuccessNotification _notification;
        private ErrorBoundary? errorBoundary;
        public List<Team> teamList { get; set; } = new List<Team>();

        [Inject]
        public ITeamHttpRepository TeamRepo { get; set; }

        [Inject]
        public IParticipantHttpRepository ParticipantRepo { get; set; }

        [Inject]
        public ILogger<UpdateParticipant> Logger { get; set; }

        [Parameter]
        public string Id { get; set; } = "";

        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            await GetTeams();
            await GetParticipant();
        }
        private async System.Threading.Tasks.Task GetTeams()
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
                throw new System.Exception("Oops! Something went wrong while getting a list of teams!", ex);
            }
        }

        private async System.Threading.Tasks.Task GetParticipant()
        {
            Logger.LogInformation("Get participant method is called");
            try
            {
                _participant = await ParticipantRepo.GetParticipantById(Id);
                Logger.LogInformation($"Success. Participant: {JsonSerializer.Serialize(_participant)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting the participant to update!", ex);
            }
        }
        private async System.Threading.Tasks.Task Update()
        {
            Logger.LogInformation("Update method is called");
            try
            {
                await ParticipantRepo.UpdateParticipant(_participant);
                Logger.LogInformation($"Success. The participant is updated");
                _notification.Show();
            }
           catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while updating the participant!", ex);
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
