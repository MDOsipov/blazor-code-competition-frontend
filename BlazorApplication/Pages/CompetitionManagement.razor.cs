using BlazorApplication.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

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

        protected async override Task OnInitializedAsync()
        {
            try
            {
                var competition = await competitionRepo.GetCompetitionById(id);
                competitionName = competition.CompetitionName;
            }
            catch(Exception ex) 
            {
                throw new System.Exception("Oops! Something went wrong while getting the competition!", ex);
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
