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

        [Parameter]
        public string Id { get; set; } = "";

        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            await GetTeams();
            await GetParticipant();
        }
        private async System.Threading.Tasks.Task GetTeams()
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

        private async System.Threading.Tasks.Task GetParticipant()
        {
            try
            {
                _participant = await ParticipantRepo.GetParticipantById(Id);
                Console.WriteLine("Got participant object: ");
                Console.WriteLine(JsonSerializer.Serialize(_participant));
            }
            catch (Exception ex)
            {
                throw new System.Exception("Oops! Something went wrong while getting the participant to update!", ex);
            }
        }
        private async System.Threading.Tasks.Task Update()
        {
           try
           {
               await ParticipantRepo.UpdateParticipant(_participant);
               _notification.Show();
           }
           catch (Exception ex)
           {
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
