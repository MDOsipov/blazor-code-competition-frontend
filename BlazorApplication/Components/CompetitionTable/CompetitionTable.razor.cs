using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorApplication.Components.CompetitionTable
{
    public partial class CompetitionTable
    {
        [Parameter]
        public List<Competition> Competitions { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private void RedirectToUpdate(int id)
        {
            var url = Path.Combine("/updateCompetition/", id.ToString());
            NavigationManager.NavigateTo(url);
        }
    }
}
