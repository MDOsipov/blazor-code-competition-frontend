using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace BlazorApplication.Pages
{
    public partial class UpdateParticipant
    {
        private Participant _participant = new Participant();
        private SuccessNotification _notification;

        public List<Team> teamList { get; set; } = new List<Team>();

        [Inject]
        public ITeamHttpRepository TeamRepo { get; set; }

        [Inject]
        public IParticipantHttpRepository ParticipantRepo { get; set; }

        [Parameter]
        public string Id { get; set; } = "";

        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            TeamParameters teamParameters = new TeamParameters
            {
                switchOff = true
            };

            PagingResponse<Team> teamListPaging = await TeamRepo.GetTeams(teamParameters);
            teamList = teamListPaging.Items;

            _participant = await ParticipantRepo.GetParticipantById(Id);
            Console.WriteLine("Got participant object: ");
            Console.WriteLine(JsonSerializer.Serialize(_participant));
        }

        private async System.Threading.Tasks.Task Update()
        {
            Console.WriteLine("I'm here!");

            await ParticipantRepo.UpdateParticipant(_participant);
            _notification.Show();
        }
    }
}
