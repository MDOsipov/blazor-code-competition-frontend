using BlazorApplication.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace BlazorApplication.Pages
{
    public partial class CompetitionManagement
    {
        [Parameter]
        public string id { get; set; } = "";

        private string competitionName = "";

        private ErrorBoundary? errorBoundary;

        [Inject]
        public ICompetitionHttpRepository competitionRepo { get; set; }
        [Inject]
        public ILogger<CompetitionManagement> Logger { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await GetCompetition();
        }

        private async Task GetCompetition()
        {
            Logger.LogInformation("Get competition method is called");
            try
            {
                var competition = await competitionRepo.GetCompetitionById(id);
                competitionName = competition.CompetitionName;
                Logger.LogInformation($"Success. Competition: {JsonSerializer.Serialize(competition)}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex}");
                throw new Exception("Oops! Something went wrong while getting the competition!", ex);
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
