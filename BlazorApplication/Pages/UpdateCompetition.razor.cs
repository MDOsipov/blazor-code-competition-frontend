using BlazorApplication.HttpRepository;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using BlazorApplication.Shared;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Pages
{
    public partial class UpdateCompetition
    {
        private Competition _competition = new Competition();
        public List<CompetitionStatus> competitionStatusesList { get; set; } = new List<CompetitionStatus>();
        private SuccessNotification _notification;

        [Inject]
        public ICompetitionHttpRepository CompetitionRepo { get; set; }

        [Parameter]
        public string Id { get; set; } = "";

        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            _competition = await CompetitionRepo.GetCompetitionById(Id);
            Console.WriteLine("Got competition object: ");
            Console.WriteLine(JsonSerializer.Serialize(_competition));
            competitionStatusesList = (List<CompetitionStatus>)await CompetitionRepo.GetAllCompetitionStatuses();
        }

        private async Task Update()
        {
            Console.WriteLine("I'm here!");

            await CompetitionRepo.UpdateCompetition(_competition);
            _notification.Show();
        }
    }
}
