using BlazorApplication.Models;
using BlazorApplication.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorApplication.Components.CompetitionTable
{
    public partial class CompetitionTable
    {
        [Inject]
        public IJSRuntime Js { get; set; }
     
        [Parameter]
        public EventCallback<int> OnDeleted { get; set; }

        [Parameter]
        public List<Competition> Competitions { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private void RedirectToUpdate(int id)
        {
            var url = Path.Combine("/updateCompetition/", id.ToString());
            NavigationManager.NavigateTo(url);
        }

        private async System.Threading.Tasks.Task Delete(int id)
        {
            var competition = Competitions.FirstOrDefault(p => p.id.Equals(id));
            var confirmed = await Js.InvokeAsync<bool>("confirm", $"Are you sure you want to delete {competition.CompetitionName} competition?");

            if (confirmed)
            {
                await OnDeleted.InvokeAsync(id);
            }
        }
    }
}
