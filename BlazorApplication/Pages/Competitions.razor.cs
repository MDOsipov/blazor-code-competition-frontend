using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json;

namespace BlazorApplication.Pages
{
    public partial class Competitions
    {
        public List<Competition> CompetitionList { get; set; } = new List<Competition>();
        public MetaData MetaData { get; set; } = new MetaData();
        private CompetitionParameters _competitionParameters = new CompetitionParameters();
        private ErrorBoundary? errorBoundary;

        [Inject]
        public ICompetitionHttpRepository CompetitionRepo { get; set; }
        [Inject]
        public ILogger<Competitions> Logger { get; set; }
        protected async override System.Threading.Tasks.Task OnInitializedAsync()
        {
            await GetCompetitions();
        }
        protected override void OnParametersSet()
        {
            errorBoundary?.Recover();
        }
        private void ResetError()
        {
            errorBoundary?.Recover();
        }

        private async System.Threading.Tasks.Task SelectedPage(int page)
        {
            _competitionParameters.PageNumber = page;
            await GetCompetitions();
        }
        
        protected async System.Threading.Tasks.Task GetCompetitions()
        {
            Logger.LogInformation("Get competitions method is called");
            try
            {
                var pagingResponse = await CompetitionRepo.GetCompetitions(_competitionParameters);
                CompetitionList = pagingResponse.Items;
                MetaData = pagingResponse.MetaData;
                Logger.LogInformation($"Success. Competitions: {JsonSerializer.Serialize(CompetitionList)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while getting a list of competitions!", ex);
            }
        }

        private async System.Threading.Tasks.Task DeleteCompetition(int id)
        {
            Logger.LogInformation("Delete competition method is called");
            try
            {
                await CompetitionRepo.DeleteCompetition(id);
                _competitionParameters.PageNumber = 1;
                Logger.LogInformation($"Success. Competition is deleted");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new System.Exception("Oops! Something went wrong while deleting a competition!", ex);
            }
            await GetCompetitions();
        }
    }
}
