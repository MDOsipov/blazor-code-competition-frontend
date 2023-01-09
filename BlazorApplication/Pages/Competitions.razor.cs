using BlazorApplication.HttpRepository;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Pages
{
    public partial class Competitions
    {
        public List<Competition> CompetitionList { get; set; } = new List<Competition>();
        // public MetaData MetaData { get; set; } = new MetaData();
        // private TeamParameters _teamParameters = new TeamParameters();

        [Inject]
        public ICompetitionHttpRepository CompetitionRepo { get; set; }

        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            await GetCompetitions();
        }

        /*
        private async System.Threading.Tasks.Task SelectedPage(int page)
        {
            _teamParameters.PageNumber = page;
            await GetTeams();
        }
        */

        protected async System.Threading.Tasks.Task GetCompetitions()
        {
            CompetitionList = (List<Competition>)await CompetitionRepo.GetCompetitions();
        }
    }
}
