using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Pages
{
    public partial class Competitions
    {
        public List<Competition> CompetitionList { get; set; } = new List<Competition>();
        public MetaData MetaData { get; set; } = new MetaData();
        private CompetitionParameters _competitionParameters = new CompetitionParameters();

        [Parameter]

        public bool successResponse { get; set; }

        [Inject]
        public ICompetitionHttpRepository CompetitionRepo { get; set; }

        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            await GetCompetitions();
        }

        
        private async System.Threading.Tasks.Task SelectedPage(int page)
        {
            _competitionParameters.PageNumber = page;
            await GetCompetitions();
        }
        

        protected async System.Threading.Tasks.Task GetCompetitions()
        {
            var pagingResponse = await CompetitionRepo.GetCompetitions(_competitionParameters);
            CompetitionList = pagingResponse.Items;
            MetaData = pagingResponse.MetaData;
            successResponse = pagingResponse.SuccessRequest;
        }

        private async System.Threading.Tasks.Task DeleteCompetition(int id)
        {
            await CompetitionRepo.DeleteCompetition(id);
            _competitionParameters.PageNumber = 1;
            await GetCompetitions();
        }
    }
}
