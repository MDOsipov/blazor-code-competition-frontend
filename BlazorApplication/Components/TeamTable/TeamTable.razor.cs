using BlazorApplication.Models;
using BlazorApplication.Pages;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Task = System.Threading.Tasks.Task;

namespace BlazorApplication.Components.TeamTable
{
	public partial class TeamTable
	{
        [Inject]
        public IJSRuntime Js { get; set; }

        [Parameter]
		public List<Team> Teams { get; set; }

        [Parameter]
        public EventCallback<int> OnDeleted { get; set; }


        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private void RedirectToUpdate(int id)
        {
            var url = Path.Combine("/updateTeam/", id.ToString());
            Console.WriteLine("Url - " + url);
            NavigationManager.NavigateTo(url);
        }

        private async Task Delete(int id)
        {
            var team = Teams.FirstOrDefault(p => p.Id.Equals(id));
            var confirmed = await Js.InvokeAsync<bool>("confirm", $"Are you sure you want to delete {team.TeamName} team?");

            if (confirmed)
            {
                await OnDeleted.InvokeAsync(id);
            }
        }
    }
}
