using BlazorApplication.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Pages
{
    public partial class CompetitionManagement
    {
        [Parameter]
        public string id { get; set; } = "";

        private string competitionName = "";

        [Inject]
        public ICompetitionHttpRepository competitionRepo { get; set; }

        protected async override Task OnInitializedAsync()
        {
            var competition = await competitionRepo.GetCompetitionById(id);
            competitionName = competition.CompetitionName;
        }
    }
}
